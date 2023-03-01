using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Vector3Int = UnityEngine.Vector3Int;

// TODO рефакторинг
public class Ground : MonoBehaviour
{
    private readonly Vector2 Offset = new (0.5f, 0.5f);

    [Header("Base settings")]
    [SerializeField] private Hints hints;
    
    [Header("Map settings")]
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public Vector2Int mapSize;
    [SerializeField] private Vector2Int startRoomSize;
    [SerializeField] public int enemyBasesCount;
    [SerializeField] private int basesFragmentsCount;
    [SerializeField] public int enemyBaseOffset;
    [SerializeField] public int basesFragmentOffset;

    [Header("Player")]
    [SerializeField] private GameObject player;

    /*
     * null - 0
     * groundTile - 1
     * hiddenGroundTile - 2
     * enemyBaseTile - 3
     * baseTile - 4
     * baseFragmentTile - 5
     * barrierTile - 6
     */
    [Header("Tiles")]
    [SerializeField] public TileInfo[] tilesList;

    [Header("Effects")]
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private Animator cameraAnimator;

    [Header("Enemy Base")] [SerializeField]
    private GameObject enemyBasePrefab;

    [Header("Base")]
    [SerializeField] private GameObject baseFragmentPrefab;
    [SerializeField] private GameObject basePrefab;

    [Header("Pathfinding")]
    [SerializeField] private Pathfinding pathfinding;

    [HideInInspector] public int[,] mapMatrix;
    [HideInInspector] private float[,] mapHealthMatrix;
    [HideInInspector] private IHpBar[,] mapSpecialTilesMatrix;
    [HideInInspector] private FragmentsController fragmentsController;

    private void Start()
    {
        fragmentsController = FindObjectOfType<FragmentsController>();

        SetupMap();
        SetupStartRoom();

        Vector3Int position = new Vector3Int(mapSize.x / 2, mapSize.y / 2, 0);
        Vector3 worldCenter = tilemap.CellToWorld(position) + new Vector3(Offset.x, Offset.y, -1);
        player.transform.position = worldCenter;

        SetupMapObject(enemyBasesCount, tilesList[3], enemyBaseOffset);
        SetupMapObject(basesFragmentsCount, tilesList[5], basesFragmentOffset);
    }

    private void SetupStartRoom()
    {
        Vector3Int position = new Vector3Int(mapSize.x / 2, mapSize.y / 2, 0);
        for (int i = -startRoomSize.x / 2; i <= startRoomSize.x / 2; i++)
        {
            for (int j = -startRoomSize.x / 2; j <= startRoomSize.y / 2; j++)
            {
                position = new Vector3Int(mapSize.x / 2, mapSize.y / 2, 0) + new Vector3Int(i, j, 0);
                UpdateTile(position, tilesList[0], effect: false);
                foreach (Vector3Int pos in GetFiledGroundTiles(position))
                {
                    UpdateTileToMatrixIndex(pos);
                }
            }
        }

        UpdateTile(position, tilesList[4], effect: false);
        CreateTilePrefab(position, true);
        SetMatrixHp(tilesList[4], position);
        foreach (Vector3Int pos in GetFiledGroundTiles(position))
        {
            UpdateTileToMatrixIndex(pos);
        }
    }

    private void SetupMap()
    {
        pathfinding.binaryMap = new bool[mapSize.x, mapSize.y];
        mapMatrix = new int[mapSize.x, mapSize.y];
        mapHealthMatrix = new float[mapSize.x, mapSize.y];
        mapSpecialTilesMatrix = new IHpBar[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (x == 0 || x == mapSize.x - 1 || y == 0 || y == mapSize.y - 1)
                    UpdateTile(new Vector3Int(x, y, 0), tilesList[6], setup:true);
                else
                    UpdateTile(new Vector3Int(x, y, 0), tilesList[2]);
            }
        }
    }

    private float GetMatrixHp(Vector3Int position)
    {
        return mapHealthMatrix[position.x, position.y];
    }

    private int GetMatrixValue(Vector3Int position)
    {
        if (position.x >= 0 && position.x < mapSize.x &&
            position.y >= 0 && position.y < mapSize.y)
            return mapMatrix[position.x, position.y];

        return -1;
    }

    private void SetMatrixHp(TileInfo tile, Vector3Int position)
    {
        mapHealthMatrix[position.x, position.y] = tile.health;
    }

    private void SetMatrixHp(Vector3Int position, float value)
    {
        mapHealthMatrix[position.x, position.y] = value;
    }

    private void SetMatrixValue(Vector3Int position, int value)
    {
        mapMatrix[position.x, position.y] = value;
        pathfinding.binaryMap[position.x, position.y] = value == 0;
    }

    private void UpdateMatrixHpBar(Vector3Int position)
    {
        mapSpecialTilesMatrix[position.x, position.y]
            .UpdateHpBar(mapHealthMatrix[position.x, position.y]);
    }

    private bool CheckBarrierTile(Vector3Int position)
    {
        return GetMatrixValue(position) != 6;
    }

    private IHpBar GetSpecialMatrix(Vector3Int position)
    {
        return mapSpecialTilesMatrix[position.x, position.y];
    }

    private void SetSpecialMatrix(Vector3Int position, IHpBar value)
    {
        mapSpecialTilesMatrix[position.x, position.y] = value;
    }

    public void DrillTile(Vector3 position)
    {
        Vector3Int drillWorldPosition = tilemap.WorldToCell(position);
        if (GetMatrixValue(drillWorldPosition) == 0) return;
        if (!CheckBarrierTile(drillWorldPosition) || !CheckGroundAround(drillWorldPosition))
            return; // если стена или нет прямого доступа
        if (GetMatrixValue(drillWorldPosition) == 4 && !fragmentsController.HasUnusedFragment())
        {
            RunShakeEffect();
            hints.ShowHint(2);
            return;
        }

        if (GetMatrixHp(drillWorldPosition) > 0)
        {
            SetMatrixHp(drillWorldPosition, GetMatrixHp(drillWorldPosition) - Time.deltaTime);
            if (GetSpecialMatrix(drillWorldPosition) != null)
            {
                UpdateMatrixHpBar(drillWorldPosition);
            }
        }
        else
        {
            if (GetMatrixValue(drillWorldPosition) != 4)
                UpdateTile(drillWorldPosition, tilesList[0],
                    GetTileInfoByTile((Tile) tilemap.GetTile(drillWorldPosition)));
            else
            {
                SetMatrixHp(drillWorldPosition, tilesList[4].health);
                UpdateMatrixHpBar(drillWorldPosition);
                CreateEffect(drillWorldPosition, tilesList[4]);
            }

            foreach (Vector3Int pos in GetFiledGroundTiles(drillWorldPosition))
            {
                UpdateTileToMatrixIndex(pos);
            }

            if (GetSpecialMatrix(drillWorldPosition) == null) return;
            if (GetSpecialMatrix(drillWorldPosition).Destroy())
                SetSpecialMatrix(drillWorldPosition, null);
        }
    }

    private void UpdateTileToMatrixIndex(Vector3Int position)
    {
        int index = GetMatrixValue(position);
        UpdateTile(position, index == 2 ? tilesList[1] : GetTileByIndex(index));
    }

    private void UpdateTile(Vector3Int position, TileInfo tile, TileInfo tileBefore = null, bool setup = false,
        bool effect = true)
    {
        SetMatrixValue(position, tile.index);
        if (GetMatrixHp(position) == 0 ||
            GetMatrixHp(position) == tilesList[2].health)
            SetMatrixHp(tile, position);
        if (!setup && CheckGroundAround(position) || tile.tile == null)
        {
            CreateTilePrefab(position);
            tilemap.SetTile(position, tile.tile);
        }

        if (tile.tile == null && effect)
        {
            CreateEffect(position, tileBefore);
        }
    }

    private void RunShakeEffect()
    {
        cameraAnimator.SetTrigger("Shake");
    }

    private void CreateEffect(Vector3Int position, TileInfo tile = null)
    {
        ParticleSystem particles = Instantiate(explosionParticles,
            tilemap.CellToWorld(position) + new Vector3(Offset.x, Offset.x, -1), Quaternion.identity);
        if (tile != null)
            particles.startColor = tile.color;
        particles.Play();
        RunShakeEffect();
    }

    private void CreateTilePrefab(Vector3Int position, bool setup = false)
    {
        if ((tilemap.GetTile(position) == tilesList[2].tile || setup) &&
            GetSpecialMatrix(position) == null)
        {
            IHpBar component;
            switch (GetMatrixValue(position))
            {
                case 3:
                {
                    component = Instantiate(enemyBasePrefab,
                            tilemap.CellToWorld(position) + new Vector3(Offset.x, Offset.x, 0),
                            Quaternion.identity)
                        .GetComponent<IHpBar>();
                    SetSpecialMatrix(position, component);
                    break;
                }
                case 4:
                    component = Instantiate(basePrefab,
                            tilemap.CellToWorld(position) + new Vector3(Offset.x, Offset.x, 0),
                            Quaternion.identity)
                        .GetComponent<IHpBar>();
                    SetSpecialMatrix(position, component);
                    break;
                case 5:
                    component = Instantiate(baseFragmentPrefab,
                            tilemap.CellToWorld(position) + new Vector3(Offset.x, Offset.x, 0),
                            Quaternion.identity)
                        .GetComponent<IHpBar>();
                    SetSpecialMatrix(position, component);
                    break;
            }
        }
    }

    private TileInfo GetTileByIndex(int index)
    {
        foreach (TileInfo tileInfo in tilesList)
        {
            if (tileInfo.index == index) return tileInfo;
        }

        return tilesList[0];
    }

    private TileInfo GetTileInfoByTile(Tile tile)
    {
        foreach (TileInfo tileInfo in tilesList)
        {
            if (tileInfo.tile == tile) return tileInfo;
        }

        return tilesList[0];
    }

    private bool CheckGroundAround(Vector3Int position)
    {
        List<Vector3Int> list = GetDirectionList(position);
        foreach (Vector3Int pos in list)
        {
            if (GetMatrixValue(pos) == 0)
                return true;
        }

        return false;
    }

    private List<Vector3Int> GetFiledGroundTiles(Vector3Int position)
    {
        List<Vector3Int> result = new List<Vector3Int> { };
        List<Vector3Int> list = GetDirectionList(position);

        foreach (var pos in list)
        {
            if (GetMatrixValue(pos) != 0) result.Add(pos);
        }

        return result;
    }
    
    public List<Vector3Int> GetEmptyGroundTiles(Vector3Int position)
    {
        List<Vector3Int> result = new List<Vector3Int> { };
        List<Vector3Int> list = GetDirectionList(position);

        foreach (var pos in list)
        {
            if (GetMatrixValue(pos) == 0) result.Add(pos);
        }

        return result;
    }

    public List<Vector3Int> GetDirectionList(Vector3Int position)
    {
        return new List<Vector3Int>
        {
            new Vector3Int(position.x, position.y + 1, 0), // Up
            new Vector3Int(position.x + 1, position.y, 0), // Right
            new Vector3Int(position.x, position.y - 1, 0), // Down
            new Vector3Int(position.x - 1, position.y, 0) // Left
        };
    }

    private void SetupMapObject(int count, TileInfo tile, int offset)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3Int position = new Vector3Int(Random.Range(offset + startRoomSize.x, mapSize.x - offset - 1),
                Random.Range(offset + startRoomSize.y, mapSize.y - offset - 1), 0);
            UpdateTile(position, tile);
        }
    }
}

[Serializable]
public class TileInfo
{
    public Tile tile;
    public int index;
    public float health;
    public Color color;
}