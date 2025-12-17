using UnityEngine;

public class gravityanimation : MonoBehaviour
{
    public Transform ring1;
    public Transform ring2;
    public Transform ring3;

    float speed = 1.7f;

    void Update()
    {
        if (InteractScript.gravityDisabled && speed != 0)
        {
            speed -= 0.001f;
            if (speed < 0.001f) speed = 0;
        }
        ring1.transform.Rotate(0, 0, 90f * Time.deltaTime * speed);
        ring2.transform.Rotate(100f * Time.deltaTime * speed, 0, 0);
        ring3.transform.Rotate(0, 0, 110f * Time.deltaTime * speed);
    }
}
