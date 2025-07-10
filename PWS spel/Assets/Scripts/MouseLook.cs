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

    float xRotation = 0f;

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
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * (mouseX + recoilX * 0.02f));

        recoilX = 0;
        recoilY = 0;
    }
}
