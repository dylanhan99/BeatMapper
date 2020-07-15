using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static globals;

public class NoteJudgement : MonoBehaviour
{
    Controller controller;
    Beatmap beatmap;
    NoteQueuePool notePool;

    List<Queue<NoteData>> noteQueueList; //List of the queues of notes per column
    List<Queue<NoteData>> noteHoldQueueList; //List of long notes that are currently held down.

    // Start is called before the first frame update
    void Start()
    {
        Controller.onKeyPress += EvaluateHit;
        Controller.onKeyRelease += EvaluateRelease;

        if (controller == null)
            controller = GameObject.FindGameObjectWithTag(TAG_MANAGER_INPUT_MANAGER).GetComponent<Controller>();

        beatmap = Conductor.instance.beatmap;
        noteQueueList = new List<Queue<NoteData>>();
        noteHoldQueueList = new List<Queue<NoteData>>();

        for (int i = 1; i <= beatmap.KeyCount; i++)
        {
            SortNotesByLane(i, out Queue<NoteData> noteQueue);
            noteHoldQueueList.Add(new Queue<NoteData>());
            noteQueueList.Add(noteQueue);

        }

        notePool = GetComponent<NoteQueuePool>();
        notePool.InitializeNoteQueue();

    }

    private void OnDestroy()
    {
        Controller.onKeyPress -= EvaluateHit;
        Controller.onKeyRelease -= EvaluateRelease;
    }

    private void Update()
    {
        float songPosition = Conductor.instance.songPosition;
        for (int i = 0; i < beatmap.KeyCount; i++)
        {
            if (noteQueueList[i].Count > 0)
            {
                NoteData noteData = noteQueueList[i].Peek();
                if (songPosition > noteData.Position - HIT_WINDOW_MAX && Conductor.instance.autoEnabled())
                {
                    //Conductor.instance.Pause();

                    EvaluateHit(i + 1);
                    //controller.RaiseKeyPressEvent(i);
                    continue;
                }

                if (songPosition > noteData.Position + TIMINGWINDOW)
                {
                    Conductor.instance.RaiseNoteMissEvent();

                    if (noteQueueList[i].Peek().EndPosition > 0)
                    {
                        noteHoldQueueList[i].Enqueue(noteQueueList[i].Dequeue());
                    }
                    else
                    {
                        noteQueueList[i].Dequeue();
                        notePool.NextNote(i + 1);
                    }
                }
            }

            if (noteHoldQueueList[i].Count > 0)
            {
                float endPosition = noteHoldQueueList[i].Peek().EndPosition;
                if (songPosition > endPosition - HIT_WINDOW_MAX && Conductor.instance.autoEnabled())
                {
                    //Conductor.instance.Pause();

                    EvaluateRelease(i + 1);
                    //controller.RaiseKeyReleaseEvent(i + 1);
                    continue;
                }

                if (songPosition > endPosition + TIMINGWINDOW)
                {
                    Conductor.instance.RaiseNoteMissEvent();
                    noteHoldQueueList[i].Dequeue();
                    notePool.NextLongNote(i + 1);
                }
            }
        }
    }

    bool GetNoteQueueIndex(int column, out int index)
    {
        index = -1;
        for (int i = 0; i < noteQueueList.Count; i++)
        {
            if (noteQueueList[i].Peek().Column == column)
            {
                index = i;
                return true;
            }

        }
        return false;

    }

    void SortNotesByLane(int column, out Queue<NoteData> noteQueue)
    {
        noteQueue = new Queue<NoteData>();
        for (int i = 0; i < beatmap.Notes.Count; i++)
        {
            NoteData note = beatmap.Notes[i];
            if (note.Column == column)
                noteQueue.Enqueue(note);
        }
    }

    bool JudgeHit(float notePosition, out int hitType)
    {
        float songPosition = Conductor.instance.songPosition;
        hitType = HIT_TYPE_MISS;

        if (songPosition + TIMINGWINDOW < notePosition)
            return false;

        for (int i = 0; i < ARRAY_HIT_WINDOWS.Length; i++)
        {
            if (songPosition >= notePosition - ARRAY_HIT_WINDOWS[i] && songPosition <= notePosition + ARRAY_HIT_WINDOWS[i])
            {
                hitType = ARRAY_HIT_TYPES[i];
                return true;
            }

        }
        return false;

    }

    void EvaluateHit(int column)
    {
        int index = column - 1;
        if (noteHoldQueueList[index].Count > 0)
            return;
        if (noteQueueList[index].Count < 1)
            return;

        float position;

        NoteData note = noteQueueList[column - 1].Peek();
        if (note == null)
            return;

        position = note.Position;
        if (JudgeHit(position, out int hitType))
        {
            Conductor.instance.RaiseNoteHitEvent(hitType, column);
            if (note.EndPosition > 0)
            {
                Conductor.instance.RaiseNoteHoldEvent(hitType);
                noteHoldQueueList[index].Enqueue(noteQueueList[index].Dequeue());
            }
            else
            {
                noteQueueList[index].Dequeue();
                notePool.NextNote(column);
            }
        }

    }

    void EvaluateRelease(int column)
    {
        int index = column - 1;
        if (noteHoldQueueList[index].Count < 1)
            return;
        if (noteQueueList[index].Count < 1)
            return;

        float endPosition;

        NoteData note = noteHoldQueueList[index].Peek();
        if (note == null)
            return;

        endPosition = note.EndPosition;
        if (!(endPosition > 0))
            return;

        if (JudgeHit(endPosition, out int hitType))
        {
            Conductor.instance.RaiseNoteReleaseEvent(hitType, column);
            noteHoldQueueList[index].Dequeue();
            notePool.NextLongNote(column);
        }

    }

}
