using UnityEngine;
using System.Collections.Generic;
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
using System.ComponentModel;
using System.IO;

public class EnemyMovementScript : MonoBehaviour
{
    public CharacterController controller;
    public LayerMask groundMask;
    EnemyHandler enemyHandler;

    public GameObject player;
    public LayerMask playerMask;

    float ySpeed;
    public float gravity = -10f;
    public float speed = 2f;
    bool isGrounded = false;

    Vector3 targetPos = Vector3.zero;

    public Animator animator;

    public GameObject map;
    public GameObject nodes;
    public GameObject cover;

    public string mode = "guard";

    public int ammo;
    float lastShot;
    public float reloadStart;
    private bool lastisGrounded;
    private Vector3 lastPlayerPos;
    private float lastHearPlayer;
    float aimtimedone;
    bool aiming = false;

    Vector3 lateralVelocity;
    Vector3 lateralAcceleration;
    Vector3 lastPos;
    Vector3 lastLateralVelocity;


    List <Vector2> path = new();

    void Start()
    {
        ammo = GetComponentInChildren<MagazineScript>().cap;

        enemyHandler = GetComponentInParent<EnemyHandler>();
        controller = GetComponent<CharacterController>();
        targetPos = transform.position;

        lastPlayerPos = player.transform.position;
    }

    void Update()
    {
        // schieten
        if (HasLineOfSight())
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
            lastHearPlayer = Time.time;
            if (!aiming)
            {
                aimtimedone = Time.time + AimTimeFormula();
                aiming = true;
                print("aimtime enemy was: " + AimTimeFormula().ToString());
            }

            if (ammo > 0 && Time.time > lastShot + GetComponentInChildren<MagazineScript>().shotCooldown && Time.time > aimtimedone)
            {
                lastShot = Time.time;
                ammo -= 1;
                animator.SetTrigger("recoil");
                if (Random.value < AccuracyFormula())
                {
                    player.GetComponent<PlayerHealth>().Hit(1);
                    print("enemy hit");
                }
                else print("enemy missed");
            }

            if (ammo == 0 && reloadStart == -1)
            {
                reloadStart = Time.time;
                animator.SetTrigger("reload");
            }
            if (Time.time > reloadStart + GetComponentInChildren<MagazineScript>().reloadTime && reloadStart != -1)
            {
                reloadStart = -1;
                ammo = GetComponentInChildren<MagazineScript>().cap;
            }
            animator.SetBool("IsAiming", true);
        }
        else
        {
            aiming = false;
            animator.SetBool("IsAiming", false);
        }


        // cover
        if (HasLineOfSight() && mode == "scout")     
        {
            mode = "cover";
            lastPlayerPos = player.transform.position + Vector3.up * 2;
        }

        // zoek player
        else if (HearPlayer())
        {
            if (mode == "guard")
            { 
                mode = "scout";
                lastPlayerPos = player.transform.position + Vector3.up * 2;
            }

            lastHearPlayer = Time.time;
          //  print(gameObject.name + " heard player");
        }

        // 
        else if (mode == "cover" && Time.time > lastHearPlayer + 5f)
        {
            mode = "scout";
          //  Vector3 scale = transform.localScale;
          //  scale.y = 1f;
        //    transform.localScale = scale;
            lastPlayerPos = player.transform.position + Vector3.up * 2;
        }

        else if (mode == "scout" && (targetPos - transform.position).magnitude <= 0.05f && path.Count <= 1)
        {
            mode = "guard";
        }


        /*
        else if (mode == "cover" && (targetPos - transform.position).magnitude <= 0.05f && path.Count <= 1)
        {
            if (Person2PersonCast(player.transform.position, transform.position))
            {
            //    Vector3 scale = transform.localScale;
              //  scale.y = 0.5f;
                //transform.localScale = scale;
            }
            Debug.Log(transform.position - targetPos);
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - targetPos);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
        }
        */


        isGrounded = Physics.CheckSphere(transform.position-Vector3.up*0.7f, 0.4f, groundMask);

        //Debug.Log(isGrounded);

        if (isGrounded && ySpeed < 0)
        {
            ySpeed = -2;
        }

        ySpeed += gravity * Time.deltaTime;

        controller.Move(new Vector3(0, ySpeed, 0) * Time.deltaTime);

        if (mode == "cover" || mode == "scout")
        {
            MoveEnemy(mode);
        }
        else if (mode == "guard")
        {
            path = new();
            targetPos = transform.position;
        }

        targetPos.y = transform.position.y;
        Vector3 diffTargetPos = targetPos - transform.position;
        if (diffTargetPos.magnitude > 0.05f)
        {

            Quaternion targetRotation;

            if (mode == "cover")
            {
                targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);

            }

            else
            {
                targetRotation = Quaternion.LookRotation(diffTargetPos);
                
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
            }

            animator.SetFloat("speed", speed);
            controller.Move(speed * Time.deltaTime * diffTargetPos.normalized);
        }

        else if (path.Count > 1)
        {
            path.RemoveAt(path.Count - 1);
            targetPos = new Vector3(path[^1].x, transform.position.y, path[^1].y);
        }

        else
        {
            animator.SetFloat("speed", 0f);
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

        Vector3 relativeVelocity = (transform.position - lastPos) / Time.deltaTime - player.GetComponent<Movement>().velocity;
        Vector3 directionToPlayer = (transform.position - player.transform.position).normalized;

        lateralVelocity = relativeVelocity - Vector3.Dot(relativeVelocity, directionToPlayer) * directionToPlayer;
        lateralAcceleration = (lateralVelocity - lastLateralVelocity) / Time.deltaTime;

        lastLateralVelocity = lateralVelocity;
        lastPos = transform.position;
    }

    void MoveEnemy(string mode)
    {
        if ((player.transform.position - lastPlayerPos).magnitude > 1f)
        {
            if (mode == "cover")
            path = NearestCover((player.transform.position - transform.position).magnitude);
            if (mode == "scout")
            path = AStarTarget(new Vector2(transform.position.x, transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z));

            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 start = new Vector3(path[i].x, transform.position.y, path[i].y);
                Vector3 end = new Vector3(path[i + 1].x, transform.position.y, path[i + 1].y);
                Debug.DrawLine(start, end, Color.green, 100f);
            }

            lastPlayerPos = player.transform.position;


            if (path.Count > 0)
            {
                targetPos = new Vector3(path[^1].x, transform.position.y, path[^1].y);
            }
            else
            {
                targetPos = transform.position;
            }
        }
    }


    bool HearPlayer()
    {

        Movement playerMovement = player.GetComponent<Movement>();

        /*float soundRadius = 0;

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
        else if (playerMovement.velocity.magnitude >= 10 && playerMovement.isGrounded) //crouchspeed
        {
            soundRadius = 0;
        }

        lastisGrounded = playerMovement.isGrounded;*/

        return (player.transform.position - transform.position).magnitude < playerMovement.soundRadius;
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

    float AccuracyFormula()
    {
        float distance = (player.transform.position - transform.position).magnitude;
        float weaponaccuracy = 1f; // deze is misschien handig om makkelijk elk wapen andere accuracy te laten hebben bij enemy
        int acceleration = 0;
        if (lateralAcceleration.magnitude > 10) acceleration = 1;

        return Mathf.Clamp(0.97f - 0.07f*Mathf.Sqrt(distance) - 0.04f*lateralVelocity.magnitude - 0.1f*acceleration, 0.04f, 0.97f) * weaponaccuracy; // echte formule moet er nog in
    }

    float AimTimeFormula()
    {
        float distance = (player.transform.position - transform.position).magnitude;
        Quaternion angleToPlayer = Quaternion.LookRotation(player.transform.position - transform.position);

        float angle = Quaternion.Angle(transform.rotation, angleToPlayer);

        return Mathf.Max(0.0218f*distance + 0.0134f*angle, 0.1f);
    }

    public bool Person2PersonCast(Vector3 pos, Vector3 pos2, float height = 1f)
    {
        Vector3 rpos;
        Vector3 dir;
        Vector3 left = -Vector3.Cross(pos2 - pos, Vector3.up).normalized * 0.4f;
        Vector3 right = Vector3.Cross(pos2 - pos, Vector3.up).normalized * 0.4f;

        rpos = pos2 + Vector3.up* height + left;
        dir = rpos - pos;
        if (!Physics.Raycast(pos, dir.normalized, dir.magnitude, groundMask)) return true;
       // Debug.DrawRay(pos, dir, Color.blue);
        rpos = pos2 + Vector3.down * height + left;
        dir = rpos - pos;
        if (!Physics.Raycast(pos, dir.normalized, dir.magnitude, groundMask)) return true;
      //  Debug.DrawRay(pos, dir, Color.blue);
        rpos = pos2 + Vector3.up * height + right;
        dir = rpos - pos;
        if (!Physics.Raycast(pos, dir.normalized, dir.magnitude, groundMask)) return true;
      //  Debug.DrawRay(pos, dir, Color.blue);
        rpos = pos2 + Vector3.down * height + right;
        dir = rpos - pos;
        if (!Physics.Raycast(pos, dir.normalized, dir.magnitude, groundMask)) return true;
      //  Debug.DrawRay(pos, dir, Color.blue);

        return false;
        // return !Physics.Raycast(pos, dir.normalized, dir.magnitude, groundMask);

    }

    List<Vector2> NodesReachable(Vector2 pos)
    {

        Transform[] children = nodes.GetComponentsInChildren<Transform>();
        List<Vector2> reachableNodes = new();


        for (int i = 1; i < children.Length; i++)
        {
            
            Transform child = children[i];
            Vector2 childPos = new Vector2(child.position.x, child.position.z);


            if (!Cast(childPos, pos))
          //  if (!Physics.Raycast(child.position, dir.normalized, dir.magnitude, groundMask))
            {
                reachableNodes.Add(childPos);
            }
        }

        return reachableNodes;
    }

    List<Vector2> NodesReachableCover(Vector2 pos)
    {
        Transform[] children = nodes.GetComponentsInChildren<Transform>();
        List<Vector2> reachableNodes = new();

        for (int i = 1; i < children.Length; i++)
        {

            Transform child = children[i];
            Vector2 childPos = new Vector2(child.position.x, child.position.z);


            if (!Cast(childPos, pos))
            //  if (!Physics.Raycast(child.position, dir.normalized, dir.magnitude, groundMask))
            {
                reachableNodes.Add(childPos);
            }
        }

        children = cover.GetComponentsInChildren<Transform>();

        for (int i = 1; i < children.Length; i++)
        {

            Transform child = children[i];
            Vector2 childPos = new Vector2(child.position.x, child.position.z);


            if (!Cast(childPos, pos))
            //  if (!Physics.Raycast(child.position, dir.normalized, dir.magnitude, groundMask))
            {
                reachableNodes.Add(childPos);
            }
        }

        return reachableNodes;
    }

    List<Vector2> NearestCover(float distanceFromPlayer)
    {
        return AStarHide(new Vector2 (transform.position.x, transform.position.z));
    }

    bool Cast(Vector2 start, Vector2 end)
    {
        Vector3 start3 = new Vector3(start.x, 1f, start.y);
        Vector3 end3 = new Vector3(end.x, 1f, end.y);
      //  Debug.DrawLine(start3, end3, Color.blue, 100f);
        return Physics.SphereCast(start3, 0.4f, (end3 - start3).normalized, out _, (end3 - start3).magnitude, groundMask);

    }

    List<Vector2> AStarHide(Vector2 pos)
    {
        List<Vector2> openSet = new();
        List<Vector2> closedSet = new();
        openSet.Add(pos);

        Dictionary<Vector2, float> fScores = new();
        Dictionary<Vector2, float> gScores = new();
        gScores.Add(pos, 0);
        fScores.Add(pos, HCost(pos, pos));

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

            if (!Person2PersonCast(player.transform.position, new Vector3(bestNode.x, 1f,bestNode.y)))
            {
                List<Vector2> path = new();

                Vector2 current = bestNode;
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
            List<Vector2> neighbors = NodesReachableCover(bestNode);
            foreach (Vector2 neighbor in neighbors)
            {
                if (!openSet.Contains(neighbor) && !closedSet.Contains(neighbor) && HCost(neighbor, pos) < 10f)
                {
                    openSet.Add(neighbor);
                    float distance = HCost(bestNode, neighbor);
                    gScores.Add(neighbor, gScores[bestNode] + distance);
                    fScores.Add(neighbor, gScores[bestNode] + distance + HCost(neighbor, pos));
                    cameFrom.Add(neighbor, bestNode);
                }
            }
        }

        // print("no path found");
        // print(target);
        return new();
    }



    List<Vector2> AStarTarget(Vector2 pos, Vector2 target)
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


