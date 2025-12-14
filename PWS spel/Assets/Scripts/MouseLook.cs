using UnityEngine;

public class MouseLook : MonoBehaviour
{
	public float mouseSensitivity = 8;

    public float recoilX;
    public float recoilY;
    float mouseX;
    float mouseY;

    public bool canLook = true;

    public Transform playerBody;

    public Transform fpsbody;

    float xRotation = 0f;

    public float maxsway;
    public float smoothness;




    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (canLook)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        }
        else
        {
            mouseX = 0;
            mouseY = 0;
        }

        xRotation -= mouseY + recoilY * 0.02f;
        xRotation = Mathf.Clamp(xRotation, -90f, 67f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


   
      


        playerBody.Rotate(Vector3.up * (mouseX + recoilX * 0.02f));

       // if (Mathf.Abs(mouseX) > 0) {
        fpsbody.localRotation = Quaternion.Euler(Mathf.Clamp(smoothness*mouseY, -maxsway, maxsway), Mathf.Clamp(smoothness*-mouseX, -maxsway, maxsway), 0f);
        
       // else {
       // fpsbody.localRotation = Quaternion.Slerp(fpsbody.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime);
      //  }


        recoilX = 0;
        recoilY = 0;
    }
}
