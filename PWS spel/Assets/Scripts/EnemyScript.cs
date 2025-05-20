using UnityEngine;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    public int health = 5;

    public CharacterController controller;
    public LayerMask groundMask;
    Transform groundCheck;
    EnemyHandler enemyHandler;

    float ySpeed;
    public float gravity = -10f;
    bool isGrounded = false;

    void Start()
    {
        groundCheck = transform.GetChild(0);
        enemyHandler = GetComponentInParent<EnemyHandler>();
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
    }

    List<Vector2Int> AStar(Vector2Int target)
    {
        List<Vector2Int> path = new();
        Dictionary<Vector2, int> map = enemyHandler.map;


        return path;
    }

    int HCost(Vector2Int pos, Vector2Int target)
    {
        return 0;
    }
}
