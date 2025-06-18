using UnityEngine;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    public int health = 1000;

    CharacterController controller;
    public LayerMask groundMask;
    Transform groundCheck;
    EnemyHandler enemyHandler;

    public GameObject player;

    float ySpeed;
    public float gravity = -10f;
    public float speed = 2f;
    bool isGrounded = false;

    Vector3 targetPos = Vector3.zero;

    List<Vector2Int> sides = new();
    List<Vector2Int> corners = new();

    Dictionary<Vector2Int, int> map = new();
    Vector2Int mapTopRight;
    Vector2Int mapBottomLeft;

    public bool findPath;
    bool currentState;
    List<Vector2Int> path = new();

    public bool spawnSpheres;
    public GameObject sphereObject;
    public Transform sphereParent;
    List<GameObject> spheres = new();

    void Start()
    {
        groundCheck = transform.GetChild(0);
        enemyHandler = GetComponentInParent<EnemyHandler>();
        controller = GetComponent<CharacterController>();

        sides.Add(Vector2Int.up);
        sides.Add(Vector2Int.right);
        sides.Add(Vector2Int.down);
        sides.Add(Vector2Int.left);

        corners.Add(new Vector2Int(1, 1));
        corners.Add(new Vector2Int(-1, 1));
        corners.Add(new Vector2Int(1, -1));
        corners.Add(new Vector2Int(-1, -1));

        mapTopRight = enemyHandler.mapTopRight;
        mapBottomLeft = enemyHandler.mapBottomLeft;
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, groundMask);

        if (isGrounded && ySpeed < 0)
        {
            ySpeed = -2;
        }

        ySpeed += gravity * Time.deltaTime;

        controller.Move(new Vector3(0, ySpeed, 0) * Time.deltaTime);

        if (currentState != findPath)
        {
            if (spheres.Count > 0 && spawnSpheres)
            {
                foreach (GameObject sphere in spheres)
                {
                    Destroy(sphere);
                }
                spheres.Clear();
            }

            path = AStar(ConvertPos(transform.position), ConvertPos(player.transform.position));
            foreach (Vector2Int tile in path)
            {
                Vector3 pos = new(tile.x, map[tile], tile.y);
                if (spawnSpheres) spheres.Add(Instantiate(sphereObject, pos, Quaternion.identity, sphereParent));
            }
            targetPos = new Vector3(path[^1].x, transform.position.y, path[^1].y);

            currentState = findPath;
        }

        targetPos.y = transform.position.y;
        Vector3 diffTargetPos = targetPos - transform.position;
        if (diffTargetPos.magnitude > 0.2f)
        {
            controller.Move(speed * Time.deltaTime * diffTargetPos.normalized);
        }
        else if(path.Count > 1)
        {
            path.RemoveAt(path.Count - 1);
            if (spawnSpheres)
            {
                Destroy(spheres[path.Count]);
                spheres.RemoveAt(path.Count);
            }
            targetPos = new Vector3(path[^1].x, transform.position.y, path[^1].y);
        }
        else
        {
            if (spheres.Count > 0 && spawnSpheres)
            {
                foreach (GameObject sphere in spheres)
                {
                    Destroy(sphere);
                }
                spheres.Clear();
            }

            path = AStar(ConvertPos(transform.position), ConvertPos(player.transform.position));
            if (path.Count > 0)
            {
                foreach (Vector2Int tile in path)
                {
                    Vector3 pos = new(tile.x, map[tile], tile.y);
                    if (spawnSpheres) spheres.Add(Instantiate(sphereObject, pos, Quaternion.identity, sphereParent));
                }
                targetPos = new Vector3(path[^1].x, transform.position.y, path[^1].y);
            }
            else
            {
                Vector2 playerPos = ConvertPos(player.transform.position);
                targetPos = new Vector3(playerPos.x, transform.position.y, playerPos.y);
            }
        }
        
    }

    List<Vector2Int> AStar(Vector2Int pos, Vector2Int target)
    {
        map = enemyHandler.map;
        List<Vector2Int> openSet = new();
        List<Vector2Int> closedSet = new();
        openSet.Add(pos);

        Dictionary<Vector2Int, int> fScores = new();
        Dictionary<Vector2Int, int> gScores = new();
        gScores.Add(pos, 0);
        fScores.Add(pos, HCost(pos, target));

        Dictionary<Vector2Int, Vector2Int> cameFrom = new();

        while (openSet.Count > 0)
        {
            Vector2Int bestTile = Vector2Int.zero;
            int bestScore = int.MaxValue;
            foreach (Vector2Int tile in openSet)
            {
                if (fScores[tile] < bestScore)
                {
                    bestTile = tile;
                    bestScore = fScores[tile];
                }
            }

            if (bestTile == target)
            {
                List<Vector2Int> path = new();

                Vector2Int current = bestTile;
                path.Add(current);
                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Add(current);
                }

                return path;
            }

            openSet.Remove(bestTile);
            closedSet.Add(bestTile);
            List<Vector2Int> neighbors = ValidNeighbors(bestTile);
            foreach (Vector2Int neighbor in neighbors)
            {
                if (!openSet.Contains(neighbor) && !closedSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                    int distance = HCost(bestTile, neighbor);
                    gScores.Add(neighbor, gScores[bestTile] + distance);
                    fScores.Add(neighbor, gScores[bestTile] + distance + HCost(neighbor, target));
                    cameFrom.Add(neighbor, bestTile);
                }
            }
        }

        print("no path found");
        return new();
    }

    int HCost(Vector2Int pos, Vector2Int target)
    {
        Vector2Int diff = new Vector2Int(Mathf.Abs(pos.x - target.x), Mathf.Abs(pos.x - target.x));

        if (diff.x > diff.y) return (diff.x - diff.y) * 10 + diff.y * 14;
        else return (diff.y - diff.x) * 10 + diff.x * 14;
    }

    List<Vector2Int> ValidNeighbors(Vector2Int tile)
    {
        List<Vector2Int> neighbors = new();

        foreach (Vector2Int side in sides)
        {
            if (IsValid(tile, tile + side)) neighbors.Add(tile + side);
        }

        foreach (Vector2Int corner in corners)
        {
            if (IsValid(tile, tile + corner) && map[tile + new Vector2Int(corner.x, 0)] - map[tile] <= 1 && map[tile + new Vector2Int(0, corner.y)] - map[tile] <= 1) neighbors.Add(tile + corner);
        }

        return neighbors;
    }

    bool IsInMap(Vector2Int pos)
    {
        return pos.x <= mapTopRight.x && pos.y <= mapTopRight.y && pos.x >= mapBottomLeft.x && pos.y >= mapBottomLeft.y;
    }

    bool IsValid(Vector2Int pos, Vector2Int tile)
    {
        return IsInMap(tile) && map[tile] - map[pos] <= 1;
    }

    Vector2Int ConvertPos(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }
}
