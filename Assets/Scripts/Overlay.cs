using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Just an interface to call the relevant game session methods</summary>
public class Overlay : MonoBehaviour
{
    // Cached references
    GameSession session;

    [Tooltip("Button press sound")]
    [SerializeField] AudioClip buttonPress = null;
    
    void Start()
    {
        session = GameObject.Find("Game Session").GetComponent<GameSession>();
        // Set main camera as canvas' camera
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void Restart()
    {
        session.PlaySound(buttonPress);
        session.Restart();
    }

    public void LoadMenu()
    {
        session.PlaySound(buttonPress);
        session.LoadMenu();
    }

    public void QuitGame()
    {
        session.PlaySound(buttonPress);
        session.QuitGame();
    }
}
