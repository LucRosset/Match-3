using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deselector : MonoBehaviour
{
    // Cached references
    GemInterface gemInterface;

    void Start() { gemInterface = FindObjectOfType<GemInterface>(); }

    void OnMouseDown() { gemInterface.CancelGemSelection(); }
}
