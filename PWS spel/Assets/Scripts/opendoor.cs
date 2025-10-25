using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.UIElements;

public class OpenDoor : MonoBehaviour
{

    public Transform doora;
    public Transform doorb;
    Vector3 startpos;
    Vector3 endpos;


    int inside;

    void Start()
    {
        startpos = doora.transform.position;
        endpos = doora.transform.position - 5f * doora.transform.right;


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            inside++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            inside--;
        }
    }

    void Update()
    {



        if (inside > 0)
        {
            if ((doora.transform.position - startpos).magnitude < 5f)
            {
                doora.transform.position -= doora.transform.right;
                doorb.transform.position -= doorb.transform.right;

            }

            //    Debug.Log((transform.position - (startpos + transform.right * 5)).magnitude);
        }

        else
        {
            if ((doora.transform.position - endpos).magnitude < 5f)
            {
     
                doora.transform.position += doora.transform.right;
                doorb.transform.position += doorb.transform.right;

            }
            //   Debug.Log((transform.position - (startpos)).magnitude);
        }
   

    }
}
