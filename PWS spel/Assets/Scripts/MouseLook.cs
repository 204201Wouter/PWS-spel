using UnityEngine;

public class MouseLook : MonoBehaviour
{
	public float mouseSensitivity;

    public float recoilX;
    public float recoilY;

    public Transform playerBody;


    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = (Input.GetAxis("Mouse X") * mouseSensitivity + recoilX) * 0.02f;
        float mouseY = (Input.GetAxis("Mouse Y") * mouseSensitivity + recoilY) * 0.02f;
        recoilX = 0;
        recoilY = 0;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);


    }
}
