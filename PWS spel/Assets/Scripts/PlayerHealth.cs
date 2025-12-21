
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerHealth : MonoBehaviour
{
    public float health;

    public TextMeshProUGUI healthText;
    public Image damageimg;

    public GuiOpenScript guiOpenScript;
    public float starthealth;


    public void Hit(float damage)
    {
        health -= damage;

        Color c = damageimg.color;
        c.a = 1f;
        damageimg.color = c;

        if (health <= 0)
        {
            guiOpenScript.ShowDeathScreen();
            health = 0;
        }

        healthText.text = "+ " + Mathf.Round(health).ToString();
    }

    void Update()
    {
        health += Time.deltaTime;
        
        if (health > starthealth)
        {
            health = starthealth;
        }

        healthText.text = "+ " + Mathf.Round(health).ToString();

        Color c = damageimg.color;
        c.a = Mathf.MoveTowards(c.a, 1f-health/starthealth, 2f * Time.deltaTime);
        damageimg.color = c;
    }
}
