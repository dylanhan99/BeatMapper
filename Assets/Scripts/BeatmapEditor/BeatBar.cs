using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static globals;

public class BeatBar : MonoBehaviour
{
    private GameObject TileParent;

    //public int BeatIndex { get; set; } //main beat index
    //public int SubBeatIndex { get; set; } // 0 for main beat
    public float Position { get; set; } //Position in Song.

    //public BeatBar()
    //{
    //
    //}

    public void init(/*int BeatIndex, int SubBeatIndex, */float Position)
    {
        //this.BeatIndex = BeatIndex;
        //this.SubBeatIndex = SubBeatIndex;
        this.Position = Position;
    }

    void OnEnable()
    {
        TileParent = GameObject.FindGameObjectWithTag("TileParent");
        Conductor.instance.OnSongPosChange += MoveBar;
    }

    void OnDisable()
    {
        Conductor.instance.OnSongPosChange -= MoveBar;
    }

    void MoveBar()
    {
        float songPosition = Conductor.instance.songPosition;
        float threshold = (Conductor.instance.getAudioSource().clip.length / (Conductor.instance.bpm / 10) * 1000);

        if (songPosition + threshold > Position)
        {
            float subBeatLength = (float)Conductor.instance.beatmap.BeatLength / 4;
            transform.position = new Vector2(2, (Position - songPosition) * Conductor.instance.scrollSpeed / 1000 + transform.localScale.y /*+ subBeatLength * Conductor.instance.scrollSpeed / 1000*/);
            if (songPosition - 2000 > Position)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
