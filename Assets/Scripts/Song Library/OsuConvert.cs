using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Linq;
using System.Data;
using Mono.Data.Sqlite;
using static globals;

public class OsuConvert : MonoBehaviour
{
    private string connectionString;

    public enum ReadState { None, General, Meta, Difficulty, Events, Editor, Timing, Colour, HitObject };
    //private ReadState CurrentState = ReadState.None;

    public OsuConvert()
    {

    }

    private bool ReadBeatmapDataFromFile(string beatmapPath, out string[] FileData) // copy from BeatmapReader.cs
    {
        FileData = null;
        if (File.Exists(beatmapPath))
        {
            FileData = System.IO.File.ReadAllLines(beatmapPath);
            return true;
        }
        return false;
    }

    private void InsertMapDB(string mapKey, string mapName)
    {
        connectionString = PATH_DB + FILENAME_DB;

        using (IDbConnection dbConnection = new SqliteConnection("Data Source =" + connectionString + ";Version=3;"))
        {
            Debug.Log(connectionString);
            dbConnection.Open();

            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = string.Format("INSERT INTO songMap(mapKey, mapName) VALUES (\"{0}\",\"{1}\")", mapKey, mapName);

                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
                dbConnection.Close();
            }
        }
    }

    public string generateID()
    {
        return Guid.NewGuid().ToString("N");
    }

    public void ConvertOsu(string item) // item may be .osu file or a folder
    {
        BeatmapWriter BMWriter = new BeatmapWriter();

        FileAttributes attr = File.GetAttributes(item);
        // if it is a folder
        if (attr.HasFlag(FileAttributes.Directory)) // if item is a directory (a folder)
        {
            string folderName = new DirectoryInfo(item).Name; // get the name of the folder
            BMWriter.CreateNewFolder(folderName, out string folderCreated, true);
            //Conductor.instance.Log.Add("folderCreated = " + folderCreated);
            ConvertOsuFolder(item, folderCreated); // pass the .osu folder path into the function so that funciton can get each file

            return;
        }

        if (Path.GetExtension(item) == ".mp3") // if .mp3 file
        {
            string fileName = Path.GetFileNameWithoutExtension(item); // get the .osu file name from the entire path without the .osu extension
            BMWriter.CreateNewFolder(fileName, out string folderCreated, false); // folderCreated is the new folder in UwU\Beatmaps\...
            string copyFrom = item;
            string copyTo = Path.Combine(PATH_RESOURCES, folderCreated, "audio.mp3"/*Path.GetFileName(item)*/);
            File.Copy(copyFrom, copyTo, true);

            BMWriter.createEmptyFile(folderCreated, Path.GetFileName(item));

            return;
        }

        //if (Path.GetExtension(item) == OSU_FILE_EXTENSION) // if item is a .osu file
        //{
        //    string fileName = Path.GetFileNameWithoutExtension(item); // get the .osu file name from the entire path without the .osu extension
        //    BMWriter.CreateNewFolder(fileName, out string folderCreated, false); // folderCreated is the new folder in UwU\Beatmaps\...
        //
        //    if (ReadBeatmapDataFromFile(item, out string[] FileData)) // parse in the .osu file path to get back the FileData[]
        //        ConvertOsuMap(folderCreated, Path.GetFileNameWithoutExtension(item), FileData);
        //
        //    return;
        //}
    }

    public void ConvertOsuMap(string folderCreated, string fileName, string[] FileData)
    {
        //Converting .osu hit objects into .bm format
        ConvertHitObjects(FileData, out string[] ConvertedHitObjects); 

        //Write to Database
        InsertMapDB(generateID(),fileName);
            
        //Create and write file
        BeatmapWriter BMWriter = new BeatmapWriter();
        // folderCreated as the name suggests is the name of the folder to be created
        BMWriter.CreateFile(folderCreated, fileName, out string newFilePath); // newFilePath = %appdata%\fileName.bm
        BMWriter.WriteOsuOtherTags(FileData, newFilePath);
        BMWriter.WriteConvertedOsuHitObjects(ConvertedHitObjects, newFilePath);

        return;
    }

    public void ConvertOsuFolder(string path, string newFolder) // path = original .osu path, newFolder = folder path which would store the .bm files 
    {
        // loop through and handle each file accordingly
        if (Directory.Exists(path))
        {
            string[] FilesinFolder = Directory.GetFiles(path);

            foreach (string file in FilesinFolder)
            {
                //Conductor.instance.Log.Add("Loading....");
                //Conductor.instance.Log.Add(file);
                Debug.Log(file);

                //Conductor.instance.Log.Add(Path.GetExtension(file));
                if (Path.GetExtension(file) == OSU_FILE_EXTENSION)
                {
                    ////Check Num of keys before continuing. Must be a 4k map
                    //Conductor.instance.Log.Add(Path.Combine(path, file));
                    //Beatmap OsuFile = Conductor.instance.beatmapReader.LoadBeatmap(Path.Combine(path,file));

                    //if (OsuFile.KeyCount == 4)
                    //{
                    if (ReadBeatmapDataFromFile(file, out string[] FileData))
                        ConvertOsuMap(newFolder, Path.GetFileNameWithoutExtension(file), FileData); //create new .bm map with file name [1] at folder [0]
                    continue;
                    //}
                }

                if (Path.GetExtension(file) == ".mp3")
                {
                    string copyFrom = file; // each file in FilesInFolder is a full path + file name 
                    string copyTo = Path.Combine(PATH_RESOURCES, newFolder, Path.GetFileName(file));
                    File.Copy(copyFrom, copyTo, true);
                    continue;
                }
            }
        }
    }

    private void ConvertHitObjects(string[] FileData, out string[] ConvertedHitObjectData)
    {
        //List<string> log = Conductor.instance.Log;

        ReadState state = ReadState.None;
        int NumKeys = CheckNumKeys(FileData);
        List<string> ConvertedNotes = new List<string>();
        foreach(string s in FileData)
        {
            if (state != ReadState.HitObject)
            {
                if (s.Contains(DATATAG_HITOBJECTS))
                    state = ReadState.HitObject;
                continue;
            }
            if (isTag_blank_comment(s, ref state))
                continue;
            //x,y,time,type,hitSound,endTime:hitSample
            //   >
            //col,time,endTime
            string[] CurrentNoteData = s.Split(',');

            int.TryParse(CurrentNoteData[0], out int currentX);
            if (CalcColumn(NumKeys, currentX, out int columnNumber))
            {
                int.TryParse(CurrentNoteData[2], out int startTime);
                string[] endTimeData = CurrentNoteData[CurrentNoteData.Length - 1].Split(':');
                if (!int.TryParse(endTimeData[0], out int endTime))
                    continue; //return error herre
                string toAdd = string.Format("{0},{1},{2}", columnNumber, startTime, endTime);
                ConvertedNotes.Add(toAdd);

                //log.Add(toAdd + "\t");
            }
            else
                continue;
        }
        ConvertedHitObjectData = ConvertedNotes.ToArray();
    }

    private int CheckNumKeys(string[] FileData)
    {
        string[] numKeys = new string[2];
        foreach (string s in FileData)
        {
            if (s.Contains(OSU_VARNAME_NUMKEYS))
            {
                numKeys = s.Split(':');
                break;
            }
        }
        int.TryParse(numKeys[1], out int keys);
        print(keys + "k");
        return keys;
    }

    private bool CalcColumn(int keys, float currentX, out int columnNum)
    {
        columnNum = -1;

        int[] columns = new int[keys];
        float startingNum = OSU_BASE_X / keys;
        int interval = OSU_X_INTERVALS[keys - 1];
        for (int i = 0; i < keys; i++)
        {
            int Osu_X = (int)(Math.Floor(startingNum) + i * interval);
            //columns.Append(Osu_X);
            columns[i] = Osu_X;
            //print("Osu_X = " + Osu_X);
        }

        //string s = "";
        //foreach (int i in columns)
        //    s += i + ",";
        //print(s);

        if (columns.Any())
        {
            for(int i = 0; i < keys; i++) //(int column in columns)
            {
                if(currentX >= columns[i] - interval / 2 &&
                    currentX < columns[i] + interval / 2)
                {
                    columnNum = i + 1;
                    return true;
                }
            }
        }
        return false;
    }

    private bool isDataTag(string CheckString, string DataTag)
    {
        if (CheckString.Contains(DataTag))
            return true;
        return false;
    }

    public static bool isTag_blank_comment(string s, ref ReadState CurrentState)
    {
        if (s == "" || s.Contains("//"))
            return true;
        else if (s.Contains(DATATAG_GENERAL))
            CurrentState = ReadState.General;
        else if (s.Contains(DATATAG_EDITOR))
            CurrentState = ReadState.Editor;
        else if (s.Contains(DATATAG_METADATA))
            CurrentState = ReadState.Meta;
        else if (s.Contains(DATATAG_DIFFICULTY))
            CurrentState = ReadState.Difficulty;
        else if (s.Contains(DATATAG_EVENTS))
            CurrentState = ReadState.Events;
        else if (s.Contains(DATATAG_TIMINGPOINTS))
            CurrentState = ReadState.Timing;
        else if (s.Contains(DATATAG_COLOURS))
            CurrentState = ReadState.Colour;
        else if (s.Contains(DATATAG_HITOBJECTS))
            CurrentState = ReadState.HitObject;
        else
            return false;
        return true;
    }

}
