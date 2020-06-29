using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // CONSTANTS
    // Number of rows / column length / elements along Y / height
    public const int ROW_NUM = 7;
    // Number of columns / row length / elements along X / width
    public const int COL_NUM = 7;

    // State variables
    private bool dontUpdate = false;
    private bool needToSpawnGems = false;
    private bool moveMade = false;

    // Cached references
    GemSpawner spawner;
    GemDestroyer destroyer;
    // When enabled, the collider blocks the gems from being clicked on by the player
    private Collider2D myCollider;
    // Transform of the game object that will keep all gems as children
    private Transform container;
    // Audio source component
    private AudioSource audioSource;

    // Table containing gem types of each gem in the board
    private int[,] gemTable;
    // Array of arrays of gems in each column
    private List<Gem>[] gemColumns;

    // The probability thing here is a quick-and-dirty trick and not necessary.
    // A better method can be easily used to set custom probabilities
    [Tooltip("Point value of a single gem broken in a combo-less move")]
    [SerializeField] int basePoints = 10;
    private int totalScore = 0;
    private int comboCounter = 0;
    private List<int[]> gemsToDestroy = new List<int[]>();

    [Space]
    [Tooltip("Prefab of the text effect for score awarded")]
    [SerializeField] GameObject scorePF = null;

    [Space]
    [Tooltip("Sound for when gems are destroyed")]
    [SerializeField] AudioClip destroySound = null;

    void Start()
    {
        // Cache references
        spawner = GetComponent<GemSpawner>();
        destroyer = GetComponent<GemDestroyer>();
        myCollider = GetComponent<Collider2D>();
        container = GameObject.Find("Gem Container").transform;
        audioSource = GetComponent<AudioSource>();
        // Initialize lists and arrays
        gemTable = new int[COL_NUM,ROW_NUM];
        gemColumns = new List<Gem>[COL_NUM];
        for (int x = 0; x < COL_NUM; x++)
        {
            gemColumns[x] = new List<Gem>();
        }
        // Fill board with gems
        spawner.SpawnGems(true);
        // Break possible matches
        gemsToDestroy = MatchLib.CheckMatches(gemTable);
        // And keep doing so until there are no matches left
        while (gemsToDestroy.Count > 0)
        {
            destroyer.DestroyGems(true);
            spawner.SpawnGems(true);
            gemsToDestroy = MatchLib.CheckMatches(gemTable);
        }
        // Make sure there are possible moves for the player and no unbroken matches
        if (!MatchLib.MovementIsPossible(gemTable))
        {
            MatchLib.Shuffle(gemColumns, gemTable);
        }
        // Disable collider
        myCollider.enabled = false;
    }

    // Hierarchy of actions: only one action per update: one gem breaking or one gem spawning
    void Update()
    {
        // Check if can update something in this frame
        if (dontUpdate)
        {
            // Reset flag
            dontUpdate = false;
            // Don't allow players to input
            myCollider.enabled = true;
            return;
        }
        // Needs to break gems
        if (gemsToDestroy.Count > 0)
        {
            // Check if a new move was made
            if (moveMade)
            {
                // Clear flag
                moveMade = false;
                // Disable player input by putting a collider in front of the gems
                myCollider.enabled = true;
                // Reset combo counter
                comboCounter = 0;

            }
            // Update combo counter
            comboCounter++;
            // Destroy gems and get the points
            int score = destroyer.DestroyGems() * basePoints * comboCounter;
            gemsToDestroy.Clear();
            // Make score effects
            audioSource.PlayOneShot(destroySound);
            if (scorePF)
            {
                ScoreEffect effect = Instantiate(
                    scorePF,
                    transform.position + Vector3.up*ROW_NUM*.6f,
                    Quaternion.identity
                ).GetComponent<ScoreEffect>();
                effect.SetScore(score);
                effect.transform.SetParent(transform);
            }
            // Add points to total
            totalScore += score;
            // Flag to move and spawn gems
            needToSpawnGems = true;
        }
        // Needs to move and spawn gems
        else if (needToSpawnGems)
        {
            // Clear flag
            needToSpawnGems = false;
            spawner.SpawnGems();
            // Check if more matches were made
            gemsToDestroy = MatchLib.CheckMatches(gemTable);
            // If not, check if new moves are possible
            if (gemsToDestroy.Count == 0 && !MatchLib.MovementIsPossible(gemTable))
            {
                // Shuffle until a new board with possible moves and no matches is made
                MatchLib.Shuffle(gemColumns, gemTable);
            }
        }
        // Else, nothing is happening. Allow player Input
        else { myCollider.enabled = false; }
    }

    public void DontUpdate() { dontUpdate = true; }

    public int GetScore() { return totalScore; }

    public List<Gem>[] GetGemColumns() { return gemColumns; }
    public int[,] GetGemTable() { return gemTable; }

    /// <summary>Creates a copy of the table of gem types</summary>
    /// <returns>Copy of the table of gem types</returns>
    public int[,] GetTableCopy()
    {
        int[,] table = new int[COL_NUM,ROW_NUM];
        Array.Copy(gemTable, table, COL_NUM*ROW_NUM);
        return table;
    }

    public void SetGemsToDestroy(List<int[]> list) { gemsToDestroy = list; }
    public List<int[]> GetGemsToDestroy() { return gemsToDestroy; }

    /// <summary>Swap two gems with the provided coordinates</summary>
    /// <param name="x1">First gem's column (x)</param>
    /// <param name="y1">First gem's row (y)</param>
    /// <param name="x2">Second gem's column (x)</param>
    /// <param name="y2">Second gem's row (y)</param>
    public void SwapGems(int x1, int y1, int x2, int y2)
    {
        int type         = gemTable[x1, y1];
        gemTable[x1, y1] = gemTable[x2, y2];
        gemTable[x2, y2] = type;
        Gem gem            = gemColumns[x1][y1];
        gemColumns[x1][y1] = gemColumns[x2][y2];
        gemColumns[x2][y2] = gem;
        // Flag that a move was made
        moveMade = true;
    }
}