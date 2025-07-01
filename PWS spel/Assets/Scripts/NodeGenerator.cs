using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem.HID;
using UnityEditor.Experimental.GraphView;

public class NodeGenerator : MonoBehaviour
{

    public LayerMask groundMask;

    void Start()
    {
        List<Vector3> nodes = new List<Vector3>();
        Transform[] children = GetComponentsInChildren<Transform>();
        Debug.Log(children.Length);
        foreach (Transform child in children)
        {
            Debug.Log(child.position);
            List<Vector3> item = new List<Vector3>();
            foreach (Transform child2 in children)
            {

                Vector3 dir = child2.position - child.position;
                if (!Physics.Raycast(child.position, dir.normalized, dir.magnitude))
                {
                    item.Add(child2.position);


                }


                Vector3 pos;
                pos = child.position;

                // if (!Physics.CheckSphere(pos, 0.4f, groundMask))
                // { //Debug.Log(pos);
                nodes.Add(pos);
                // }


            }
        }
        // Debug.Log(nodes);
        foreach (Vector3 node in nodes)
        {
            Debug.DrawRay(node,new Vector3(0,100,0), Color.green, 100f);
            
        }





        //   Debug.DrawRay
    }
}
