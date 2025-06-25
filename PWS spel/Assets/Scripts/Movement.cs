using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class Movement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 2f;
    public float gravity = -10f;
    public float jumpHeight = 1f;
    bool canMove = true;

    float ySpeed;
    public Vector3 velocity;
    Vector3 lastPos;

    public LayerMask groundMask;
    public Transform groundCheck;
    bool isGrounded;





    void Update()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, groundMask);
        if (isGrounded && ySpeed < 0)
        {
            ySpeed = -2;
        }
        bool climbing = false;
        if (canMove)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(speed * Time.deltaTime * move);

            if (Input.GetButton("Jump"))
            {
  


                Ray rayBottom = new Ray(transform.position + new Vector3(0, -0.6f, 0), transform.forward);
                Ray rayTop = new Ray(transform.position+new Vector3(0, 1.5f, 0), transform.forward);

                if (isGrounded)
                {
                    ySpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }

                if (Physics.Raycast(rayBottom, 0.6f, groundMask) && !Physics.Raycast(rayTop, 0.6f, groundMask))
                {
                    climbing = true;
                    ySpeed = 2f;
                }
               
            }
        }

        ySpeed += gravity * Time.deltaTime;

        controller.Move(new Vector3(0, ySpeed, 0) * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !Input.GetMouseButton(1) != climbing) speed = 8f;
        if (!Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(1) || climbing) speed = 2f;

     //    if (Input.GetKey(KeyCode.LeftControl); crouch
    //    if (!Input.GetKey(KeyCode.X); crawl

        if (transform.position.y < -20)
        {
            transform.position = new Vector3(0, 5, 0);
        }

        velocity = (transform.position - lastPos)*Time.deltaTime;
        lastPos = transform.position;
    }
}
