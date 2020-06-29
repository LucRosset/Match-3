using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///////////////// Run this script before running BoardManager! /////////////////

/// <summary>
/// Possible responses for when a gem is clicked by the player
/// </summary>
public enum GemSelect
{
    Select,     // First gem of the pair to be selected
    Deselect,   // Gem was already selected, deselect
    Valid,      // Second gem selected; valid match
    NotValid    // Second gem selected; invalid match
}

/// <summary>
/// Interface between gems and board
/// </summary>
public class GemInterface : MonoBehaviour
{
    [Tooltip("Sound for gem swaps")]
    [SerializeField] AudioClip selectSound = null;

    // Cached references
    BoardManager board;
    AudioSource audioSource;

    // State variables
    private bool disableBoardUpdate = false;

    // Pair of selected gems
    Gem gem1 = null;
    Gem gem2 = null;

    void Start()
    {
        // Cache references
        board = GameObject.FindObjectOfType<BoardManager>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (disableBoardUpdate)
        {
            // Reset flag
            disableBoardUpdate = false;
            // Notify board not to update
            board.DontUpdate();
        }
    }

    /// <summary>
    /// Set a flag for Update toprevent the board from running an update in this frame
    /// </summary>
    public void NotDoneMoving() { disableBoardUpdate = true; }

    public GemSelect Select(Gem gem)
    {
        // Is this the first gem selected?
        if (!gem1)
        {
            // Mark this gem
            gem1 = gem;
            // Play selection sound
            audioSource.PlayOneShot(selectSound);
            return GemSelect.Select;
        }
        // Is it the same as the first one?
        if (gem1 == gem)
        {
            // De-select the gem
            gem1 = null;
            // Play selection sound
            audioSource.PlayOneShot(selectSound);
            return GemSelect.Deselect;
        }
        // Are the gems adjacent? (i.e. distance = 1 < sqrt(2))
        if (Vector2.Distance(gem1.transform.localPosition, gem.transform.localPosition) > 1.1f)
        {
            // Unmark gem 1
            gem1.SetNotSelected();
            gem1 = null;
            // Play selection sound
            audioSource.PlayOneShot(selectSound);
            return GemSelect.NotValid;
        }
        // Check validity of second gem selected
        gem2 = gem;
        // Get a copy of the table of gem types
        int[,] table = board.GetTableCopy();
        // Swap gems and check validity
        int x1, y1, x2, y2;
        x1 = gem1.GetCol();
        y1 = gem1.GetRow();
        x2 = gem2.GetCol();
        y2 = gem2.GetRow();
        table[x1,y1] = gem2.GetGemType();
        table[x2,y2] = gem1.GetGemType();
        // Check if this move makes a match
        List<int[]> matches = MatchLib.CheckMatches(table);
        if (matches.Count > 0)
        {
            // Swap gem positions
            gem1.MoveToCoordinate(x2, y2);
            gem2.MoveToCoordinate(x1, y1);
            // Swap gem positions in the board manager's tables
            board.SwapGems(x1, y1, x2, y2);
            // Pass on the list of matched gems
            board.SetGemsToDestroy(matches);
            // Notify gem1 to set the color back to normal
            gem1.SetNotSelected();
            // Unmark gems
            gem1 = null;
            gem2 = null;
            // Play no sound, the board will play the destroy sound next
            return GemSelect.Valid;
        }
        // else
        // Just change the first gem's color and unmark them
        gem1.SetNotSelected();
        // Unmark gems
        gem1 = null;
        gem2 = null;
        // Play selection sound
        audioSource.PlayOneShot(selectSound);
        return GemSelect.NotValid;
    }

    /// <summary>Cancel selection of a first gem by the player</summary>
    public void CancelGemSelection()
    {
        if (gem1)
        {
            // Remove shading
            gem1.SetNotSelected();
            // Unmark gem
            gem1 = null;
            // Play selection sound
            audioSource.PlayOneShot(selectSound);
        }
    }
}
