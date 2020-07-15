using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongSpeedSliderControl : MonoBehaviour
{
    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 3;
    }

    public void sliderCheck()
    {
        float tempSongPos = Conductor.instance.songPosition;
        print("Bef - " + Conductor.instance.songDspTime);
        switch (slider.value)
        {
            case 0:
                Conductor.instance.getAudioSource().pitch = 0.25f;
                break;
            case 1:
                Conductor.instance.getAudioSource().pitch = 0.5f;
                break;
            case 2:
                Conductor.instance.getAudioSource().pitch = 0.75f;
                break;
            case 3:
                Conductor.instance.getAudioSource().pitch = 1f;
                break;

            default:
                break;
        }
        print("New - " + Conductor.instance.songDspTime);
        Conductor.instance.songPosition = tempSongPos;
        //GameObject.Find("SliderSongPos").GetComponent<EditorSliderControl>().ValueChangeCheck();

    }
}
