using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonReset : MonoBehaviour
{
    public void ResetScene(int numScene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(numScene);
    }
}
