using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static globals;

public class NoteQueuePool : MonoBehaviour
{
    Conductor conductor;
    Beatmap beatmap;

    List<Queue<NoteObject>> noteObjectQueueList;
    List<Queue<NoteObject>> longNoteObjectQueueList;

    List<Queue<NoteData>> noteDataQueueList;
    List<Queue<NoteData>> longNoteDataQueueList;

    private int noteMaxQueueSize = 30; //Per lane
    private int longNoteMaxQueueSize = 30;

    // Start is called before the first frame update
    void Start()
    {
        conductor = Conductor.instance;
        conductor.Play();

        beatmap = conductor.beatmap;
        noteObjectQueueList = new List<Queue<NoteObject>>();
        longNoteObjectQueueList = new List<Queue<NoteObject>>();

        noteDataQueueList = new List<Queue<NoteData>>();
        longNoteDataQueueList = new List<Queue<NoteData>>();

        for (int key = 1; key <= beatmap.KeyCount; key++)
        {
            noteObjectQueueList.Add(new Queue<NoteObject>());
            longNoteObjectQueueList.Add(new Queue<NoteObject>());

            SortNotesByLane(key, out Queue<NoteData> noteQueue, out Queue<NoteData> longNoteQueue);
            noteDataQueueList.Add(noteQueue);
            longNoteDataQueueList.Add(longNoteQueue);

        }

    }

    public void NextNote(int column)
    {
        ReInitializeNote(noteObjectQueueList[column - 1], noteDataQueueList[column - 1]);
    }

    public void NextLongNote(int column)
    {
        ReInitializeNote(longNoteObjectQueueList[column - 1], longNoteDataQueueList[column - 1]);
    }

    public void ReInitializeNote(Queue<NoteObject> objectQueue, Queue<NoteData> dataQueue)
    {
        if (objectQueue.Count > 0)
        {
            NoteObject obj = objectQueue.Dequeue();
            obj.ResetPosition();

            if (dataQueue.Count > 0)
            {
                NoteData data = dataQueue.Dequeue();
                obj.init(data);
                objectQueue.Enqueue(obj);

                return;

            }
            else
            {
                Destroy(obj.gameObject);
            }

        }

    }

    public void InitializeNoteQueue()
    {
        if (beatmap.Notes.Count <= 0)
            return;

        for (int i = 0; i < beatmap.KeyCount; i++)
        {
            Queue<NoteObject> objQueue = noteObjectQueueList[i];
            Queue<NoteObject> longObjQueue = longNoteObjectQueueList[i];

            int nNotes = noteDataQueueList[i].Count;
            int nLongNotes = longNoteDataQueueList[i].Count;

            for (int n = 0; n < noteMaxQueueSize; n++)
            {
                if (n >= nNotes)
                    break;

                NoteData noteData = noteDataQueueList[i].Dequeue();
                NoteObject noteObject = Instantiate(PREFAB_NOTE).GetComponent<NoteObject>();
                noteObject.init(noteData);
                objQueue.Enqueue(noteObject);
            }

            for (int ln = 0; ln < longNoteMaxQueueSize; ln++)
            {
                if (ln >= nLongNotes)
                    break;

                NoteData longNoteData = longNoteDataQueueList[i].Dequeue();
                NoteObject longNoteObject = Instantiate(PREFAB_NOTE_LONG).GetComponent<NoteObject>();
                longNoteObject.init(longNoteData);
                longObjQueue.Enqueue(longNoteObject);
            }

        }

    }

    void SortNotesByLane(int column, out Queue<NoteData> noteQueue, out Queue<NoteData> longNoteQueue)
    {
        noteQueue = new Queue<NoteData>();
        longNoteQueue = new Queue<NoteData>();

        for (int i = 0; i < beatmap.Notes.Count; i++)
        {
            NoteData note = beatmap.Notes[i];
            if (note.Column == column)
            {
                if (note.EndPosition > 0)
                    longNoteQueue.Enqueue(note);
                else
                    noteQueue.Enqueue(note);

            }
        }



    }

}
