using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///////////////// Run this script before running GemInterface! /////////////////

public class Gem : MonoBehaviour
{
    // Cached component refrences
    SpriteRenderer myRenderer;
    Animator animator;
    GemInterface gemInterface;

    [Tooltip("Color for shading the gem when it is selected")]
    [SerializeField] Color shade = Color.gray;
    [Tooltip("Numerical tag to identify the gem's type")]
    [SerializeField] int gemType = -1;
    [Space]
    [Tooltip("Speed at which the gem falls when others beneath it break")]
    [SerializeField] float fallSpeed = 4f;

    // State variables
    bool selected = false;
    bool moving = false;
    bool beingDestroyed = false;
    Vector3 goalPosition;

    void Start()
    {
        // Chace references
        myRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gemInterface = FindObjectOfType<GemInterface>();
    }

    void Update()
    {
        if (moving)
        {
            // Move gem gown to goal row
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                goalPosition,
                fallSpeed * Time.deltaTime
            );
            // Test for (aproximate) equality between position and goal
            if (transform.localPosition == goalPosition) { moving = false; }
            // If not done moving, notify the interface
            else { gemInterface.NotDoneMoving(); }
        }
        // If the gem is still running the animation for being destroyed
        else if (beingDestroyed) { gemInterface.NotDoneMoving(); }
    }

    public int GetGemType() { return gemType; }
    public int GetCol() { return Mathf.RoundToInt(transform.localPosition.x); }
    public int GetRow() { return Mathf.RoundToInt(transform.localPosition.y); }

    /// <summary>Set gem to move to the specified position</summary>
    /// <param name="x">Column to move the gem to</param>
    /// <param name="y">Row to move the gem to</param>
    public void SetGoalPosition(int x, int y)
    {
        goalPosition = new Vector3(x, y, 0);
        // Set gem to move down to that row
        moving = true;
    }

    /// <summary>Immediately move the gem to a new position in the board.</summary>
    /// <param name="x">Column to move the gem to</param>
    /// <param name="y">Row to move the gem to</param>
    public void MoveToCoordinate(int x, int y)
    {
        transform.localPosition = new Vector3(x, y, 0);
    }

    /// <summary>Triggers animation to destroy the gem.
    /// The game object is only destroyed after the animation</summary>
    public void DestroyGem()
    {
        // Trigger animation
        animator.SetTrigger("destroy");
        // Flag this gem is being destroyed
        beingDestroyed = true;
    }

    /// <summary>This method (actually) destroys the gameObject</summary>
    public void SelfDestruct() { Destroy(gameObject); }

    /// <summary>De-selects gem and applies color white to the Renderer.color</summary>
    public void SetNotSelected()
    {
        myRenderer.color = Color.white;
        selected = false;
    }

    /// <summary>Selects the gem to swap</summary>
    private void Select()
    {
        GemSelect answer = gemInterface.Select(this);
        // Shade the gem to signal it as selected
        if (answer == GemSelect.Select)
        {
            myRenderer.color = shade;
            selected = true;
        }
        // Unshade the gem / don´t change the gem's shading
        else { SetNotSelected(); }
    }

    void OnMouseDown() { Select(); }
}
