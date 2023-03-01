using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Pathfinding : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private Text text;
    [SerializeField] public Tilemap tilemap;
    [SerializeField] private bool debug;

    [HideInInspector] public bool[,] binaryMap;
    
    private void Start()
    {
        text.enabled = debug;
    }

    public List<Vector3> FindPath(Vector2Int start, Vector2Int end)
    {
        List<Vector3> finalList = new List<Vector3>();
        List<Vector2Int> exploredList = new List<Vector2Int>();
        Point[,] points = CreatePointsMap();

        for (int x = 0; x < binaryMap.GetLength(0); x++)
        {
            for (int y = 0; y < binaryMap.GetLength(1); y++)
            {
                points[x, y].SetCost(Vector2Int.zero, -1);
            }
        }

        CheckNeighbors(start);

        if (points[end.x, end.y].previousPoint != Vector2Int.zero)
        {
            Vector2Int currentPoint = end;
            while (currentPoint != start)
            {
                finalList.Add(tilemap.CellToWorld((Vector3Int) currentPoint));
                currentPoint = points[currentPoint.x, currentPoint.y].previousPoint;
            }

            finalList.Reverse();
        }

        if (debug)
        {
            text.text = "";
            for (int x = 0; x < binaryMap.GetLength(0); x++)
            {
                for (int y = 0; y < binaryMap.GetLength(1); y++)
                {
                    if (new Vector2(y, x) == start || new Vector2(y, x) == end)
                    {
                        text.text += "[*]";
                    }
                    else
                        text.text += points[y, x].cost == 0 ? "[#]" : $"[{points[y, x].cost}]";
                }

                text.text += "\n";
            }
        }

        text.text = String.Join("\n", text.text.Split("\n").Reverse());

        return finalList;

        void CheckNeighbors(Vector2Int startPoint)
        {
            exploredList.Add(startPoint);
            if (startPoint == end) return;

            Vector2Int[] neighborPoints =
            {
                new(startPoint.x + 1, startPoint.y),
                new(startPoint.x - 1, startPoint.y),
                new(startPoint.x, startPoint.y + 1),
                new(startPoint.x, startPoint.y - 1)
            };

            foreach (Vector2Int neighborPoint in neighborPoints)
            {
                if (neighborPoint.x < binaryMap.GetLength(0) &&
                    neighborPoint.y < binaryMap.GetLength(1) &&
                    neighborPoint.x >= 0 &&
                    neighborPoint.y >= 0 &&
                    binaryMap[neighborPoint.x, neighborPoint.y])
                {
                    if (!exploredList.Contains(new Vector2Int(neighborPoint.x, neighborPoint.y)))
                    {
                        points[neighborPoint.x, neighborPoint.y]
                            .SetCost(startPoint, points[startPoint.x, startPoint.y].cost);
                        CheckNeighbors(new Vector2Int(neighborPoint.x, neighborPoint.y));
                    }
                    else
                    {
                        if (points[neighborPoint.x, neighborPoint.y]
                            .UpdateCost(startPoint, points[startPoint.x, startPoint.y].cost))
                            CheckNeighbors(new Vector2Int(neighborPoint.x, neighborPoint.y));
                    }
                }
            }
        }
    }

    private Point[,] CreatePointsMap()
    {
        Point[,] points = new Point[binaryMap.GetLength(0), binaryMap.GetLength(1)];
        for (int x = 0; x < binaryMap.GetLength(0); x++)
        {
            for (int y = 0; y < binaryMap.GetLength(1); y++)
            {
                points[x, y] = new Point();
            }
        }

        return points;
    }
}

[Serializable]
public class Point
{
    public Vector2Int previousPoint;
    public int cost = -1;

    public bool UpdateCost(Vector2Int point, int value)
    {
        value++;
        if (value < cost)
        {
            cost = value;
            previousPoint = point;
            return true;
        }

        return false;
    }

    public void SetCost(Vector2Int point, int value)
    {
        cost = ++value;
        previousPoint = point;
    }
}