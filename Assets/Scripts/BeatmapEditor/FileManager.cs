using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.InteropServices;
using static globals;

public class FileManager : MonoBehaviour
{
    //public delegate void OnFileSelected(string path);
    //public static event OnFileSelected onFileSelected;

    public string OpenExplorer()
    {
        System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
        ofd.InitialDirectory = Application.persistentDataPath + "\\" + PATH_BEATMAPS;
        ofd.Filter = "MP3 files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*";
        if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            //Get the path of specified file
            return ofd.FileName;

        }

        return "";
        //StartCoroutine(WaitForFile(ofd));

    }

    public void OnImportButtonClick()
    {
        string path = OpenExplorer();
        if (path == "")
            return;

        if (!Directory.Exists(PATH_TEMPORARY_ASSETS))
        {
            Directory.CreateDirectory(PATH_TEMPORARY_ASSETS);
        }

        //Copy audio file to temporary folder
        string tempFilePath = Path.Combine(PATH_TEMPORARY_ASSETS, Path.GetFileName(path));
        File.Copy(path, tempFilePath, true);

        Conductor.instance.audioFilePath = tempFilePath;
        Conductor.instance.ImportMP3File(tempFilePath);
        ModeSelector.LoadEditor();
        //onFileSelected(path);
    }

    //IEnumerator WaitForFile(System.Windows.Forms.OpenFileDialog ofd)
    //{
    //    string path = "failed";
    //    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
    //    {
    //        //Get the path of specified file
    //         path = ofd.FileName;

    //    }
    //    yield return System.Windows.Forms.DialogResult.OK;
    //    if (path == "failed")
    //        yield return null;

    //    onFileSelected(path);
    //}

}
