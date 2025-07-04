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

using UnityEngine.InputSystem.EnhancedTouch;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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
    public GameObject cover;

    public string mode;

    


    public int ammo;
    float lastShot;
    public float reloadStart;
    private bool lastisGrounded;



    List <Vector2> path = new();

    void Start()
    {

        
        ammo = GetComponentInChildren<MagazineScript>().cap;


        groundCheck = transform.GetChild(0);
        enemyHandler = GetComponentInParent<EnemyHandler>();
        controller = GetComponent<CharacterController>();
        targetPos = transform.position;

        //player.GetComponent<Movement>().velocity.magnitude;



    }

    void Update()
    {

        Movement playerMovement = player.GetComponent<Movement>();

        

        float soundRadius = 0;

        if (playerMovement.velocity.magnitude >= 8 && playerMovement.isGrounded)
        {
            soundRadius = 50;
        }
        else if (!lastisGrounded && playerMovement.isGrounded)
        {
            soundRadius = 40;
        }
        else if (playerMovement.velocity.magnitude >= 4 && playerMovement.isGrounded)
        {
            soundRadius = 20;
        }
        else if (playerMovement.velocity.magnitude >= 10 && playerMovement.isGrounded) //crouchspeed)
        {
            soundRadius = 0;
        }

        // shoot
        // soundRadius = 100;

        lastisGrounded = playerMovement.isGrounded;

      //  soundRadius = 0;


        if ((player.transform.position - transform.position).magnitude < soundRadius)
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




        if (HasLineOfSight())
        {
            if (ammo > 0 && Time.time > lastShot + GetComponentInChildren<MagazineScript>().ShotCooldown)
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
            path = NearestCover((player.transform.position-transform.position).magnitude);

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
            Quaternion targetRotation = Quaternion.LookRotation(diffTargetPos);

            transform.rotation = Quaternion.RotateTowards(transform.rotation,targetRotation,360f * Time.deltaTime);
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
        Vector3 dir = player.transform.position - transform.position;
        if (Vector3.Angle(dir.normalized, transform.forward) < 40f)
        {
            //Debug.DrawRay(transform.position, dir * 100, Color.red, 2f);

            return Person2PersonCast(transform.position+Vector3.up*0.5f, player.transform.position);
        }
        else return false;
    }

    public bool Person2PersonCast(Vector3 pos, Vector3 pos2, float height = 1f)
    {
        Vector3 rpos;
        Vector3 dir;
        Vector3 left = -Vector3.Cross(pos2 - pos, Vector3.up).normalized * 0.4f;
        Vector3 right = Vector3.Cross(pos2 - pos, Vector3.up).normalized * 0.4f;

        rpos = pos2 + Vector3.up* height + left;
        dir = rpos - pos;
        if (!Physics.Raycast(pos, dir.normalized, dir.magnitude, groundMask)) { return true; }
        Debug.DrawRay(pos, dir, Color.blue);
        rpos = pos2 + Vector3.down * height + left;
        dir = rpos - pos;
        if (!Physics.Raycast(pos, dir.normalized, dir.magnitude, groundMask)) { return true; }
        Debug.DrawRay(pos, dir, Color.blue);
        rpos = pos2 + Vector3.up * height + right;
        dir = rpos - pos;
        if (!Physics.Raycast(pos, dir.normalized, dir.magnitude, groundMask)) { return true; }
        Debug.DrawRay(pos, dir, Color.blue);
        rpos = pos2 + Vector3.down * height + right;
        dir = rpos - pos;
        if (!Physics.Raycast(pos, dir.normalized, dir.magnitude, groundMask)) {return true; }
        Debug.DrawRay(pos, dir, Color.blue);

        return false;
        // return !Physics.Raycast(pos, dir.normalized, dir.magnitude, groundMask);

    }





    List<Vector2> NodesReachable(Vector2 pos)
    {

        Transform[] children = nodes.GetComponentsInChildren<Transform>();
        List<Vector2> ReachableNodes = new();


        for (int i = 1; i < children.Length; i++)
        {
            Transform child = children[i];
            Vector3 dir = new Vector3(pos.x, child.position.y,pos.y) - child.position;


            if (!Physics.SphereCast(child.position, 0.4f, dir.normalized, out _, dir.magnitude, groundMask))
          //  if (!Physics.Raycast(child.position, dir.normalized, dir.magnitude, groundMask))
            {
                ReachableNodes.Add(new Vector2(child.position.x, child.position.z));
            }
        }

        return ReachableNodes;
    }


    List<Vector2> NearestCover(float distanceFromPlayer = 0f)
    {
        Transform[] children = nodes.GetComponentsInChildren<Transform>();

        List<Vector2> NearestCover = new();
        float NearestCoverDistance = float.PositiveInfinity;

        for (int e = 1; e < children.Length; e++)
        {
            Transform child = children[e];

            List<Vector2> childPosList = new();
            childPosList.Add(new Vector2(child.position.x-2f, child.position.z));
            childPosList.Add(new Vector2(child.position.x + 2f, child.position.z));
            childPosList.Add(new Vector2(child.position.x, child.position.z - 2f));
            childPosList.Add(new Vector2(child.position.x, child.position.z + 2f));


            foreach (Vector2 childPos in childPosList)
            {
                Debug.DrawLine(player.transform.position + Vector3.up * 0.5f, new Vector3(childPos.x, 1f, childPos.y), Color.red,100f);
                Debug.DrawRay(new Vector3(childPos.x, 1f, childPos.y), Vector3.up*100f, Color.green, 100f);
                if (!Physics.CheckSphere(new Vector3(childPos.x, 1f, childPos.y), 0.4f, groundMask) &&
                    !Person2PersonCast(player.transform.position+Vector3.up*0.5f, new Vector3(childPos.x, 1f, childPos.y), 0.5f) &&
                   // Physics.Raycast(player.transform.position, (new Vector3(childPos.x, 0.5f, childPos.y) - player.transform.position).normalized, (new Vector3(childPos.x, 0.5f, childPos.y) - player.transform.position).magnitude, groundMask) &&
                    !Cast(transform.position, childPos))
                {

                    /*
                    

                   // Vector2 coverPos = new Vector2(child.position.x, child.position.z);

                    List<Vector2> path = AStar(new Vector2(transform.position.x, transform.position.z), childPos);


                    if (path.Count > 0)
                    {
                        float distance = 0;


                        for (int i = 0; i < path.Count - 1; i++)
                        {

                            distance += HCost(path[i], path[i + 1]);

                        }

                        if (Math.Abs(distance - distanceFromPlayer) < NearestCoverDistance)
                        {
                            NearestCoverDistance = distance;
                            NearestCover = path;


                        }
                    */
                    List<Vector2> path = new();
                    path = AStar(new Vector2(transform.position.x, transform.position.z), childPos);
                    float distance = (childPos - new Vector2(transform.position.x, transform.position.z)).magnitude;

                    if (Math.Abs(distance - distanceFromPlayer) < NearestCoverDistance)
                    {
                        NearestCoverDistance = distance;
                        NearestCover = path;


                    }
                
                }
            }
            

        }

      //  foreach (Vector2 pos in NearestCover)
      //  Debug.Log(pos);

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


