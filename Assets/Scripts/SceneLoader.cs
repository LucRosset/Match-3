using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>Load menu screen</summary>
    public void LoadMenu() { SceneManager.LoadScene("Menu"); }

    /// <summary>Load next level</summary>
    public void LoadNextLevel() { SceneManager.LoadScene("Level"); }

    /// <summary>Quit application</summary>
    public void QuitGame() { Application.Quit(); }
}
