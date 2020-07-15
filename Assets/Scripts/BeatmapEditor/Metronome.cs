using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    private AudioClip audioMetronome;
    private float bpm;
    private float timeToNextBeat;
    private float pattern;
    private int beats; //internal count

    private float countDown;

    void Awake()
    {
    }

    void Start()
    {
        audioMetronome = Resources.Load<AudioClip>(""); //find a metranome sound
        bpm = Conductor.instance.bpm; //song.bm needs to have a "bpm" tag
        timeToNextBeat = 60 / bpm;
        countDown = timeToNextBeat;
    }

    void FixedUpdate()
    {
        countDown -= Time.deltaTime;

        if (countDown <= 0f)
        {
            //audioMetronome.//play audio
            //countDown = timeToNextBeat;
        }
    }
}
