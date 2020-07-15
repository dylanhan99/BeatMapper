using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static globals;
using static BeatmapScanner;

public class PopulateSongMenu : MonoBehaviour
{
    List<BeatmapFolder> beatmapFolderList;

    private RectTransform rectTransform;
    private GridLayoutGroup gridLayoutGroup;

    public delegate void OnRefreshBeatmaps();
    public static event OnRefreshBeatmaps onRefreshBeatmaps;

    void Start()
    {
        FileDragAndDrop.onBeatmapConverted += Populate;
        //Import button only visible in editor mode.
        if (Conductor.instance.editorMode)
        {
            GameObject.FindGameObjectWithTag(TAG_UI_MENU_BUTTON_IMPORT_PARENT).transform.GetChild(0).gameObject.SetActive(true);
            GameObject.FindGameObjectWithTag(TAG_UI_MENU_LEADERBOARD).SetActive(false);
        }
        else
        {
            GameObject.FindGameObjectWithTag(TAG_UI_MENU_BUTTON_IMPORT_PARENT).transform.GetChild(0).gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag(TAG_UI_MENU_LEADERBOARD).SetActive(true);
        }

        Populate();
    }

    void OnDisable()
    {
        FileDragAndDrop.onBeatmapConverted -= Populate;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            Populate();
    }

    void Populate()
    {
        refreshAllBeatmaps();
        if (beatmapFolderList != null)
        {
            onRefreshBeatmaps?.Invoke();
            foreach (BeatmapFolder folder in beatmapFolderList)
            {
                foreach(string beatmap in folder.beatmaps)
                {
                    //Check Num of keys before continuing. Must be a 4k map
                    Beatmap bm = new Beatmap(Path.Combine(folder.folderPath, beatmap));
                    BeatmapReader reader = new BeatmapReader();
                    bm.setDifficultyData(reader.getData(Path.Combine(folder.folderPath, beatmap), DATATAG_DIFFICULTY));

                    Conductor.instance.Log.Add(bm.KeyCount.ToString())
;                   if (bm.KeyCount == 4)
                    {
                        GameObject songInfoObject = Instantiate(PREFAB_MENU_SONG_INFO_OBJECT, transform, false);
                        PopulateSongInfo songInfo = songInfoObject.GetComponent<PopulateSongInfo>();
                        songInfo.beatmapFilePath = beatmap;
                        //songInfo.PopulateTextFields();
                    }
                }
            }
        }
        //setObjectPos();
    }

    //public bool setObjectPos()
    //{
    //    rectTransform = gameObject.GetComponent<RectTransform>();
    //    gridLayoutGroup = gameObject.GetComponent<GridLayoutGroup>();

    //    if (rectTransform != null && gridLayoutGroup != null)
    //    {
    //        gridLayoutGroup.padding.left = 190;
    //        gridLayoutGroup.padding.top = 10;
    //        rectTransform.position.Set(0, 0, 0);

    //        return true;
    //    }
    //    return false;
    //}

    public List<BeatmapFolder> getAllBeatmapNames()
    {
        return beatmapFolderList;
    }

    public void refreshAllBeatmaps() //kinda like osu f5 
    {
        //Debug.Log("test");
        string path = Application.persistentDataPath + "/" + PATH_BEATMAPS;
        if (Directory.Exists(path))
        {
            //allBeatmaps = getAllBeatmaps(PATH_RESOURCES + PATH_BEATMAPS);
            beatmapFolderList = GetAllBeatmaps(path);
#if UNITY_EDITOR_WIN
            //EditorUtility.OpenFilePanel("test", path, "mp3");
#endif

        }
        else
        {
            Debug.Log("Creating directory...");
            Directory.CreateDirectory(path);
            Debug.Log(Directory.Exists(path));
            Invoke("refreshAllBeatmaps", 2.0f);

        }

    }

}
