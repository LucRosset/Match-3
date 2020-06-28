using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class encapsulates many of the methods used exhaustively by BoardManager
/// </summary>
public class MatchLib
{
    /// <summary>
    /// Checks all matches in the board
    /// </summary>
    /// <returns>
    /// List of gem indexes (coordinates) to be destroyed
    /// </returns>
    static public List<int[]> CheckMatches(int[,] table)
    {
        // Get table dimensions
        int COL_NUM = table.GetLength(0);
        int ROW_NUM = table.GetLength(1);

        // List of gem coordinates (column x row)
        List<int[]> list = new List<int[]>();
        // Go through horizontal matches
        for (int y = 0; y < ROW_NUM; y++)
        {
            // Keep track of the type of gem analysed
            int type = -1;
            // Number of gems of the same type in this streak
            int streak = 0;
            for (int x = 0; x < COL_NUM; x++)
            {
                // If this gem is the same type as the previous
                if (table[x,y] == type) { streak++; }
                // If not, start a new streak
                else
                {
                    // If streak is of 3 or more gems, add them to the list
                    if (streak >= 3)
                    {
                        for (int i = x-1; i > x-1-streak; i--)
                        {
                            list.Add(new int[] { i, y });
                        }
                    }
                    // Now start a new streak
                    streak = 1;
                    type = table[x,y];
                }
            }
            // Repeat check after end of the row
            if (streak >= 3)
            {
                for (int i = COL_NUM-1; i > COL_NUM-1-streak; i--)
                {
                    list.Add(new int[] { i, y });
                }
            }
        }
        // Go through vertical matches
        for (int x = 0; x < COL_NUM; x++)
        {
            // Keep track of the type of gem analysed
            int type = -1;
            // Number of gems of the same type in this streak
            int streak = 0;
            for (int y = 0; y < ROW_NUM; y++)
            {
                // If this gem is the same type as the previous
                if (table[x, y] == type) { streak++; }
                // If not, start a new streak
                else
                {
                    // If streak is of 3 or more gems, add them to the list
                    if (streak >= 3)
                    {
                        for (int i = y-1; i > y-1-streak; i--)
                        {
                            list.Add(new int[] { x, i });
                        }
                    }
                    // Now start a new streak
                    streak = 1;
                    type = table[x, y];
                }
            }
            // Repeat streak check after end of the row
            if (streak >= 3)
            {
                for (int i = ROW_NUM-1; i > ROW_NUM-1-streak; i--)
                {
                    list.Add(new int[] { x, i });
                }
            }
        }
        // Return total accumulated points
        return list;
    }

    /// <summary>
    /// Checks if there are any possible movements in the 3x2 window provided
    /// </summary>
    /// <param name="table">Table with all gem types in the tabletop</param>
    /// <param name="x">Leftmost column of the window</param>
    /// <param name="y">Lowest row of the window</param>
    /// <returns>true if the window has a possible movement, else false</returns>
    static public bool Check3x2Window(int[,] table, int x, int y)
    {
        bool conditions = (
            (table[x,y]   == table[x+1,y]   && table[x,y]   == table[x+2,y+1]) || // ..'
            (table[x,y]   == table[x+1,y+1] && table[x,y]   == table[x+2,y])   || // .'.
            (table[x,y+1] == table[x+1,y]   && table[x,y+1] == table[x+2,y])   || // '..
            (table[x,y+1] == table[x+1,y+1] && table[x,y+1] == table[x+2,y])   || // ''.
            (table[x,y+1] == table[x+1,y]   && table[x,y+1] == table[x+2,y+1]) || // '.'
            (table[x,y]   == table[x+1,y+1] && table[x,y]   == table[x+2,y+1])    // .''
        );
        return conditions;
    }

    /// <summary>
    /// Checks if there are any possible movements in the 2x3 window provided
    /// </summary>
    /// <param name="table">Table with all gem types in the tabletop</param>
    /// <param name="x">Leftmost column of the window</param>
    /// <param name="y">Lowest row of the window</param>
    /// <returns>true if the window has a possible movement, else false</returns>
    static public bool Check2x3Window(int[,] table, int x, int y)
    {
        bool conditions = (
            (table[x  ,y] == table[x  ,y+1] && table[x  ,y] == table[x+1,y+2]) ||
            (table[x  ,y] == table[x+1,y+1] && table[x  ,y] == table[x  ,y+2]) ||
            (table[x+1,y] == table[x  ,y+1] && table[x+1,y] == table[x  ,y+2]) ||
            (table[x+1,y] == table[x+1,y+1] && table[x+1,y] == table[x  ,y+2]) ||
            (table[x+1,y] == table[x  ,y+1] && table[x+1,y] == table[x+1,y+2]) ||
            (table[x  ,y] == table[x+1,y+1] && table[x  ,y] == table[x+1,y+2])   
        );
        return conditions;
    }

    /// <summary>
    /// Checks if there are any possible movements in the 4x1 window provided
    /// </summary>
    /// <param name="table">Table with all gem types in the tabletop</param>
    /// <param name="x">Leftmost column of the window</param>
    /// <param name="y">Lowest row of the window</param>
    /// <returns>true if the window has a possible movement, else false</returns>
    static public bool Check4x1Window(int[,] table, int x, int y)
    {
        bool conditions = (
            (table[x,y] == table[x+1,y] && table[x,y] == table[x+3,y]) || // ooxo
            (table[x,y] == table[x+2,y] && table[x,y] == table[x+3,y])    // oxoo
        );
        return conditions;
    }

    /// <summary>
    /// Checks if there are any possible movements in the 1x4 window provided
    /// </summary>
    /// <param name="table">Table with all gem types in the tabletop</param>
    /// <param name="x">Leftmost column of the window</param>
    /// <param name="y">Lowest row of the window</param>
    /// <returns>true if the window has a possible movement, else false</returns>
    static public bool Check1x4Window(int[,] table, int x, int y)
    {
        bool conditions = (
            (table[x,y] == table[x,y+1] && table[x,y] == table[x,y+3]) ||
            (table[x,y] == table[x,y+2] && table[x,y] == table[x,y+3])
        );
        return conditions;
    }

    /// <summary>
    /// Goes through the tabletop and checks is there are any possible moves
    /// </summary>
    /// <param name="table">Table with all gem typer in the tabletop</param>
    /// <returns>
    /// Returns true if movements are possible, else false
    /// </returns>
    // Strategy: Sweeps 4x1, 1x4, 3x2 and 2x3 windows through the tabletop and
    // evaluate if there are any moves that create matches
    public static bool MovementIsPossible(int[,] table)
    {
        // Get table dimensions
        int COL_NUM = table.GetLength(0);
        int ROW_NUM = table.GetLength(1);

        // Sweep 4x1 and 1x4 windows
        for (int x = 0; x < COL_NUM-3; x++)
        {
            for (int y = 0; y < ROW_NUM; y++)
            {
                // Check 4x1 window
                if (Check4x1Window(table, x, y)) { return true; }
                // SINCE THE BOARD IS SQUARE, check the symmetric 1x4 window as well
                if (Check1x4Window(table, y, x)) { return true; }
            }
        }// Sweep 3x2 and 2x3 windows
        for (int x = 0; x < COL_NUM-2; x++)
        {
            for (int y = 0; y < ROW_NUM-1; y++)
            {
                // Check 3x2 window
                if (Check3x2Window(table, x, y)) { return true; }
                // SINCE THE BOARD IS SQUARE, check the symmetric 2x3 window as well
                if (Check2x3Window(table, y, x)) { return true; }
            }
        }
        return false;
    }

    /// <summary>Apply the Durstenfeld shuffle algorithm to the board</summary>
    /// <param name="gemColumns">Table with gems</param>
    /// <param name="table">Table with gem types</param>
    public static void Shuffle(List<Gem>[] gemColumns, int[,] table)
    {
        // Get board dimensions
        int COL_NUM = gemColumns.Length;
        int ROW_NUM = gemColumns[0].Count;
        do
        {
            // Shuffle the board, mapping the indices from 1D to 2D
            for (int end = (COL_NUM*ROW_NUM)-1; end > 0; end--)
            {
                // Pick a random gem
                int pick = Random.Range(0, end);
                int pickX = pick % COL_NUM;
                int pickY = pick / COL_NUM;
                int endX = end % COL_NUM;
                int endY = end / COL_NUM;
                // Swap positions `pick' and `end' for both tables
                Gem aux                  = gemColumns[pickX][pickY];
                gemColumns[pickX][pickY] = gemColumns[endX][endY];
                gemColumns[endX][endY]   = aux;
                int type            = table[pickX, pickY];
                table[pickX, pickY] = table[endX, endY];
                table[endX, endY]   = type;
            }
            // Repeat this while matches are created or movement is not possible
        } while (CheckMatches(table).Count > 0 || !MovementIsPossible(table));
        // Put gems in the correct positions
        for (int x = 0; x < COL_NUM; x++)
            for (int y = 0; y < ROW_NUM; y++)
                // Just set the goal position as the actual position and let the gems move there
                gemColumns[x][y].SetGoalPosition(x,y);
    }
}