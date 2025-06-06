using UnityEngine;

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

        if (canMove)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(speed * Time.deltaTime * move);

            if (isGrounded && Input.GetButton("Jump"))
            {
                ySpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        ySpeed += gravity * Time.deltaTime;

        controller.Move(new Vector3(0, ySpeed, 0) * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded) speed = 8f;
        if (!Input.GetKey(KeyCode.LeftShift)) speed = 2f;


        if (transform.position.y < -20)
        {
            transform.position = new Vector3(0, 5, 0);
        }

        velocity = transform.position - lastPos;
        lastPos = transform.position;
    }
}
