using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsRewindTracker : MonoBehaviour
{
    Text isRewindText;
    public GameObject scriptObj;

    private void Start()
    {
        isRewindText = GetComponent<Text>();
    }

    private void OnGUI()
    {
        if (scriptObj.GetComponent<BeatBarController>().CurrentDirection == 
            BeatBarController.Direction.Rewind)
            isRewindText.text = string.Format("IsRewind = true");
        else
            isRewindText.text = string.Format("IsRewind = false");
    }
}
