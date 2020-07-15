using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static globals;

public class HitTracker : MonoBehaviour
{
    public Text text;
    public float fadeTime = 0.5f; //seconds
    private float timer = 0;

    private void OnEnable()
    {
        text = GetComponent<Text>();
        Conductor.onNoteHit += DisplayText;
        Conductor.onNoteMiss += DisplayMissText;

    }

    private void DisplayMissText()
    {
        if (text == null)
            return;

        timer = fadeTime;
        text.enabled = true;

        text.text = "MISS";
        text.color = Color.red;
    }

    private void DisplayText(int type, int lane)
    {
        if (text == null)
            return;

        timer = fadeTime;
        text.enabled = true;
        switch(type)
        {
            case HIT_TYPE_MAX:
                text.text = "MAX";
                text.color = Color.magenta;
                break;
            case HIT_TYPE_PERFECT:
                text.text = "PERFECT";
                text.color = Color.yellow;
                break;
            case HIT_TYPE_GREAT:
                text.text = "GREAT";
                text.color = Color.blue;
                break;
            case HIT_TYPE_GOOD:
                text.text = "GOOD";
                text.color = Color.green;
                break;
            case HIT_TYPE_BAD:
                text.text = "BAD";
                text.color = Color.grey;
                break;

            default:
                break;

        }

    }

    private void Update()
    {
        if (timer != 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                text.enabled = false;

        }

    }
}
