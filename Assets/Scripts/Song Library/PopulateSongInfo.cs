using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static globals;

public class PopulateSongInfo : MonoBehaviour
{
    public string audioFilename { get; private set; }
    public string beatmapFilePath { get; set; }

    private string title = "";
    private string artist = "";
    private string version = "";
    private string creator = "";

    private int keyCount;
    private double drainRate;
    private double overallDifficulty;

    public Text txtTitle;
    public Text txtArtist;
    public Text txtVersionCreator;
    public Text txtKey;

    private void Start()
    {
        PopulateSongMenu.onRefreshBeatmaps += DeleteThis;
        if (beatmapFilePath != null)
        {
            ReadBeatmapInfo();
            setSongInfoText();
        }
    }

    void DeleteThis()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        PopulateSongMenu.onRefreshBeatmaps -= DeleteThis;
    }



    //public void setBeatmapPath(string path)
    //{
    //    beatmapFilePath = path;
    //}

    //public void PopulateTextFields()
    //{
    //    if (beatmapFilePath != null)
    //    {
    //        ReadBeatmapInfo();
    //        setSongInfoText();
    //    }
    //}

    private void setSongInfoText()
    {
        if (title == "")
            return;

        txtTitle.text = title;
        txtArtist.text = artist;
        txtVersionCreator.text = version + " by " + creator;
        txtKey.text = title + version;
    }

    private void ReadBeatmapInfo()
    {
        BeatmapReader reader = new BeatmapReader();
        Dictionary<string, string> metaDataDict = reader.getData(beatmapFilePath, DATATAG_METADATA);
        Dictionary<string, string> generalDataDict = reader.getData(beatmapFilePath, DATATAG_GENERAL);
        Dictionary<string, string> difficultyDataDict = reader.getData(beatmapFilePath, DATATAG_DIFFICULTY);

        //META DATA
        title = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_TITLE);
        artist = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_ARTIST);
        creator = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_CREATOR);
        version = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_VERSION);
        audioFilename = TryGetValueFromKey(generalDataDict, DICTIONARY_KEY_BEATMAP_DATA_GENERAL_AUDIO_FILENAME);

        int.TryParse(TryGetValueFromKey(difficultyDataDict, DICTIONARY_KEY_BEATMAP_DATA_DIFFICULTY_KEYCOUNT), out keyCount);
        double.TryParse(TryGetValueFromKey(difficultyDataDict, DICTIONARY_KEY_BEATMAP_DATA_HPDRAINRATE), out drainRate);
        double.TryParse(TryGetValueFromKey(difficultyDataDict, DICTIONARY_KEY_BEATMAP_DATA_DIFFICULTY_OVERALLDIFFICULTY), out overallDifficulty);

    }

}
