using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditorSliderControl : MonoBehaviour/*, IPointerDownHandler, IPointerUpHandler*/
{
    private Slider slider;

    void Start()
    {
        Conductor.instance.OnSongPosChange += MoveSlider;
        slider = GetComponent<Slider>();
        //slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        slider.minValue = 0;
        slider.maxValue = Conductor.instance.getAudioSource().clip.length * 1000;
    }

    void OnDisable()
    {
        Conductor.instance.OnSongPosChange -= MoveSlider;
    }

    private void MoveSlider()
    {
        if(Conductor.instance.isPlaying())
            slider.value = Conductor.instance.songPosition;
    }

    public void ValueChangeCheck()
    {
        //print("test");
        //if(Conductor.instance.isPlaying())
        //    Conductor.instance.Pause();
        ////else
        ////{
        //    //Conductor.instance.songPosition = slider.value;
        float delay = Conductor.instance.crotchet * globals.GAMEPLAY_BARS_TO_WAIT;
        float currentTime = Conductor.instance.getAudioSource().time;
        Conductor.instance.getAudioSource().time = slider.value / 1000;
        float newTime = Conductor.instance.getAudioSource().time;
        //Conductor.instance.songBeatPosition = Conductor.instance.songPosition 
        //                                        / (Conductor.instance.crotchet * 1000);
        Conductor.instance.skippedTime += (newTime - currentTime) * 1000;
        Conductor.instance.RaiseSongTimeChangedEvent();


        //Conductor.instance.songDspTime = AudioSettings.dspTime + 429 / 1000 + delay;

        BeatBarController.instance.CalculateNextBeatTIme();
        BeatBarController.instance.InitBeatBars();
        //}

    }

    public void playpause()
    {
        Conductor.instance.Pause();
    }

    public void Stop()
    {
        Conductor.instance.Stop();
    }

}
