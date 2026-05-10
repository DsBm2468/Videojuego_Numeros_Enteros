using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadScene(int numScene) 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(numScene); //ese numero es el index de la escena, se puuede ver en la lista de escenas en la parte derecha
    }

    // SALIR DEL VIDEOJUEGO
    public void ExitGame()
    {
        Application.Quit();
    }
}
