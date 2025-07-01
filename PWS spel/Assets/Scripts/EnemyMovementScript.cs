using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.HID;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.Experimental.GraphView;
using static UnityEditor.PlayerSettings;
using Unity.Properties;
using NUnit;

public class EnemyMovementScript : MonoBehaviour
{
    CharacterController controller;
    public LayerMask groundMask;
    Transform groundCheck;
    EnemyHandler enemyHandler;

    public GameObject player;
    public LayerMask playerMask;

    float ySpeed;
    public float gravity = -10f;
    public float speed = 2f;
    bool isGrounded = false;

    Vector3 targetPos = Vector3.zero;



    public GameObject map;
    public GameObject nodes;
    


    public int ammo;
    float lastShot;
    public float reloadStart;



    List<Vector2> path = new();

    void Start()
    {
        ammo = GetComponentInChildren<MagazineScript>().cap;


        groundCheck = transform.GetChild(0);
        enemyHandler = GetComponentInParent<EnemyHandler>();
        controller = GetComponent<CharacterController>();


    }

    void Update()
    {



        if (HasLineOfSight() && ammo > 0 && Time.time > lastShot + GetComponentInChildren<MagazineScript>().ShotCooldown)
        {
            lastShot = Time.time;
           // Debug.Log(HasLineOfSight());
            player.GetComponent<PlayerHealth>().Hit(1);
            ammo -= 1;
        }

        if (ammo == 0 && reloadStart == -1) 
        {
            reloadStart = Time.time; 
        }
        if (Time.time > reloadStart + GetComponentInChildren<MagazineScript>().ReloadTime && reloadStart != -1)
        {
            reloadStart = -1;
            ammo = GetComponentInChildren<MagazineScript>().cap;
        }


        

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.4f, groundMask);


        if (isGrounded && ySpeed < 0)
        {
            ySpeed = -2;
        }

        ySpeed += gravity * Time.deltaTime;

        controller.Move(new Vector3(0, ySpeed, 0) * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.T))
        {
            path = NearestCover();

            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 start = new Vector3(path[i].x, transform.position.y, path[i].y);
                Vector3 end = new Vector3(path[i + 1].x, transform.position.y, path[i + 1].y);
                Debug.DrawLine(start, end, Color.green, 100f);
            }


            if (path.Count > 0)
            {
                targetPos = new Vector3(path[^1].x, transform.position.y, path[^1].y);
            }
            else
            {
                targetPos = transform.position;
            }
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            path = AStar(new Vector2(transform.position.x, transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z));

            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 start = new Vector3(path[i].x, transform.position.y, path[i].y);
                Vector3 end = new Vector3(path[i + 1].x, transform.position.y, path[i + 1].y);
                Debug.DrawLine(start, end, Color.green, 100f);
            }


            if (path.Count > 0)
            {
                targetPos = new Vector3(path[^1].x, transform.position.y, path[^1].y);
            }
            else
            {
                targetPos = transform.position;
            }
        }


        targetPos.y = transform.position.y;
        Vector3 diffTargetPos = targetPos - transform.position;
        if (diffTargetPos.magnitude > 0.05f)
        {
            controller.Move(speed * Time.deltaTime * diffTargetPos.normalized);
        }
        else if (path.Count > 1)
        {
            path.RemoveAt(path.Count - 1);
            targetPos = new Vector3(path[^1].x, transform.position.y, path[^1].y);
        }
        else
        {
            /*path = AStar(ConvertPos(transform.position), ConvertPos(player.transform.position));
            if (path.Count > 0)
            {
                targetPos = new Vector3(path[^1].x, transform.position.y, path[^1].y);
            }
            else
            {
                Vector2 playerPos = ConvertPos(player.transform.position);
                targetPos = new Vector3(playerPos.x, transform.position.y, playerPos.y);
            }*/
        }

    }  

    public bool HasLineOfSight()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        if (Vector3.Angle(dir, transform.forward) < 40f)
        {
            //Debug.DrawRay(transform.position, dir * 100, Color.red, 2f);

            return !Physics.Raycast(transform.position, dir, (player.transform.position - transform.position).magnitude, groundMask);
        }
        else return false;
    }





    List<Vector2> NodesReachable(Vector2 pos)
    {

        Transform[] children = nodes.GetComponentsInChildren<Transform>();
        List<Vector2> ReachableNodes = new();


        foreach (Transform child in children)
        {
            Vector3 dir = new Vector3(pos.x, child.position.y,pos.y) - child.position;


            if (!Physics.SphereCast(child.position, 0.4f, dir.normalized, out _, dir.magnitude, groundMask))
          //  if (!Physics.Raycast(child.position, dir.normalized, dir.magnitude, groundMask))
            {
                ReachableNodes.Add(new Vector2(child.position.x, child.position.z));
            }
        }

        return ReachableNodes;
    }


    List<Vector2> NearestCover()
    {
        Transform[] children = map.GetComponentsInChildren<Transform>();

        List<Vector2> NearestCover = new();
        float NearestCoverDistance = float.PositiveInfinity;

        foreach (Transform child in children)
        {

            Vector3 dir = transform.position - player.transform.position;

            RaycastHit hit;
            Physics.Raycast(child.position, dir.normalized, out hit, 100f, groundMask);

            Physics.Raycast(hit.point, -dir.normalized, out hit, 100f, groundMask);



            Vector3 coverPos = hit.point + hit.normal * 0.5f;

     
         //   Debug.DrawRay(coverPos, Vector3.up * 100, Color.red, 10f);
            Debug.DrawRay(child.position, Vector3.up * 100, Color.blue, 10f);
           // Debug.DrawRay(child.position, hit.normal*100f, Color.green, 10f);
            Debug.DrawRay(hit.point, Vector3.up * 100, Color.green, 10f);
            Debug.DrawLine(child.position, coverPos, Color.blue, 10f);
            if (!Physics.CheckSphere(coverPos, 0.4f, groundMask))
            {

                Vector2 coverPos2 = new Vector2(coverPos.x, coverPos.z);

                List<Vector2> path = AStar(new Vector2(transform.position.x, transform.position.z), coverPos2);


                if (path.Count > 0)
                {
                    float distance = 0;


                    for (int i = 0; i < path.Count - 1; i++)
                    {

                        distance += HCost(path[i], path[i + 1]);

                    }

                    if (distance < NearestCoverDistance)
                    {
                        NearestCoverDistance = distance;
                        NearestCover = path;


                    }
                }
            }
            

        }

        return NearestCover;


    }





    bool Cast(Vector2 start, Vector2 end)
    {
        Vector3 start3 = new Vector3(start.x, 0.5f, start.y);
        Vector3 end3 = new Vector3(end.x, 0.5f, end.y);

        return Physics.SphereCast(start3, 0.4f, (end3 - start3).normalized, out _, (end3 - start3).magnitude, groundMask);

    }



    List<Vector2> AStar(Vector2 pos, Vector2 target)
    {
        List<Vector2> openSet = new();
        List<Vector2> closedSet = new();
        openSet.Add(pos);

        Dictionary<Vector2, float> fScores = new();
        Dictionary<Vector2, float> gScores = new();
        gScores.Add(pos, 0);
        fScores.Add(pos, HCost(pos, target));

        Dictionary<Vector2, Vector2> cameFrom = new();

        while (openSet.Count > 0)
        {
            Vector2 bestNode = Vector2.zero;
            float bestScore = float.MaxValue;
            foreach (Vector2 node in openSet)
            {
                if (fScores[node] < bestScore)
                {
                    bestNode = node;
                    bestScore = fScores[node];
                }
            }

            if (!Cast(bestNode, target))
            {
                List<Vector2> path = new();

                Vector2 current = bestNode;
                path.Add(target);
                path.Add(current);
                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Add(current);
                }
                

                return path;
            }

            openSet.Remove(bestNode);
            closedSet.Add(bestNode);
            List<Vector2> neighbors = NodesReachable(bestNode);
            foreach (Vector2 neighbor in neighbors)
            {
                if (!openSet.Contains(neighbor) && !closedSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                    float distance = HCost(bestNode, neighbor);
                    gScores.Add(neighbor, gScores[bestNode] + distance);
                    fScores.Add(neighbor, gScores[bestNode] + distance + HCost(neighbor, target));
                    cameFrom.Add(neighbor, bestNode);
                }
            }
        }

       // print("no path found");
       // print(target);
        return new();
    }

    float HCost(Vector2 pos, Vector2 target)
    {
        return Vector2.Distance(pos, target);


    }


}


