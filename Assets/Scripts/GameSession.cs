using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton for management of level progression
/// </summary>
public class GameSession : MonoBehaviour
{
    [Tooltip("Time limit for a level, in seconds")]
    [SerializeField] float timeLimit = 120f;
    [Tooltip("Target score for beating the first level")]
    [SerializeField] int baseTargetScore = 1000;
    [Tooltip("Target score increment between levels")]
    [SerializeField] int targetScoreIncrement = 250;
    [Space]
    [Tooltip("Overlay prefab for the game over")]
    [SerializeField] GameObject gameOverPF = null;
    [Space]
    [Tooltip("Overlay prefab for the victory")]
    [SerializeField] GameObject victoryPF = null;
    [Tooltip("Time to wait before loading next level, in seconds")]
    [SerializeField] float victoryTime = 4f;

    // State variables
    bool gameStopped = false;
    int level = 1;

    // Cached references
    AudioSource audioSource;
    BoardManager board;
    SceneLoader sceneLoader;

    void Awake()
    {
        int sessions = FindObjectsOfType<GameSession>().Length;
        if (sessions > 1)
        {
            // Not the first one created. Destroy it
            Destroy(gameObject);
        }
        else
        {
            // Maintain this object throughout all levels and menus
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // Cache references
        audioSource = GetComponent<AudioSource>();
        StartOver();
    }

    private void StartOver()
    {
        board = FindObjectOfType<BoardManager>();
        sceneLoader = FindObjectOfType<SceneLoader>();
        gameStopped = false;
    }

    void Update()
    {
        // Check if needs to find new references (i.e. new Level was loaded)
        if (!board) { StartOver(); }
    }

    public void PlaySound(AudioClip clip) { audioSource.PlayOneShot(clip); }

    /// <summary>Calls the game over overlay and halts the board</summary>
    public void TimeIsUp()
    {
        // If another halt is already happening, ignore this
        if (gameStopped) { return; }
        // Call board.DontUpdate on every update
        gameStopped = true;
        // Create gameover overlay
        GameObject gameOver = Instantiate(
            gameOverPF,
            Vector3.zero,
            Quaternion.identity
        );
    }

    /// <summary>Calls the victory overlay, halts the board and schedules a new level to load</summary>
    public void Victory()
    {
        // If another overlay is already present, ignore this
        if (gameStopped) { return; }
        // Call board.DontUpdate on every update
        gameStopped = true;
        // Create victory overlay
        GameObject gameOver = Instantiate(
            victoryPF,
            Vector3.zero,
            Quaternion.identity
        );
        StartCoroutine(WaitToLoadNextLevel());
    }

    IEnumerator WaitToLoadNextLevel()
    {
        // Wait before starting a new level
        yield return new WaitForSeconds(victoryTime);
        // Increment level
        level++;
        // Load next level
        sceneLoader.LoadNextLevel();
    }

    public void Restart()
    {
        level = 1;
        sceneLoader.LoadNextLevel();
    }

    public void LoadMenu() { sceneLoader.LoadMenu(); }

    public void QuitGame() { sceneLoader.QuitGame(); }

    public int GetTargetScore() { return baseTargetScore + (level-1) * targetScoreIncrement; }

    public float GetTimeLimit() { return timeLimit; }

    public float GetLevel() { return level; }
}
