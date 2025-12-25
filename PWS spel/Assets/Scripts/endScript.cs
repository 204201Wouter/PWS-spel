using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class endScript : MonoBehaviour
{

    public GameObject targetObject;
    public GameObject credits;

    float timer = 0f;


    void Start()
    {
        Destroy(targetObject, 10f);
    }

    

    void Update()
    {
  
        timer += Time.deltaTime;

   
        if (timer >= 15f)
        {
            credits.SetActive(true);
        }
    }
}
