
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerHealth : MonoBehaviour
{
    public float health;

    public TextMeshProUGUI healthText;
    public Image damageimg;


    public void Hit(float damage)
    {
        health -= damage;

        healthText.text = "+ " + health.ToString();
        Color c = damageimg.color;
        c.a = 1f;
        damageimg.color = c;


        //   if (health <= 0)
        //  {
        //    Destroy(gameObject);
        //  }
    }

    void Update()
    {
   
        Color c = damageimg.color;
        c.a = Mathf.MoveTowards(c.a, 1f-health/100f, 2f * Time.deltaTime);
        damageimg.color = c;
    }



}
