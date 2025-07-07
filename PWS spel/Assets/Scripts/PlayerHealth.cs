using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health;

    public TextMeshProUGUI healthText;


    public void Hit(float damage)
    {
        health -= damage;

        healthText.text = "+ " + health.ToString();

     //   if (health <= 0)
      //  {
        //    Destroy(gameObject);
      //  }
    }
    
    
}
