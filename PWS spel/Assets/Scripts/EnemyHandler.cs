using UnityEngine;
using System.Collections.Generic;

public class EnemyHandler : MonoBehaviour
{
    public GameObject enemy;
    public Dictionary<Vector2Int, int> map = new();
    public LayerMask groundMask;

    public Vector2Int mapTopRight;
    public Vector2Int mapBottomLeft;
    void Start()
    {
        for (int i = mapBottomLeft.x; i <= mapTopRight.x; i++)
        {
            for (int j = mapBottomLeft.x; j <= mapTopRight.y; j++)
            {
                Vector3Int pos = new Vector3Int(i, 5, j);
                while (!Physics.CheckBox(pos, new Vector3(0.49f, 0.49f, 0.49f), Quaternion.identity, groundMask))
                {
                    pos += Vector3Int.down;
                }
                map.Add(new Vector2Int(i, j), pos.y + 1);
            }
        }
    }
}
