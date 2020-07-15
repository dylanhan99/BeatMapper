using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static globals;

public class Beatmap
{
    private List<NoteData> NotesList = new List<NoteData>();

    //Slightly modified .osu format
    //Descriptions taken from https://osu.ppy.sh/help/wiki/osu!_File_Formats/Osu_(file_format)
    //Some unused content is omitted

    //General
    public string AudioFilename {get; private set;}     //Location of the audio file relative to the current folder
    public int AudioLeadIn { get; private set; }        //Milliseconds of silence before the audio starts playing
    public int PreviewTime { get; private set; }        //Time in milliseconds when the audio preview should start
    public int Countdown { get; private set; }          //Speed of the countdown before the first hit object (0 = no countdown, 1 = normal, 2 = half, 3 = double)
    public string SampleSet { get; private set; }       //Sample set that will be used if timing points do not override it (Normal, Soft, Drum)
    public int CountdownOffset { get; private set; }    //Time in beats that the countdown starts before the first hit object

    //Editor
    public List<int> Bookmarks { get; private set; }     //Time in milliseconds of bookmarks
    public double DistanceSpacing { get; private set; }  //Distance snap multiplier
    public double BeatDivisor { get; private set; }      //Beat snap divisor
    public int GridSize { get; private set; }            //Grid size
    public double TimelineZoom { get; private set; }     //Scale factor for the object timeline

    //Meta Data
    public string Title { get; private set; }            //Romanised song title
    public string TitleUnicode { get; private set; }     //Song title
    public string Artist { get; private set; }           //Romanised song artist
    public string ArtistUnicode { get; private set; }    //Song artist
    public string Creator { get; private set; }          //Beatmap creator
    public string Version { get; private set; }          //Difficulty name
    public string Source { get; private set; }           //Original media the song was produced for
    public string[] Tags { get; private set; }           //Search terms
    public int BeatmapID { get; private set; }           //Beatmap ID
    public int BeatmapSetID { get; private set; }        //Beatmapset ID

    //Difficulty
    public double HPDrainRate { get; private set; }         //HP setting (0–10)
    public double OverallDifficulty { get; private set; }   //OD setting (0–10)
    public int KeyCount { get; private set; }               //Circle Size in .osu

    //Background and Video
    public bool hasVideo { get; private set; }              //Whether the beatmap has an accompany video file
    public string backgroundfileName { get; private set; }  //Name of video/background file
    public int xOffset { get; private set; }                //Pixels from centre of screen
    public int yOffset { get; private set; }

    //Timing (currently only reads the first line)
    public int Offset { get; private set; }                  // Offset before first beat in ms
    public double BeatLength { get; private set; }           // Beat length in ms
    public int TimeSignature { get; private set; }           // Beats per measure 3/4 or 4/4 timing. Value should be 3 or 4.
    public float BeatsPerSong { get; private set; }          // Total number of beats in particular song
    public int SubBeatsPerBeat { get; private set; }         // Number of sub beats per beat (according to fraction)
    public double BeatLengthFraction { get; private set; }   // Beat length in ms (according to fraction)
    public int Volume { get; private set; }

    public string filePath { get; private set; }
    public List<NoteData> Notes { get; private set; }

    public Beatmap(string path)
    {
        Notes = new List<NoteData>();
        filePath = path;

        //Defaults
        AudioLeadIn = 0;
        PreviewTime = -1;
        Countdown = 1; //normal speed
        SampleSet = "Normal";

    }

    public Beatmap()
    {
        Notes = new List<NoteData>();
    }

    public void setMetaData(Dictionary<string, string> metaDataDict)
    {
        if (metaDataDict == null)
            return;

        Title = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_TITLE);
        TitleUnicode = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_TITLEUNICODE);
        Artist = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_ARTIST);
        ArtistUnicode = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_ARTISTUNICODE);
        Creator = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_CREATOR);
        Version = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_VERSION);
        Source = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_SOURCE);

        string tags = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_TAGS);
        string id = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_BEATMAPID);
        string setid = TryGetValueFromKey(metaDataDict, DICTIONARY_KEY_BEATMAP_METADATA_BEATMAPSETID);

        if (tags != null)
            Tags = metaDataDict[DICTIONARY_KEY_BEATMAP_METADATA_TAGS].Split(',');

        if (id != null)
            BeatmapID = int.Parse(id);

        if (setid != null)
            BeatmapSetID = int.Parse(setid);

    }

    public void setGeneralData(Dictionary<string, string> generalDataDict)
    {
        if (generalDataDict == null)
            return;

        AudioFilename = TryGetValueFromKey(generalDataDict, DICTIONARY_KEY_BEATMAP_DATA_GENERAL_AUDIO_FILENAME);
        SampleSet = TryGetValueFromKey(generalDataDict, DICTIONARY_KEY_BEATMAP_DATA_GENERAL_SAMPLESET);

        int.TryParse(TryGetValueFromKey(generalDataDict, DICTIONARY_KEY_BEATMAP_DATA_GENERAL_AUDIO_LEADIN), out int audioLeadIn);
        int.TryParse(TryGetValueFromKey(generalDataDict, DICTIONARY_KEY_BEATMAP_DATA_GENERAL_PREVIEW_TIME), out int previewTime);
        int.TryParse(TryGetValueFromKey(generalDataDict, DICTIONARY_KEY_BEATMAP_DATA_GENERAL_COUNTDOWN_TYPE), out int countDown);
        int.TryParse(TryGetValueFromKey(generalDataDict, DICTIONARY_KEY_BEATMAP_DATA_GENERAL_COUNTDOWN_OFFSET), out int countdownOffset);

        AudioLeadIn = audioLeadIn;
        PreviewTime = previewTime;
        Countdown = countDown;
        CountdownOffset = countdownOffset;

    }

    public void setTiming(int offset, double beatLength, int timeSignature)
    {
        this.Offset = offset;
        this.BeatLength = beatLength;
        this.TimeSignature = timeSignature;
    }

    public void setDifficultyData(Dictionary<string, string> difficultyDataDict)
    {
        int.TryParse(TryGetValueFromKey(difficultyDataDict, DICTIONARY_KEY_BEATMAP_DATA_DIFFICULTY_KEYCOUNT), out int keyCount);
        double.TryParse(TryGetValueFromKey(difficultyDataDict, DICTIONARY_KEY_BEATMAP_DATA_HPDRAINRATE), out double drainRate);
        double.TryParse(TryGetValueFromKey(difficultyDataDict, DICTIONARY_KEY_BEATMAP_DATA_DIFFICULTY_OVERALLDIFFICULTY), out double overallDifficulty);

        KeyCount = keyCount;
        HPDrainRate = drainRate;
        OverallDifficulty = overallDifficulty;
    }

    public float getBeatsPerSong()
    {
        double nBeats = (Conductor.instance.getAudioSource().clip.length * 1000 - Offset) / BeatLength;
        return (float)nBeats;
    }

    public void getSubBeatsPerBeat()
    {
        //Number of sub beats per beat = fraction denominator - 1
    }

    public void getBeatLengthFraction()
    {
        //beatInterval according to fraction = beatLength / fraction denominator
    }

}
