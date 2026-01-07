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
        // health omlaag met damage keer difficulty kwadraat, zonder kwadraat zijn de makkelijke difficulties alsnog heel lastig
        health -= damage * difficulty * difficulty;

        Color c = damageimg.color;
        c.a = 1f;
        damageimg.color = c;

        // ga dood als dood
        if (health <= 0)
        {
            guiScript.ShowDeathScreen();
            health = 0;
        }

        healthText.text = "+ " + Mathf.Round(health).ToString();
    }

    void Update()
    {
        // laat je rustig health omhooggaan
        if (health > 0) health += Time.deltaTime;
        
        if (health > starthealth) health = starthealth;

        // laat health zien
        healthText.text = "+ " + Mathf.Round(health).ToString();

        // altijd zichtbare damage indicator
        Color c = damageimg.color;
        c.a = Mathf.MoveTowards(c.a, 1f-health/starthealth, 2f * Time.deltaTime);
        damageimg.color = c;
    }
}
