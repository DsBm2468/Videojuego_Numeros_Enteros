using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [Header("Elementos UI utilizados")]
    public GameObject PausePanel;
    public GameObject PauseButton; // En aspectos como los botones es mejor que sean mencionados como GameObject ya que de esta forma, cuando ya no vayan a ser usados, estos desaparezcan junrto al panel en vez de quedar en pantalla pero sin poder usarlos

    private bool isPaused = false; // Inicialmente el juego no está pausado

    public void OnPauseKey(InputAction.CallbackContext context)
    {
        if (context.started) // context.started detecta el momento exacto en el que se presiona la tecla esc, evitando que salte varias veces seguidas como pasa al usar performed
        {
            PauseAction(); // Revisa la accion de pausa
        }
    }

    public void PauseAction()
    {
        // Aplica tanto al presionar el botón, como al presionar esc
        if (isPaused) // Si el juego está pausado...
        {
            ResumeGame(); // Lo reanuda
        }
        else
        {
            PausedGame(); // Si no, entonces se detendrá y diriguirá al menu de pausa
        }
    }

    public void PausedGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Congela el tiempo del juego, pausando todo lo que ocurra en el
        PausePanel.SetActive(true); // Aparece el panel del menú
        PauseButton.SetActive(false); // Oculta el botón de pausa
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // El juego vuelve a correr como antes
        PausePanel.SetActive(false); // Oculta el panel del menú
        PauseButton.SetActive(true); // Aparece el botón de pausa

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
