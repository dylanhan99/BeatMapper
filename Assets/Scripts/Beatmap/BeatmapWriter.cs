using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;
using System.Linq;
using static globals;

public class BeatmapWriter
{
    string[] fileData;

    public bool CreateNewFolder(string fileName, out string folderCreated, bool isFolder)
    {
        string path = PATH_RESOURCES;
        var directoryInfo = new System.IO.DirectoryInfo(path);
        string FolderToCreate = "";

        if (isFolder)
            FolderToCreate = fileName;
        else
            FolderToCreate = string.Format("{0} {1}", DirectoryLength(path), fileName);
         
        folderCreated = FolderToCreate;
        if (!CreateSubDirectory(FolderToCreate, directoryInfo))
            return false;
        return true;
    }

    public bool CreateSubDirectory(string PathToCreate, DirectoryInfo Location)
    {
        if (Directory.Exists(PathToCreate))
            return false;
        Location.CreateSubdirectory(PathToCreate);
        return true;
    }

    public int DirectoryLength(string path) //error handling?
    {
        var directoryInfo = new System.IO.DirectoryInfo(path);
        int directoryCount = directoryInfo.GetDirectories().Length;
        return directoryCount;
    }

    public bool CreateFile(string folderCreated, string fileName, out string outFilePath/*, string[] HitObjectData*/)
    {
        outFilePath = null;
        string FilePath = Path.Combine(PATH_RESOURCES, folderCreated, fileName); // %appdata%\UwU\Beatmaps\folderName\fileName
        //if (string.IsNullOrEmpty(Path.GetExtension(FilePath)))
        FilePath += BM_FILE_EXTENSION; // + .bm

        outFilePath = FilePath;
        if (!Directory.Exists(folderCreated))
        {
            Directory.CreateDirectory(Directory.GetParent(FilePath).ToString());
        }

        if (!File.Exists(FilePath))
        {
            using (StreamWriter writer = new StreamWriter(FilePath, append: true))  //create empty %appdata%\UwU\Beatmaps\folderName\fileName.bm
            {
                //writer.WriteLine(OSU_DATATAG_GENERAL);
                //writer.WriteLine(OSU_DATATAG_EDITOR);
                //writer.WriteLine(OSU_DATATAG_METADATA);
                //writer.WriteLine(OSU_DATATAG_DIFFICULTY);
                //writer.WriteLine(OSU_DATATAG_DIFFICULTY);
                //writer.WriteLine(OSU_DATATAG_TIMINGPOINTS);
                //writer.WriteLine(OSU_DATATAG_HITOBJECTS);
                writer.Close();
            }

            return true;
        }
        return false;
    }

    public void createEmptyFile(string folderCreated, string fileName) //to be used when import new mp3 file.
    {
        string fileLoc = Path.Combine(PATH_RESOURCES, folderCreated, fileName);
        CreateFile(folderCreated, Path.GetFileNameWithoutExtension(fileName), out string outFilePath);
        //Conductor.instance.Log.Add("============================================" + outFilePath);
        writeGeneralData(outFilePath, "audio.mp3");
        writeMetaData(outFilePath, Path.GetFileNameWithoutExtension(fileName), "//", "//", "//", "1", "1");
        writeDifficultyData(outFilePath, 4, 1); //4k is default for now, difficulty dosnt do anything for now so default at 1.
        writeTimingPointsData(outFilePath, 429, 186, 4);

        string[] dat = { };
        writeData(outFilePath, dat, DATATAG_HITOBJECTS);

    }

    public bool WriteOsuOtherTags(string[] FileData, string FilePath)
    {
        if (FileData != null)
        {
            //OsuConvert.ReadState state = OsuConvert.ReadState.None;
            List<string> FileDataList = new List<string>(FileData);
            OsuConvert.ReadState state = OsuConvert.ReadState.None;
            //foreach (string s in FileDataList)
            int StartIndex = -1;
            int Range = 0;

            for (int i = 0; i < FileData.Length; i++)
            {
                //Debug.Log("State =" + state.ToString());
                //Debug.Log(FileDataList[i]);
                OsuConvert.isTag_blank_comment(FileDataList[i], ref state);
                if (state != OsuConvert.ReadState.HitObject)
                    continue;
                if (StartIndex < 0)
                    StartIndex = i;
                Range++;

                //if (state == OsuConvert.ReadState.HitObject)
                //{
                    //Conductor.instance.Log.Add("Enter Readstate HitObject");
                //}
                //Conductor.instance.Log.Add(FileData[i]);
                //Debug.Log(FileData[i]);
            }
            //StartIndex += 1;
            FileDataList.RemoveRange(StartIndex, Range);
            //FileDataList.Remove(OSU_DATATAG_HITOBJECTS);
            string[] RemainingData = FileDataList.ToArray();

            //using (StreamWriter writer = new StreamWriter(FilePath, append: true))
            using (StreamWriter writer = new StreamWriter(FilePath))
            {
                for (int i = 0; i < RemainingData.Length; i++)
                {
                    writer.WriteLine(RemainingData[i]);
                }
                writer.Close();
            }
            return true;
        }
        return false;
    }

    public bool WriteConvertedOsuHitObjects(string[] HitObjectData, string FilePath)
    {
        if(HitObjectData != null)
        {
            //write to file
            if (File.Exists(FilePath))
            {
                using (StreamWriter writer = new StreamWriter(FilePath, append: true))
                {
                    writer.WriteLine(DATATAG_HITOBJECTS);
                    for (int i = 0; i < HitObjectData.Length; i++)
                    {
                        //Debug.Log(HitObjectData[i]);
                        writer.WriteLine(HitObjectData[i]);
                    }
                    writer.Close();
                }
                return true;
            }
        }
        return false; //error
    }

    private void writeData(string filePath, string[] data, string dataTag)
    {
        StreamWriter writer = new StreamWriter(filePath, true);
        writer.WriteLine(dataTag);
        for (int i = 0; i < data.Length; i++)
        {
            writer.WriteLine(data[i]);
        }
        writer.WriteLine();
        writer.Close();
    }

    public bool writeMetaData(string filePath, string title, string artist, string creator, string Version, string BeatmapID, string BeatmapSetID)
    {
        List<string> metaDataItemList = new List<string>(); //convert into temp List to allow edit
        metaDataItemList.Insert(0, "Title:" + title);
        metaDataItemList.Insert(1, "Artist:" + artist);
        metaDataItemList.Insert(2, "Creator:" + creator);
        metaDataItemList.Insert(3, "Version:" + Version);
        metaDataItemList.Insert(4, "BeatmapID:" + BeatmapID);
        metaDataItemList.Insert(5, "BeatmapSetID:" + BeatmapSetID);

        string[] metaData = metaDataItemList.ToArray();
        writeData(filePath, metaData, DATATAG_METADATA);
        return true;

    }

    public void writeDifficultyData(string filePath, int keyCount, int overallDifficulty)
    {
        List<string> difficultyDataItemList = new List<string>();
        difficultyDataItemList.Insert(0, "CircleSize:" + keyCount);
        difficultyDataItemList.Insert(1, "OverallDifficulty:" + overallDifficulty);

        string[] difficultyData = difficultyDataItemList.ToArray();

        writeData(filePath, difficultyData, DATATAG_DIFFICULTY);

    }

    public void writeGeneralData(string filePath, string audioFileName)
    {
        List<string> generalDataItemList = new List<string>();
        generalDataItemList.Insert(0, "AudioFilename: " + audioFileName);

        string[] generalData = generalDataItemList.ToArray();

        writeData(filePath, generalData, DATATAG_GENERAL);
    }

    public void writeTimingPointsData(string filePath, float offset, float bpm, int timing)
    {
        double bl = 60 / bpm * 1000;
        string[] timingPointsData = { string.Format("{0},{1},{2}", offset, bl, timing) };
        writeData(filePath, timingPointsData, DATATAG_TIMINGPOINTS);
    }

    public bool writeTimingPoints(string filePath, float offset, float bpm, int timing)
    {
        if (fileData != null)
        {
            List<string> tempFileData = new List<string>(fileData); //convert into temp List to allow edit
            double bl = 60 / bpm * 1000;
            int index = 0;
            while (index < tempFileData.Count)
            {
                if (tempFileData[index].Contains(DATATAG_TIMINGPOINTS)) //contains [TimingPoints]
                {
                    index += 1;
                    break;
                }
                index += 1;
            }
            if (index >= tempFileData.Count) // create [TimingPoints]
            {
                tempFileData.Add("");
                tempFileData.Add(DATATAG_TIMINGPOINTS);
                tempFileData.Add(string.Format("{0},{1},{2}", offset, bl, timing));
            }
            else
                tempFileData[index] = string.Format("{0},{1},{2}", offset, bl, timing);

            fileData = tempFileData.ToArray();

            //write to file
            if (File.Exists(filePath))
                File.Delete(filePath);

            StreamWriter writer = new StreamWriter(filePath);
            for (int i = 0; i < fileData.Length; i++)
            {
                writer.WriteLine(fileData[i]);
            }
            writer.Close();

            return true;
        }
        return false; //error
    }



}
