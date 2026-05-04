using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonReset : MonoBehaviour
{
    public void Reiniciar(int Nescena)
    {
        //Time.timeScale = 1f;
        SceneManager.LoadScene(Nescena);
    }
}
