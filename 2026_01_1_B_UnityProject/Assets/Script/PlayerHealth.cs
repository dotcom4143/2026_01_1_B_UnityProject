using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    public int curentLives;

    public float invincibleTime = 1.0f;
    public bool isinvincible = false;

    void Start()
    {
        curentLives = maxLives;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Missile"))
        {
            curentLives--;
            Destroy(other.gameObject);

            if (curentLives <= 0) 
            {
                GameOver();
            }
        }
    }

    void GameOver()
    {
        gameObject.SetActive(false);
        Invoke("RestartGame", 3.0f);
    }
    
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
