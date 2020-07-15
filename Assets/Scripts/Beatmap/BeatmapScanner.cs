using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using static globals;

public static class BeatmapScanner
{
    public struct BeatmapFolder
    {
        public string folderPath;
        public string[] beatmaps;
    }

    public static List<BeatmapFolder> GetAllBeatmaps(string rootDirectory)
    {
        string[] ListOfAllBeatmaps = Directory.GetDirectories(rootDirectory);
        List<BeatmapFolder> beatmapFolderList = new List<BeatmapFolder>();

        foreach (string folder in ListOfAllBeatmaps)
        {
            BeatmapFolder beatmapFolder = new BeatmapFolder() { folderPath = folder };
            beatmapFolder.beatmaps = RetrieveBeatmapFiles(folder);

            beatmapFolderList.Add(beatmapFolder);
        }

        return beatmapFolderList;
    }

    static string[] RetrieveBeatmapFiles(string directory)
    {
        string[] beatmapNames = Directory.GetFiles(directory, "*.bm");
        return beatmapNames;
    }

    public static string removePATH(string path, string full)
    {
        string newItem = full.Replace(path, "");
        return newItem;
    }
}
