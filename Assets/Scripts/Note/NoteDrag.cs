using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static globals;

public class NoteDrag : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    private float horizontalSnapThreshold; //prob dont need this. shd use the rhythm fraction and calculate how far away note is from initial point?
    private float verticalSnapThreshold;

    private float currentColumn;
    private float currentSongPos; // To adjust this variable based on where i drag the note to

    Camera sceneCamera;
    Vector3 barScreenPos;

    private enum newColumn { one = 1, two, three, four }
    private newColumn newCol = newColumn.one;

    void Start()
    {
        if (!Conductor.instance.editorMode)
            enabled = false;
        else
        {
            GameObject ObjBar = GameObject.FindGameObjectWithTag("Bar");
            sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            barScreenPos = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().
                WorldToScreenPoint(new Vector3(ObjBar.transform.position.x, ObjBar.transform.position.y, ObjBar.transform.position.z));

            horizontalSnapThreshold = 0.5f;
        }

    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            BeatLogger recorder = GameObject.FindGameObjectWithTag(TAG_MANAGER_OBJECT_NOTE_RECORDER).GetComponent<BeatLogger>();
            NoteObject noteObject = GetComponent<NoteObject>();

            Debug.Log(recorder);

            Conductor.instance.beatmap.Notes.RemoveAt(noteObject.ID);
            recorder.UpdateNoteIDs();
            Destroy(gameObject);

        }

    }

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }

    void OnMouseDrag()
    {
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
        //transform.position = cursorPosition;
        //Debug.Log(cursorPoint);
        //Debug.Log(cursorPosition);

        Vector3 newPos = new Vector3();
        newPos.y = cursorPosition.y;
        newPos.z = cursorPosition.z;
        // if mouse if withing the next column, snap to that column.
        if (cursorPosition.x < 2)
        {
            newPos.x = 1;
            newCol = newColumn.one;
        }
        else if (cursorPosition.x < 3)
        {
            newPos.x = 2;
            newCol = newColumn.two;
        }
        else if (cursorPosition.x < 4)
        {
            newPos.x = 3;
            newCol = newColumn.three;
        }
        else //(cursorPosition.x < 4.5)
        {
            newPos.x = 4;
            newCol = newColumn.four;
        }

        //WorldSpaceToSongTime(gameObject.transform);

        transform.position = newPos;
    }

    void OnMouseUp()
    {
        NoteObject noteObject = gameObject.GetComponent<NoteObject>();
        float noteLength = noteObject.EndPosition - noteObject.Position;

        noteObject.Position = gameObject.transform.position.y
            / Conductor.instance.scrollSpeed * 1000 + Conductor.instance.songPosition;
        noteObject.Column = (int)newCol;

        //comapre current note songpos to nearest subbeat 
        noteObject.Position = Mathf.Floor(noteObject.Position);
        float subBeatLength = (float)Conductor.instance.beatmap.BeatLength / 4;
        noteObject.Position -=
            noteObject.Position % subBeatLength;

        if (noteLength > 0)
            noteObject.EndPosition = noteObject.Position + noteLength;

        gameObject.transform.position = new Vector2(noteObject.Column, 
            (noteObject.Position - Conductor.instance.songPosition) * Conductor.instance.scrollSpeed / 1000 + PREFAB_BEATBAR.transform.localScale.y * 2 - PREFAB_NOTE.transform.localScale.y);

        print("SongPos = " + Conductor.instance.songPosition);
        print("NotePos = " + noteObject.Position);

        NoteData noteData = Conductor.instance.beatmap.Notes[noteObject.ID];
        noteData.Column = noteObject.Column;
        noteData.Position = noteObject.Position;
        noteData.EndPosition = noteObject.EndPosition;

        bool test = noteObject.Position == Conductor.instance.beatmap.Notes[noteObject.ID].Position;
        if (!test)
        {
            Debug.Log("Splat");
            Debug.Break();
        }

    }

    //on mouse release, snap note to whichever line is nearest
}