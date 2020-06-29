using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    // Cached references
    BoardManager board;
    // Transform of the game object that will keep all gems as children
    private Transform container;

    // The probability thing here is a quick-and-dirty trick and not necessary.
    // A better method can be easily used to set custom probabilities
    [Tooltip("Array of gem prefabs. Add the same gem multiple times to increase its probability of appearing")]
    [SerializeField] Gem[] gemPFs = null;

    void Start()
    {
        // Cache all needed references and values
        board = GetComponent<BoardManager>();
        container = GameObject.Find("Gem Container").transform;
    }

    /// <summary>Move gems down and spawn all gems missing from the board</summary>
    /// <param name="gemColumns">Array of gem columns</param>
    /// <param name="gemTable">Matrix with gem types</param>
    /// <param name="immediate">If true, skips falling animation</param>
    public void SpawnGems(bool immediate = false)
    {
        // Get necessary arrays and values
        List<Gem>[] gemColumns = board.GetGemColumns();
        int[,] gemTable = board.GetGemTable();
        int COL_NUM = BoardManager.COL_NUM;
        int ROW_NUM = BoardManager.ROW_NUM;
        // Go through each column and add missing gems
        for (int x = 0; x < COL_NUM; x++)
        {
            int numGems = gemColumns[x].Count;
            // If any gem is missing, add them
            if (numGems < ROW_NUM)
            {
                int y = 0;
                // Move all gems to the appropriate height
                for (; y < numGems; y++)
                {
                    // Move gem to the new position
                    if (immediate) { gemColumns[x][y].MoveToCoordinate(x, y); }
                    else { gemColumns[x][y].SetGoalPosition(x, y); }
                    // Write gem's type to table
                    gemTable[x, y] = gemColumns[x][y].GetGemType();
                }
                // Now add the missing gems
                for (int offset = 0; y < ROW_NUM; y++, offset++)
                {
                    // Instantiate a random gem
                    Gem newGem = Instantiate(
                        gemPFs[UnityEngine.Random.Range(0, gemPFs.Length)],
                        Vector3.zero,
                        Quaternion.identity
                    ) as Gem;
                    // Set gem's parent
                    newGem.transform.SetParent(container);
                    // Set gem's starting local position (and goal position)
                    if (immediate) { newGem.MoveToCoordinate(x, y); }
                    else
                    {
                        newGem.transform.localPosition = new Vector3(x, ROW_NUM+offset, 0);
                        newGem.SetGoalPosition(x, y);
                    }
                    // Add the gem's "Gem" component to the column
                    gemColumns[x].Add(newGem);
                    // Write gem's type to table
                    gemTable[x, y] = newGem.GetGemType();
                }
            }
        }
    }
}
