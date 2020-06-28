using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreEffect : MonoBehaviour
{
    public void SetScore(int score)
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = score.ToString();
    }

    public void SelfDestruct() { Destroy(gameObject); }
}
