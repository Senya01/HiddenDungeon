using System;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] public Level[] levels;
    [SerializeField] public Ground ground;
    [SerializeField] public Timer timer;

    private void Awake()
    {
        /*
         * groundTile - 0
         * enemyBaseTile - 1
         * baseTile - 2
         * baseFragmentTile - 3
         */
        int level = CurrentLevel.currentLevel;
        ground.mapSize = levels[level].mapSize;
        ground.enemyBasesCount = levels[level].enemyBasesCount;
        ground.tilesList[1] = levels[level].tilesInfo[0];
        ground.tilesList[3] = levels[level].tilesInfo[1];
        ground.tilesList[4] = levels[level].tilesInfo[2];
        ground.tilesList[5] = levels[level].tilesInfo[3];
        ground.enemyBaseOffset = levels[level].enemyBaseOffset;
        ground.basesFragmentOffset = levels[level].basesFragmentOffset;
        timer.time = levels[level].time;
    }

    /*
     * Размер карты
     * Кол-во вражеских баз
     * Время на ломание базы и фрагмента
     * Время до следующей волны
     */
}

[Serializable]
public class Level
{
    public Vector2Int mapSize;
    public int enemyBasesCount;
    public TileInfo[] tilesInfo;
    public int enemyBaseOffset;
    public int basesFragmentOffset;
    public int time;
}