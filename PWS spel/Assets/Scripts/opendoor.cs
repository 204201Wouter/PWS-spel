using UnityEngine;
using System.Collections.Generic;

public class OpenDoor : MonoBehaviour
{
    public Transform player;
    public Transform enemyParent;
    Vector3 startpos;
    Vector3 endpos;
    float x = 5f;
    float z = 5f;

    bool open;

    void Start()
    {
        startpos = transform.position;
        endpos = transform.position - 5f * transform.right;

        if (Mathf.Abs(transform.right.x) > 0) x = 20f; 
        else z = 20f;
    }

    void Update()
    {
        open = false;
        if (Mathf.Abs(player.position.x - startpos.x) < x && Mathf.Abs(player.position.z - startpos.z) < z)
        {
            open = true; 
        }

        foreach (Transform child in enemyParent)
        {
            CharacterController controller = child.GetComponent<CharacterController>();

            if (controller != null)
            {
                if (Mathf.Abs(child.position.x - startpos.x) < x && Mathf.Abs(child.position.z - startpos.z) < z)
                {
                    open = true;
                }
            }
        }

        if (open)
        {
            if ((transform.position - startpos).magnitude < 5f)
            {
                transform.position -= transform.right;

            }

            //    Debug.Log((transform.position - (startpos + transform.right * 5)).magnitude);
        }

        else
        {
            if ((transform.position - endpos).magnitude < 5f)
            {
                transform.position += transform.right;

            }
            //   Debug.Log((transform.position - (startpos)).magnitude);
        }
   

    }
}
