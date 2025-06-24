using UnityEngine;
using System.Collections.Generic;

public class EnemyHandler : MonoBehaviour
{
    public Dictionary<Vector2Int, int> map = new();
    public Dictionary<int, List<Vector2Int>> inverseMap = new();
    public int mapMaxHeight;

    public EnemyMovementScript[] enemies;

    public LayerMask groundMask;

    public Vector2Int mapTopRight;
    public Vector2Int mapBottomLeft;
    void Start()
    {
        enemies = GetComponentsInChildren<EnemyMovementScript>();

        for (int i = 0; i <= mapMaxHeight; i++)
        {
            inverseMap.Add(i, new List<Vector2Int>());
        }

        for (int i = mapBottomLeft.x; i <= mapTopRight.x; i++)
        {
            for (int j = mapBottomLeft.x; j <= mapTopRight.y; j++)
            {
                Vector3Int pos = new(i, mapMaxHeight, j);
                while (!Physics.CheckBox(pos, new Vector3(0.49f, 0.49f, 0.49f), Quaternion.identity, groundMask))
                {
                    pos += Vector3Int.down;
                }

                map.Add(new Vector2Int(i, j), pos.y);
                inverseMap[pos.y].Add(new Vector2Int(i, j));
            }
        }

        foreach (EnemyMovementScript enemy in enemies)
        {
            enemy.map = map;
            enemy.inverseMap = inverseMap;
            enemy.mapMaxHeight = mapMaxHeight;
        }
    }
}
