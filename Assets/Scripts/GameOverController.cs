using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [Header("Elementos UI utilizados")]
    public GameObject GameOverPanel;

    private bool isGameOver = false; // Inicialmente el juego no ha terminado

    void Start()
    {
        GameOverPanel.SetActive(false);
    }

    public void gameState()
    {
        //if (isGameOver) // Si el juego está en un gameover
        {

        }
    }

    public void ActivateGameOver()
    {
        Time.timeScale = 0f; // Congela el tiempo del juego, pausando todo lo que ocurra en el
        isGameOver = true;
        GameOverPanel.SetActive(true);
        Debug.Log("<color=red>GAME OVER</color>");
    }

    public void ResetLevel(int NumberScene)
    {
        Time.timeScale = 1f; // El juego vuelve a correr como antes
        SceneManager.LoadScene(NumberScene);
    }

    public void ReturnMainScreen(int NumberScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(NumberScene);
    }
}
