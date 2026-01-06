using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health;

    public TextMeshProUGUI healthText;
    public Image damageimg;

    public GuiScript guiScript;
    public float starthealth;

    public static float difficulty = 0.25f;

    public void Hit(float damage)
    {
        health -= damage * difficulty * difficulty;

        Color c = damageimg.color;
        c.a = 1f;
        damageimg.color = c;

        if (health <= 0)
        {
            guiScript.ShowDeathScreen();
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
