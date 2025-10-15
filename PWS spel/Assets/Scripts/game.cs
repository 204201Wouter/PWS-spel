using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class game : MonoBehaviour
{

    public Transform computer;
    public TextMeshProUGUI AmmoText;

    public Transform door1;
    public Transform door2;

    float progression;
    string objective;

    void Start()
    {
        progression = 0;
        objective = "computer";

        spawnenemy();
    }

    void spawnenemy()
    {
        door1.transform.position += Vector3.forward;
        door2.transform.position += Vector3.back;
        door1.transform.position -= Vector3.forward;
        door2.transform.position -= Vector3.back;
    }

    void Update()
    {
        if ((transform.position - computer.position).magnitude < 10f && objective == "computer")
        {
            progression += 0.1f;
            if (progression > 100f) objective = "";
            //AmmoText.text = "copying..." + Mathf.Round(progression); nu betere popup
        }
        else AmmoText.text = "";
    }
}
