using UnityEngine;
using System.Collections.Generic;

public class EnemyMovementScript : MonoBehaviour
{
    public CharacterController controller;
    public LayerMask groundMask;

    public GameObject player;
    public LayerMask playerMask;

    float ySpeed;
    public float gravity = -10f;
    public float speed = 2f;
    bool isGrounded = false;

    public Vector3 targetPos = Vector3.zero;

    public Animator animator;

    public GameObject map;
    public Transform nodes;
    public Transform cover;

    public string mode;

    public int ammo;
    float lastShot;
    public float reloadStart;
    private Vector3 lastPlayerPos;
    private float lastHearPlayer;
    float aimtimedone;
    bool aiming = false;

    public BoxCollider lift;

    Vector3 lateralVelocity;
    Vector3 lateralAcceleration;
    Vector3 lastPos;
    Vector3 lastLateralVelocity;
    Vector3 velocity;

    AudioSource audioSource;
    public AudioClip shotsound;
    public AudioClip reloadsound;
    public AudioClip walkSound;

    float walkPhase;
    public float frequency;

    List <Vector2> path = new();

    MagazineScript magazineScript;

    void Start()
    {
        magazineScript = GetComponentInChildren<MagazineScript>();
        audioSource = GetComponentInChildren<AudioSource>();
        ammo = magazineScript.cap;

        controller = GetComponent<CharacterController>();

        lastPlayerPos = player.transform.position;

        if (mode == "move")
        {
            Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), lift);
        }
    }

    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 90)
        {
            if (mode != "move")
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
                    }

                    if (ammo > 0 && Time.time > lastShot + magazineScript.shotCooldown && Time.time > aimtimedone)
                    {
                        lastShot = Time.time;
                        ammo -= 1;
                        audioSource.PlayOneShot(shotsound);
                        if (ammo >= 0)
                        {
                            if (ammo % 4 == 0) animator.SetTrigger("recoil");
                            if (ammo % 4 == 1) animator.SetTrigger("recoilb");
                            if (ammo % 4 == 2) animator.SetTrigger("recoilc");
                            if (ammo % 4 == 3) animator.SetTrigger("recoild");
                        }

                        float accuracy = AccuracyFormula();
                        for (int i = 0; i < magazineScript.ammoType.amount; i++)
                        {
                            if (Random.value < accuracy)
                            {
                                player.GetComponent<PlayerHealth>().Hit(magazineScript.ammoType.damage);
                                Debug.Log(magazineScript.ammoType.damage);
                            }
                        }
                        
                        velocity += 0.003f * -transform.forward;
                    }

                    if (ammo == 0 && reloadStart == -1)
                    {
                        animator.SetFloat("reloadspeed", 6f/magazineScript.reloadTime);
                        reloadStart = Time.time;
                        audioSource.PlayOneShot(reloadsound);

                        animator.SetTrigger("reload");

                    }
                    if (Time.time > reloadStart + magazineScript.reloadTime && reloadStart != -1)
                    {
                        reloadStart = -1;
                        ammo = magazineScript.cap;
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
                    // mode = "cover";
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
                }

                else if (mode == "cover" && Time.time > lastHearPlayer + 5f)
                {
                    mode = "scout";
                    lastPlayerPos = player.transform.position + Vector3.up * 2;
                }

                else if (mode == "scout" && (targetPos - transform.position).magnitude <= 0.05f && path.Count <= 1)
                {
                    mode = "guard";
                }

                if (!InteractScript.gravityDisabled)
                {
                    isGrounded = Physics.CheckSphere(transform.position - Vector3.up * 0.7f, 0.4f, groundMask);

                    if (isGrounded && ySpeed < 0)
                    {
                        ySpeed = -2;
                    }

                    ySpeed += gravity * Time.deltaTime;
                }
                else
                {
                    ySpeed += Random.Range(-0.01f, 0.01f);
                    ySpeed = Mathf.Clamp(ySpeed, -0.1f, 0.1f);
                }

                controller.Move(new Vector3(0, ySpeed, 0) * Time.deltaTime);
            }

          
            if (mode == "cover" || mode == "scout")
            {
                MoveEnemy(mode);
            }
            else if (mode == "guard")
            {
                path = new();
                targetPos = transform.position;
            }

            if (InteractScript.gravityDisabled)
            {
                controller.Move(velocity);

                velocity *= 0.99f;
                if (Mathf.Abs(velocity.x) < 0.001f) velocity.x = 0;
                if (Mathf.Abs(velocity.y) < 0.001f) velocity.y = 0;
                if (Mathf.Abs(velocity.z) < 0.001f) velocity.z = 0;

                Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
            }

            targetPos.y = transform.position.y;
            Vector3 diffTargetPos = targetPos - transform.position;
            if (diffTargetPos.magnitude > 0.05f && !InteractScript.gravityDisabled)
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
                float prev = Mathf.Repeat(walkPhase, Mathf.PI * 2);

                walkPhase += speed * frequency * Time.deltaTime;

                float curr = Mathf.Repeat(walkPhase, Mathf.PI * 2);

                if (curr < prev)
                {
                    audioSource.PlayOneShot(walkSound);
                }

                controller.Move(speed * Time.deltaTime * diffTargetPos.normalized);
            }
            else if (mode == "move")
            {
                if (InteractScript.gravityDisabled)
                {
                    if (diffTargetPos.magnitude > 0.05f)
                    {
                        velocity = diffTargetPos * 0.05f;
                    }
                    else
                    {
                        mode = "guard";
                        Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), lift, false);
                    }
                }
                else if (diffTargetPos.magnitude < 0.05f)
                {
                    mode = "guard";
                    Physics.IgnoreCollision(GetComponent<CapsuleCollider>(), lift, false);
                }
            }
            else if (path.Count > 1)
            {
                path.RemoveAt(path.Count - 1);
                targetPos = new Vector3(path[^1].x, transform.position.y, path[^1].y);
            }
            else
            {
                animator.SetFloat("speed", 0f);
            }

            Vector3 relativeVelocity = (transform.position - lastPos) / Time.deltaTime - player.GetComponent<Movement>().velocity;
            Vector3 directionToPlayer = (transform.position - player.transform.position).normalized;

            lateralVelocity = relativeVelocity - Vector3.Dot(relativeVelocity, directionToPlayer) * directionToPlayer;
            lateralAcceleration = (lateralVelocity - lastLateralVelocity) / Time.deltaTime;

            lastLateralVelocity = lateralVelocity;
            lastPos = transform.position;
        }
        if (Time.deltaTime > 0.2f) print(Time.deltaTime);
    }

    void MoveEnemy(string mode)
    {
        if ((player.transform.position - lastPlayerPos).magnitude > 1f)
        {
            if (mode == "cover") path = NearestCover();
            if (mode == "scout") path = AStarTarget(new Vector2(transform.position.x, transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z));

            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 start = new(path[i].x, transform.position.y, path[i].y);
                Vector3 end = new(path[i + 1].x, transform.position.y, path[i + 1].y);
                Debug.DrawLine(start, end, Color.green, 100f);
            }

            lastPlayerPos = player.transform.position;

            if (path.Count > 0)
            {
                targetPos = new(path[^1].x, transform.position.y, path[^1].y);
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

        return (player.transform.position - transform.position).magnitude < playerMovement.soundRadius;
    }

    public bool HasLineOfSight()
    {
        Vector3 dir = player.transform.position - transform.position;
        if (Vector3.Angle(dir.normalized, transform.forward) < 75f)
        {
            return Person2PersonCast(transform.position+Vector3.up*0.5f, player.transform.position);
        }
        else return false;
    }

    float AccuracyFormula()
    {
        float distance = (player.transform.position - transform.position).magnitude;
        float weaponaccuracy = 1f;
        int acceleration = 0;
        if (lateralAcceleration.magnitude > 10) acceleration = 1;
        print(0.001f * Mathf.Sqrt(distance) * magazineScript.ammoType.spread);
        return Mathf.Clamp(0.97f - 0.07f*Mathf.Sqrt(distance) - 0.04f*lateralVelocity.magnitude - 0.1f*acceleration - 0.001f*Mathf.Sqrt(distance)*magazineScript.ammoType.spread, 0.04f, 0.97f) * weaponaccuracy;
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

        rpos = pos2 + Vector3.up * height + left;
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
    }

    List<Vector2> NodesReachable(Vector2 pos)
    {
        List<Vector2> reachableNodes = new();

        foreach (Transform child in nodes)
        {
            Vector2 childPos = new(child.position.x, child.position.z);

            if (Vector2.Distance(childPos, pos) < 24f && !Cast(childPos, pos))
            {
                reachableNodes.Add(childPos);
            }
        }

        return reachableNodes;
    }

    List<Vector2> NodesReachableCover(Vector2 pos)
    {
        List<Vector2> reachableNodes = new();

        foreach (Transform child in nodes)
        {
            Vector2 childPos = new(child.position.x, child.position.z);

            if (Vector2.Distance(childPos, pos) < 24f && !Cast(childPos, pos))
            {
                reachableNodes.Add(childPos);
            }
        }

        foreach (Transform child in cover)
        {
            Vector2 childPos = new(child.position.x, child.position.z);

            if (Vector2.Distance(childPos, pos) < 24f && !Cast(childPos, pos))
            {
                reachableNodes.Add(childPos);
            }
        }

        return reachableNodes;
    }

    List<Vector2> NearestCover()
    {
        return AStarHide(new(transform.position.x, transform.position.z));
    }

    bool Cast(Vector2 start, Vector2 end)
    {
        Vector3 start3 = new(start.x, 1f, start.y);
        Vector3 end3 = new(end.x, 1f, end.y);
        return Physics.Raycast(start3, end3 - start3, out _, (end3 - start3).magnitude, groundMask);
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

        return new();
    }

    float HCost(Vector2 pos, Vector2 target)
    {
        return Vector2.Distance(pos, target);
    }
}


