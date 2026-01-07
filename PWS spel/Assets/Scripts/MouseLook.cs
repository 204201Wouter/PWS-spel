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

    // Deze video gebruikt voor een deel van de code: https://www.youtube.com/watch?v=_QajrabyTJc

    void Start()
    {
        // zorgt ervoor dat je de cursor niet kan zien
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // beweeg de camera met de Y en de hele player met de X
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

        // zorgt ervoor dat je niet gelijk op de muis zit maar er een beetje naar toe beweegt
        fpsbody.localRotation = Quaternion.Euler(Mathf.Clamp(smoothness*mouseY, -maxsway, maxsway), Mathf.Clamp(smoothness*-mouseX, -maxsway, maxsway), 0f);
        
        recoilX = 0;
        recoilY = 0;
    }
}