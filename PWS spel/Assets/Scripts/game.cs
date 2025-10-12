using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class game : MonoBehaviour
{

    public Transform computer;
    public TextMeshProUGUI AmmoText;

    float progression;
    string objective;

    void Start()
    {
        progression = 0;
        objective = "computer";
    }

    void spawnenemy()
    {
        
    }

    void Update()
    {
        if ((transform.position - computer.position).magnitude < 10f && objective == "computer")
        {
            progression += 0.1f;
            if (progression > 100f) objective = "";
            AmmoText.text = "copying..." + Mathf.Round(progression);
        }
        else { AmmoText.text = "";}
    }
}
