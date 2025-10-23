using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using System.Drawing;

public class gravityanimation : MonoBehaviour
{

    public Transform ring1;
    public Transform ring2;
    public Transform ring3;





    void Update()
    {
        ring1.transform.Rotate(0, 0, 100f * Time.deltaTime);
        ring2.transform.Rotate(100f * Time.deltaTime, 0, 0);
        ring3.transform.Rotate(0, 0, 100f * Time.deltaTime);


    }
}
