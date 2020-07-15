using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B83.Win32;

using System.IO;
using static globals;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class FileDragAndDrop : MonoBehaviour
{
    public delegate void OnBeatmapConverted();
    public static event OnBeatmapConverted onBeatmapConverted;

    private GameObject LoadPanel;

    void OnEnable ()
    {
        // must be installed on the main thread to get the right thread id.
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;

        List<GameObject> objectsInScene = new List<GameObject>();
        foreach(GameObject obj in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (obj.name == GAMEOBJECT_LOADING_PANEL)
            {
                LoadPanel = obj;
                break;
            }
        }
    }
    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    async void OnFiles(List<string> aFiles, POINT aPos)
    {
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.
        string str = "Dropped " + aFiles.Count + " files at: " + aPos + "\n\t" +
            aFiles.Aggregate((a, b) => a + "\n\t" + b);

        LoadPanel.SetActive(true);
        await ConvertToBM(aFiles);
        LoadPanel.SetActive(false);

        onBeatmapConverted?.Invoke(); //refresh

        Debug.Log(str);
        Conductor.instance.Log.Add(str);
    }

    private Task ConvertToBM(List<string> aFiles)
    { 
        return Task.Factory.StartNew(() => {

            OsuConvert osuConvert = new OsuConvert();
            foreach(string item in aFiles)
            {
                osuConvert.ConvertOsu(item);
            }
            
        });
    }

    /*private */void OnGUI()
    {
        if (Conductor.instance.Log == null)
            return;

        if (GUILayout.Button("clear log"))
            Conductor.instance.Log.Clear();
        foreach (var s in Conductor.instance.Log)
            GUILayout.Label(s);
    }
}
