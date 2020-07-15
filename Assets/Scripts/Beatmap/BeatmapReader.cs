using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using static globals;

public class BeatmapReader
{
    //private enum ReadState { None, General, Meta, Difficulty, Editor, Timing, Note };
    private ReadState readState = 0;

    private string[] fileData;

    public string[] getFileData()
    {
        return fileData;
    }

    public static bool ReadOsuFile(string FilePath, out string[] FileData)
    {
        FileData = null;
        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllLines(FilePath);
            return true;
        }
        return false;
    }

    public bool ReadBeatmapDataFromFile(string beatmapPath)
    {
        if (File.Exists(beatmapPath))
        {
            fileData = System.IO.File.ReadAllLines(beatmapPath);
            return true;
        }
        return false;
    }

    public Beatmap LoadBeatmap(string beatmapPath)
    {
        Debug.Log(beatmapPath);
        if (fileData == null)
        {
            if (!ReadBeatmapDataFromFile(beatmapPath))
                return null;
        }

        Beatmap beatmap = new Beatmap(beatmapPath);

        if (initNotesData(ref beatmap))
            Debug.Log("Notes Loaded successfully!");

        if (initTimingPoints(ref beatmap))
            Debug.Log("Timing Points parsed successfully!");

        beatmap.setMetaData(getData(beatmapPath, DATATAG_METADATA));
        beatmap.setGeneralData(getData(beatmapPath, DATATAG_GENERAL));
        beatmap.setDifficultyData(getData(beatmapPath, DATATAG_DIFFICULTY));

        return beatmap;

    }

    public Dictionary<string, string> getData(string beatmapPath, string dataTag)
    {
        Dictionary<string, string> DataDictionary;
        if (fileData == null)
            if (!ReadBeatmapDataFromFile(beatmapPath))
                return null;

        if (ParseData(out DataDictionary, dataTag))
            return DataDictionary;
        else
            return null;
    }

    private bool ParseData(out Dictionary<string, string> DataDictionary, string dataTag)
    {
        if (fileData != null)
        {
            DataDictionary = new Dictionary<string, string>();
            for (int i = 0; i < fileData.Length; i++)
            {
                ReadState state;
                if (!EvaluateReadState(dataTag, out state))
                    return false;

                if (fileData[i].Contains(dataTag))
                {
                    readState = state;
                    continue;
                }

                else if (readState == state)
                {
                    string[] dataItem = fileData[i].Split(':');

                    if (isTag_blank_comment(dataItem[0], ref readState))
                        continue;
                    DataDictionary.Add(dataItem[0], dataItem[1].Trim());
                }
                else
                    continue;
            }
            return true;
        }

        DataDictionary = null; //error
        return false;
    }

    private bool initNotesData(ref Beatmap beatmap)
    {
        if (fileData != null)
        {
            int noteID = 0;
            for (int i = 0; i < fileData.Length; i++)
            {
                if (fileData[i].Contains(DATATAG_HITOBJECTS))
                {
                    readState = ReadState.HitObject;
                    continue;
                }
                else if (readState == ReadState.HitObject) // Fill up noteData[]
                {
                    string[] noteData = fileData[i].Split(',');

                    if (isTag_blank_comment(noteData[0], ref readState))
                        continue;

                    int column = int.Parse(noteData[0]);
                    float position = float.Parse(noteData[1]);
                    float endPosition = float.Parse(noteData[2]);

                    if (endPosition < position)
                    {
                        endPosition = 0;
                    }

                    NoteData note = new NoteData(noteID, column, position, endPosition);
                    beatmap.Notes.Add(note);
                    noteID++;

                }
                else
                    continue;
            }
            return true;
        }
        return false; //error
    }

    private bool initTimingPoints(ref Beatmap beatmap)
    {
        if (fileData != null)
        {
            for (int i = 0; i < fileData.Length; i++)
            {
                if (fileData[i].Contains(DATATAG_TIMINGPOINTS))
                {
                    readState = ReadState.Timing;
                    continue;
                }
                else if (readState == ReadState.Timing) // Fill up offset, beatLength, timing  
                {
                    string[] timingPoints = fileData[i].Split(',');

                    int offset = int.Parse(timingPoints[0]);
                    double beatLength = double.Parse(timingPoints[1]);
                    int timeSignature = int.Parse(timingPoints[2]);
                    beatmap.setTiming(offset, beatLength, timeSignature);

                    break; //Break, currently not in scope to implement multiple timing points.
                }
                else
                    continue;
            }
            return true;
        }
        return false; //error
    }

    private bool EvaluateReadState(string dataTag, out ReadState state)
    {
        state = ReadState.None;

        if (dataTag == DATATAG_GENERAL)
            state = ReadState.General;
        else if (dataTag == DATATAG_EDITOR)
            state = ReadState.Editor;
        else if (dataTag == DATATAG_METADATA)
            state = ReadState.Meta;
        else if (dataTag == (DATATAG_DIFFICULTY))
            state = ReadState.Difficulty;
        //else if (dataTag == DATATAG_EVENTS)
        //    state = ReadState.Events;
        else if (dataTag == DATATAG_TIMINGPOINTS)
            state = ReadState.Timing;
        //else if (dataTag == DATATAG_COLOURS)
        //    state = ReadState.Colour;
        //else if (dataTag == DATATAG_HITOBJECTS)
        //    state = ReadState.HitObject;


        if (state == ReadState.None)
            return false;
        else
            return true;

    }

}
