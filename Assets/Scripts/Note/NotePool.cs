using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static globals;

public class NotePool : MonoBehaviour
{
    List<GameObject> noteObjectPool;
    List<GameObject> longNoteObjectPool;
    List<NoteData> noteList;
    public List<NoteObject> noteObjectList;

    Beatmap beatmap;
    int keyCount;
    int currentNoteIndex = 0;

    int poolSize = 30;
    // Start is called before the first frame update
    void Start()
    {
        Controller.onMouseShiftClick += CreateNewNote;

        noteObjectPool = new List<GameObject>();
        longNoteObjectPool = new List<GameObject>();

        noteList = new List<NoteData>();
        noteObjectList = new List<NoteObject>();

        beatmap = Conductor.instance.beatmap;
        noteList = beatmap.Notes;
        keyCount = beatmap.KeyCount;

        //InitializePool();

        for (int i = 0; i < noteList.Count; i++)
        {
            NoteObject noteObject;
            if (noteList[i].EndPosition > 0)
                noteObject = Instantiate(PREFAB_NOTE_LONG).GetComponent<NoteObject>();
            else
                noteObject = Instantiate(PREFAB_NOTE).GetComponent<NoteObject>();

            noteObject.init(noteList[i]);
            noteObjectList.Add(noteObject);

        }

    }

    void OnDestroy()
    {
        Controller.onMouseShiftClick -= CreateNewNote;
    }

    public void CreateNewNote(Vector2 position)
    {
        float songPosition = Conductor.instance.songPosition;
        float notePosition = songPosition + position.y * 100;

        //Proof of concept
        NoteObject noteObject = Instantiate(PREFAB_NOTE).GetComponent<NoteObject>();
        NoteData data = new NoteData(0, 1, notePosition, 0);
        noteObject.init(data);

    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject longNoteObject = Instantiate(PREFAB_NOTE_LONG);
            GameObject noteObject = Instantiate(PREFAB_NOTE);

            noteObject.SetActive(false);
            longNoteObject.SetActive(false);

            noteObjectPool.Add(noteObject);
            longNoteObjectPool.Add(longNoteObject);

        }
    }

    // Update is called once per frame
    void Update()
    {
        //float songPosition = Conductor.instance.songPosition;
        //for (int i = currentNoteIndex; i < currentNoteIndex + keyCount; i ++)
        //{
        //    float endPosition = noteList[currentNoteIndex].EndPosition;
        //    float notePosition = noteList[currentNoteIndex].Position;
        //    if (endPosition > 0)
        //    {
        //        if (songPosition > endPosition)
        //        {
        //            currentNoteIndex++;
        //        }

        //    }
        //    else
        //    {
        //        if (songPosition > notePosition)
        //        {
        //            currentNoteIndex++;
        //        }
        //    }


        //}


    }

}
