using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[ExecuteInEditMode]
public class PersistentObjectCreator : MonoBehaviour
{

    public LayerMask groundMask;

  //  public GameObject cover;
    void Start()
    {
        if (!Application.isPlaying)
        {

            GameObject obj1 = new GameObject("test");
            obj1.transform.position = Vector3.zero;
            Transform[] children = GetComponentsInChildren<Transform>();

            for (int e = 1; e < children.Length; e++)
            {
                Transform child = children[e];

                List<Vector2> childPosList = new();
                childPosList.Add(new Vector2( - 2f, 0f));
                childPosList.Add(new Vector2(2f, 0));
                childPosList.Add(new Vector2(0,  - 2f));
                childPosList.Add(new Vector2(0, 2f));


                foreach (Vector2 childPos in childPosList)
                {
                    Vector2 pos = childPos + new Vector2(child.position.x, child.position.z);

                    bool valid = false;
          
                    if (!Physics.CheckSphere(new Vector3(pos.x, 1f, pos.y), 0.4f, groundMask))
                      
                    {

                        foreach (Vector2 childPos2 in childPosList)
                        {
                            Vector2 pos2 = childPos2 + pos;
                            if (Physics.CheckSphere(new Vector3(pos2.x, 1f, pos2.y), 0.4f, groundMask)) valid = true;
                        }
                        if (valid)
                        {
                            GameObject obj = new GameObject("coverNode");
                            obj.transform.position = new Vector3(pos.x, child.position.y, pos.y);
                        }
                    }
                }

            }



        }
    }
}
