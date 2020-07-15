using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    Text scoreDisplay;
    private void Start()
    {
        scoreDisplay = GetComponent<Text>();
    }

    private void OnGUI()
    {
        scoreDisplay.text = Conductor.instance.getScore().ToString();
    }
}
