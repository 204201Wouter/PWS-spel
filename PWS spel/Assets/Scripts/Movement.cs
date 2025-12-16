using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class Movement : MonoBehaviour
{
    public CharacterController controller;
    public WeaponScript weaponScript;
    public GuiOpenScript guiOpenScript;

    public float speed = 4f;
    public float gravity = -10f;
    public float jumpHeight = 1f;
    public bool canMove = true;

    float ySpeed;
    public Vector3 velocity;
    Vector3 lastPos;

    public LayerMask groundMask;
    public Transform groundCheck;
    public bool isGrounded;
    bool lastisGrounded = true;
    public float soundRadius;
    public bool firedGun;
    public bool reloading;

    public Animator animator;
    public Animator animatorshadow;

    public Transform fpsbody;

    Vector3 startfpsbody;

    float startwalk;
    public float frequency;
    public float amplitude;
    public float amplitude2;

    public float amplitude3;

    float bobspeed = 0f;

    float headbobTime = 0f;
    public float recoil;

    void Start()
    {
        startfpsbody = fpsbody.localPosition;
    }

    void Update()
    {


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (!InteractScript.gravityDisabled)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundMask);

            if (isGrounded && ySpeed < 0)
            {
                ySpeed = -2;
            }
            bool climbing = false;
            if (canMove)
            {


                Vector3 move = transform.right * x + transform.forward * z;

                controller.Move(speed * Time.deltaTime * move);

                if (Input.GetButton("Jump"))
                {

                    Ray rayBottom = new Ray(transform.position + new Vector3(0, -0.6f, 0), transform.forward);
                    Ray rayTop = new Ray(transform.position + new Vector3(0, 1.5f, 0), transform.forward);

                    if (isGrounded)
                    {
                        ySpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
                        animator.SetTrigger("jump");
                        animatorshadow.SetTrigger("jump");

                    }
                    /*
                    if (Physics.Raycast(rayBottom, 0.6f, groundMask) && !Physics.Raycast(rayTop, 0.6f, groundMask))
                    {
                        climbing = true;
                        ySpeed = 2f;
                    }
                    */

                }

            }

            ySpeed += gravity * Time.deltaTime;

            controller.Move(new Vector3(0, ySpeed, 0) * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !Input.GetMouseButton(1) != climbing && !reloading) speed = 8f;
            if (!Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(1) || climbing || reloading) speed = 4f;

            //if (Input.GetKey(KeyCode.LeftControl); crouch
            //if (!Input.GetKey(KeyCode.X); crawl

            if (transform.position.y < -20)
            {
                transform.position = new Vector3(0, 5, 0);
            }

            velocity = (transform.position - lastPos) / Time.deltaTime;
        }
        else
        {
            controller.Move(velocity);

            velocity *= 0.99f;
            if (Mathf.Abs(velocity.x) < 0.001f) velocity.x = 0;
            if (Mathf.Abs(velocity.y) < 0.001f) velocity.y = 0;
            if (Mathf.Abs(velocity.z) < 0.001f) velocity.z = 0;
        }

        if (velocity.magnitude >= 8 && isGrounded)
        {
            soundRadius = 50;
        }
        else if (!lastisGrounded && isGrounded)
        {
            soundRadius = 40;
        }
        else if (velocity.magnitude >= 4 && isGrounded)
        {
            soundRadius = 20;
        }
        else if (velocity.magnitude >= 10 && isGrounded) //crouchspeed
        {
            soundRadius = 0;
        }
        else {
            soundRadius = 0;
        }



  
        //if (velocity.magnitude < 1f && Mathf.Abs(Mathf.Sin(velocity.magnitude*frequency*(startwalk-Time.time))) < 0.05f)
       // {
          //  startwalk = Time.time;
          //  fpsbody.localPosition = startfpsbody;

       // }


        if (Mathf.Abs(Mathf.Sin(bobspeed*frequency*(Time.time-startwalk))) < 0.1f)
        {
            if (velocity.magnitude > 1f)
                bobspeed = velocity.magnitude;
            if (velocity.magnitude < 1f)
            {
                startwalk = Time.time;
            }
        }

        bobspeed = 0f;

        /*
        fpsbody.localPosition = startfpsbody
            -amplitude*Vector3.up*Mathf.Abs(Mathf.Sin(bobspeed*frequency*(Time.time-startwalk)))
            +amplitude3*x*Vector3.right
            -amplitude2*Vector3.up*Mathf.Clamp(velocity.y,-10f,10f)
            
            ;*/



        
        if (isGrounded) {
            headbobTime += velocity.magnitude ;
        }

        fpsbody.localPosition = startfpsbody
            +Mathf.Clamp(velocity.magnitude,-1f,1f)*amplitude*Vector3.up*Mathf.Sin(headbobTime*frequency)
            +Mathf.Clamp(velocity.magnitude,-1f,1f)*amplitude*Vector3.right*Mathf.Cos(headbobTime*frequency/2f)
            -amplitude2*Vector3.up*Mathf.Clamp(velocity.y,-10f,10f)
            +amplitude3*x*Vector3.right

            -Vector3.forward*recoil;

            ;     





        Vector2 vel2d = new Vector2(velocity.x, velocity.z);
        animator.SetFloat("speed", vel2d.magnitude);
        animatorshadow.SetFloat("speed", vel2d.magnitude);
        
        if (firedGun)
        {
            soundRadius = 100;
        }

        lastPos = transform.position;
        lastisGrounded = isGrounded;
        firedGun = false;
    }
}
