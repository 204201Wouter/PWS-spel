using UnityEngine;

public class Movement : MonoBehaviour
{
    public CharacterController controller;
    public WeaponScript weaponScript;
    public ShootProjectile shootProjectile;

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

    public AudioSource audioSource;
    public AudioClip walkSound;

    public Camera playercam;

    void Start()
    {
        startfpsbody = fpsbody.localPosition;
    }

    void Update()
    {
        float x = 0;
        float z = 0;
        if (canMove)
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        if (!InteractScript.gravityDisabled)
        {
            if (!isGrounded && Physics.CheckSphere(groundCheck.position, 0.1f, groundMask)) audioSource.PlayOneShot(walkSound);
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
                    if (isGrounded)
                    {
                        ySpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
                        animator.SetTrigger("jump");
                        animatorshadow.SetTrigger("jump");
                    }
                }
            }

            ySpeed += gravity * Time.deltaTime;

            controller.Move(new Vector3(0, ySpeed, 0) * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !Input.GetMouseButton(1) != climbing && !reloading) speed = 8f;
            if (!Input.GetKey(KeyCode.LeftShift) || Input.GetMouseButton(1) || climbing || reloading) speed = 4f;

            if (transform.position.y < -20)
            {
                transform.position = new Vector3(0, 5, 0);
            }

            velocity = (transform.position - lastPos) / Time.deltaTime;
        }
        else
        {
            controller.Move(velocity * Time.deltaTime);
            Vector3 realVelocity = (transform.position - lastPos) / Time.deltaTime;

            if (velocity.x != 0 && Mathf.Abs(realVelocity.x / velocity.x - 1) > 0.3f) velocity.x = realVelocity.x;
            if (velocity.y != 0 && Mathf.Abs(realVelocity.y / velocity.y - 1) > 0.3f) velocity.y = realVelocity.y;
            if (velocity.z != 0 && Mathf.Abs(realVelocity.z / velocity.z - 1) > 0.3f) velocity.z = realVelocity.z;

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


        // 4-8
        if (!shootProjectile.aiming) playercam.fieldOfView = Mathf.Clamp(1.25f * velocity.magnitude + 55f, 60f, 65f);


        if (Mathf.Abs(Mathf.Sin(bobspeed*frequency*(Time.time-startwalk))) < 0.1f)
        {
            if (velocity.magnitude > 1f) bobspeed = velocity.magnitude;
            if (velocity.magnitude < 1f) startwalk = Time.time;
        }

        bobspeed = 0f;

        
        if (isGrounded && !InteractScript.gravityDisabled) headbobTime += velocity.magnitude;

        if (InteractScript.gravityDisabled) x = 0f;
 
        fpsbody.localPosition = startfpsbody
            + amplitude * Mathf.Clamp(velocity.magnitude, -1f, 1f) * Mathf.Sin(headbobTime * frequency) * Vector3.up
            + amplitude * Mathf.Clamp(velocity.magnitude, -1f, 1f) * Mathf.Cos(headbobTime * frequency / 2f) * Vector3.right
            - amplitude2 * Mathf.Clamp(velocity.y, -10f, 10f) * Vector3.up
            + amplitude3 * x * Vector3.right
            - Vector3.forward * recoil;


        if (isGrounded && !InteractScript.gravityDisabled && headbobTime*frequency % (Mathf.PI*2) < velocity.magnitude*0.05f && (headbobTime-velocity.magnitude)*frequency % (Mathf.PI*2) > (Mathf.PI-velocity.magnitude*0.05f)) audioSource.PlayOneShot(walkSound);

        
        Vector2 vel2d = new(velocity.x, velocity.z);
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
