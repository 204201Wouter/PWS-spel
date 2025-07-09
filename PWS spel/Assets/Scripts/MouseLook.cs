using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class MouseLook : MonoBehaviour
{
	public float mouseSensitivity;

    public float recoilX;
    public float recoilY;

    public Transform playerBody;
    public Transform enemyBody;


    float xRotation = 0f;

    public Dictionary<int, int> table = new();

    float lastRecord;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        xRotation -= Random.Range(-30f, 30f);
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * Random.Range(-30f, 30f));

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

        if (Time.time > lastRecord + 0.1)
        {
            table[Mathf.RoundToInt(Time.time*100)] = Mathf.RoundToInt(Vector3.Angle(Camera.main.transform.forward, enemyBody.position- playerBody.position)*100);

            lastRecord = Time.time;
        }


    }
}
