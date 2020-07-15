using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    FileManager fileManager;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = Conductor.instance.getAudioSource();

        if (fileManager == null)
            fileManager = GetComponent<FileManager>();

        //if (audioSource == null)
        //    audioSource = GetComponent<AudioSource>();

        //FileManager.onFileSelected += LoadAudio;

    }

    private void LoadAudio(string path)
    {
        StartCoroutine(AudioLoader(path));
        ModeSelector.LoadEditor();
    }

    private IEnumerator AudioLoader(string path)
    {
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else
        {
            audioSource.clip = AudioConverter.FromMp3Data(request.downloadHandler.data);
            audioSource.Play();
        }

    }


}
