using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBase : MonoBehaviour, IHpBar
{
    [Header("Base settings")]
    [SerializeField] private HpBar hpBar;
    [SerializeField] private GameObject enemyPrefab;

    [HideInInspector] private Ground ground;

    private void Start()
    {
        ground = FindObjectOfType<Ground>();
        hpBar.maxValue = ground.tilesList[3].health;
    }

    public void SpawnEnemy()
    {
        List<Vector3Int> list = ground.GetEmptyGroundTiles(ground.tilemap.WorldToCell(transform.position));
        Vector2 position = ground.tilemap.CellToWorld(list[Random.Range(0, list.Count)]);
        Instantiate(enemyPrefab, position, Quaternion.identity);
    }
    
    public bool Destroy()
    {
        Destroy(gameObject);
        return true;
    }
    
    public void UpdateHpBar(float hp)
    {
        hpBar.UpdateBar(hp);
    }
}
