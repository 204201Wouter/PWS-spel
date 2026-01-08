using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScript : MonoBehaviour
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
