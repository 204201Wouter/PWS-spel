using UnityEngine;
using System.Collections.Generic;

public class EnemyHandler : MonoBehaviour
{
    public GameObject enemy;
    public Dictionary<Vector2, int> map = new();
    public LayerMask groundMask;

    public Vector2Int mapSize;
    void Start()
    {
        int maxX = mapSize.x / 2;
        int maxY = mapSize.y / 2;
        for (int i = -maxX; i < maxX; i++)
        {
            for (int j = -maxY; j < maxY; j++)
            {
                Vector3Int pos = new Vector3Int(i, 5, j);
                while (!Physics.CheckBox(pos, new Vector3(0.49f, 0.49f, 0.49f), Quaternion.identity, groundMask))
                {
                    pos += Vector3Int.down;
                }
                map.Add(new Vector2(i, j), pos.y + 1);
            }
        }
    }
}
