using UnityEngine;

public class OpenDoorScript : MonoBehaviour
{
    public Transform doora;
    public Transform doorb;
    Vector3 startpos;
    Vector3 endpos;

    int inside;

    public bool locked;

    void Start()
    {
        startpos = doora.transform.position;
        endpos = doora.transform.position - 5f * doora.transform.right;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null && !locked)
        {
            inside++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null && !locked)
        {
            inside--;
        }
    }

    void Update()
    {
        if (inside > 0)
        {
            if ((doora.transform.position - startpos).magnitude < 4.9f)
            {
                doora.transform.position -= doora.transform.right;
                doorb.transform.position -= doorb.transform.right;
            }
        }
        else
        {
            if ((doora.transform.position - endpos).magnitude < 4.9f)
            {
     
                doora.transform.position += doora.transform.right;
                doorb.transform.position += doorb.transform.right;

            }
        }
    }
}
