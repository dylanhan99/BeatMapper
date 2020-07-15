using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static globals;

public class onClickSongInfo : MonoBehaviour
{
    PopulateSongInfo songInfo;

    public void LoadSong()
    {
        songInfo = GetComponent<PopulateSongInfo>();
        string audioFilepath = Directory.GetParent(songInfo.beatmapFilePath) + "\\" + songInfo.audioFilename;
        Conductor.instance.beatmapFilePath = songInfo.beatmapFilePath;

        if (audioFilepath.Substring(audioFilepath.Length - 4) == ".mp3")
        {
            Conductor.instance.ImportMP3File(audioFilepath);
        }

        if (Conductor.instance.getAudioSource().clip == null)
        {
            Debug.Log("Failed to load Audio!");
            return;
        }

        if (!Conductor.instance.editorMode)
            SceneManager.LoadScene("Gameplay");
        else
        {
            Conductor.instance.LoadBeatmap();
            SceneManager.LoadScene("Editor");
        }
    }

}
