using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using System.Drawing;

public class game : MonoBehaviour
{

    public Transform computer;
    public TextMeshProUGUI AmmoText;

    public Transform door1;
    public Transform door2;
    public Transform spawn;

    float progression;
    string objective;
    int spawnenemy = 1;

    Vector3 startpos;
    Vector3 endpos;

    public Transform enemyParent;
    public GameObject originalEnemy;


    void Start()
    {
        //progression = 0;
        //objective = "computer";

        startpos = door1.transform.position;
        endpos = door1.transform.position - 5f * door1.transform.up;



        //   spawnenemy();
    }
    /*
    void spawnenemy()
    {
        door1.transform.position += Vector3.forward*5f;
        door2.transform.position += Vector3.back * 5f;
        door1.transform.position -= Vector3.forward * 5f;
        door2.transform.position -= Vector3.back * 5f;
    }*/

    void Update()
    {
        if (spawnenemy >= 1)
        {


            if (spawnenemy == 1)
            {
                if ((door1.transform.position - startpos).magnitude < 5f)
                {
                    door1.transform.position -= door1.transform.up;
                    door2.transform.position += door2.transform.up;

                }
                else spawnenemy = 2;

                //    Debug.Log((transform.position - (startpos + transform.right * 5)).magnitude);
            }

            if (spawnenemy == 2)
            {
                foreach (Transform point in spawn)
                {
                    GameObject enemy = Instantiate(originalEnemy, point.transform.position, point.transform.rotation, enemyParent);
                    enemy.GetComponent<EnemyScript>().enabled = true;
                    enemy.GetComponent<EnemyMovementScript>().enabled = true;
                }


                spawnenemy = 3;
            }
            if (spawnenemy >= 3 && spawnenemy < 1000)
            {
                spawnenemy++;
            }

            if (spawnenemy >= 1000)
            {
                if ((door1.transform.position - endpos).magnitude < 5f)
                {
                    door1.transform.position += door1.transform.up;
                    door2.transform.position -= door2.transform.up;

                }
                else spawnenemy = 0;
                //   Debug.Log((transform.position - (startpos)).magnitude);
            }


        }





    }
}
