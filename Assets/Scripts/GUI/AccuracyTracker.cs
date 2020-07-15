using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccuracyTracker : MonoBehaviour
{
    Text accuracyText;
    private void Start()
    {
        accuracyText = GetComponent<Text>();
    }

    private void OnGUI()
    {
        accuracyText.text = string.Format("{0:0.00}%", Conductor.instance.getAccuracy());
    }
}
