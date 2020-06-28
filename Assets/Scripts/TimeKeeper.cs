using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeKeeper : MonoBehaviour
{
    // Cached references
    GameSession session;
    TextMeshProUGUI timeText;
    BoardManager board;
    Slider timeSlider;

    // State variables
    float timeLimit;
    float timeRemaining;

    void Start()
    {
        // Cache references
        session = GameObject.Find("Game Session").GetComponent<GameSession>();
        timeText = GetComponent<TextMeshProUGUI>();
        board = FindObjectOfType<BoardManager>();
        timeSlider = GameObject.Find("Time Slider").GetComponent<Slider>();
        // Get this level's goal
        timeLimit = session.GetTimeLimit();
        timeRemaining = timeLimit;
    }

    void Update()
    {
        // Decrement remaining time
        timeRemaining -= Time.deltaTime;
        int timeToDisplay = (timeRemaining > 0) ? (int)timeRemaining : 0;
        // Update text
        timeText.text = timeToDisplay.ToString();
        // Set time remaining bar
        timeSlider.value = Mathf.Clamp01(timeRemaining / timeLimit);
        // Check if time is up
        if (timeRemaining < 0) { session.TimeIsUp(); }
    }
}
