using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemDestroyer : MonoBehaviour
{
    // Cached references
    BoardManager board;

    void Start()
    {
        // Cache references
        board = GetComponent<BoardManager>();
    }

    /// <summary>Checks and destroys all possible gem matches</summary>
    /// <param name="immediate">If true, will not play the animation</param>
    /// <return>Points awarded by destroying the gems</returns>
    public int DestroyGems(bool immediate = false)
    {
        // Get necessary lists and arrays
        List<Gem>[] gemColumns = board.GetGemColumns();
        int[,] gemTable = board.GetGemTable();
        List<int[]> gemsToDestroy = board.GetGemsToDestroy();
        List<Gem> destroyQueue = new List<Gem>();
        // Go through gems listed to be destroyed
        foreach (int[] coord in gemsToDestroy)
        {
            Gem gem = gemColumns[coord[0]][coord[1]];
            destroyQueue.Add(gem);
        }
        foreach (Gem gem in destroyQueue)
        {
            // No need to check if gem was already destroyed, as this will not raise an error
            // Remove list element
            gemColumns[gem.GetCol()].Remove(gem);
            // Destroy gem's GameObject without animation
            if (immediate) { Destroy(gem.gameObject); }
            // Destroy gem through an animation
            else { gem.DestroyGem(); }
        }
        // Points accumulated from this iteration of gems destroyed
        return gemsToDestroy.Count;
    }
}
