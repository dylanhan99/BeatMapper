using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboTracker : MonoBehaviour
{
    Text comboText;
    private void Start()
    {
        comboText = GetComponent<Text>();
    }

    private void OnGUI()
    {
        try
        {
            comboText.text = Conductor.instance.getCombo().ToString();
        }
        catch
        {

        }
    }
}
