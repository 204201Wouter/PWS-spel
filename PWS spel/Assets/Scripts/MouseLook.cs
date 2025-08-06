using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using static UnityEngine.Rendering.DebugUI;

public class MouseLook : MonoBehaviour
{

    public string testmode;
	public float mouseSensitivity;

    public float recoilX;
    public float recoilY;

    public Transform playerBody;
    public Transform enemyBody;


    float xRotation = 0f;

    public Dictionary<int, int> table = new();

    public float StartAngle;
    public float StartTime;

 


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        float random = Mathf.Deg2Rad * Random.Range(0f, 360f);

        float range = 0;
        if (testmode == "aimtimedistance")
        {
            range = 30;
        }
        if (testmode == "aimtimeangle" || testmode == "aimtimecombined")
        {
            range = Random.value * 30;
        }
  

        

        xRotation -= Mathf.Cos(random)* range;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * Mathf.Sin(random)* range);//Random.Range(-30f, 30f));
        StartAngle = Vector3.Angle(Camera.main.transform.forward, new Vector3(0,0,1) );
        StartTime = Time.time;
        
     //   Debug.Log(StartAngle);
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
        /*
        if (Time.time > lastRecord + 0.1)
        {

            Vector3 pointOnRay = Vector3.Project(enemyBody.transform.position - playerBody.transform.position, Camera.main.transform.forward) + playerBody.transform.position;
            float distance = Vector3.Distance(pointOnRay, enemyBody.transform.position);

            table[Mathf.RoundToInt(Time.time*100)] = Mathf.RoundToInt(distance * 100);

            lastRecord = Time.time;
        }
        */




    }
}
