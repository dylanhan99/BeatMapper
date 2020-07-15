using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsEditTracker : MonoBehaviour
{
    Text isEditText;
    private void Start()
    {
        isEditText = GetComponent<Text>();
    }

    private void OnGUI()
    {
        if (Conductor.instance.editorMode)
            isEditText.text = string.Format("IsEdit = true");
        else
            isEditText.text = string.Format("IsEdit = false");
    }

}
