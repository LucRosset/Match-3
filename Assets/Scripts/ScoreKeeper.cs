using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreKeeper : MonoBehaviour
{
    // Cached references
    GameSession session;
    TextMeshProUGUI scoreText;
    BoardManager board;
    Slider scoreProgress;

    // State variables
    int targetScore;

    void Start()
    {
        // Cache references
        session = GameObject.Find("Game Session").GetComponent<GameSession>();
        scoreText = GetComponent<TextMeshProUGUI>();
        board = FindObjectOfType<BoardManager>();
        scoreProgress = GameObject.Find("Score Slider").GetComponent<Slider>();
        // Get this level's goal
        targetScore = session.GetTargetScore();
        // Set goal text
        GameObject.Find("Score Goal Text").GetComponent<TextMeshProUGUI>().text = targetScore.ToString();
        // And since we're here, set the level text as well
        GameObject.Find("Level Text").GetComponent<TextMeshProUGUI>().text = session.GetLevel().ToString();
    }

    void Update()
    {
        // Get current score
        int score = board.GetScore();
        // Update text
        scoreText.text = score.ToString();
        // Set score progress bar
        scoreProgress.value = Mathf.Clamp01(score / (float)targetScore);
        // Check if reached score goal
        if (score >= targetScore) { session.Victory(); }
    }
}
