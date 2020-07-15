using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static globals;

public class BeatLogger : MonoBehaviour
{
    private BeatmapWriter writer;
    private BeatmapReader reader;
    private Controller controller;
    private NotePool notePool;

    public delegate void OnSave();
    public static event OnSave onSave;

    float holdThreshold = 200; //ms
    float holdTime = 0;
    int NoteID = -1;

    bool recording;

    // Start is called before the first frame update
    void Start()
    {
        Controller.onKeyPress += RegisterHit;
        Conductor.onSongPause += UpdateNoteIDs;

        recording = false;

        writer = new BeatmapWriter();
        reader = new BeatmapReader();

        if (controller == null)
            controller = GameObject.FindGameObjectWithTag(TAG_MANAGER_INPUT_MANAGER).GetComponent<Controller>();

        if (notePool == null)
            notePool = GameObject.FindGameObjectWithTag(TAG_MANAGER_OBJECT_NOTE_MANAGER).GetComponent<NotePool>();

    }

    void OnDisable()
    {
        Controller.onKeyPress -= RegisterHit;
        Conductor.onSongPause -= UpdateNoteIDs;

    }

    public void ToggleRecording()
    {
        recording = !recording;
    }

    public void SaveBeatMap()
    {
        if (writer == null || reader == null)
        {
            Debug.Log("boom boom");
            return;
        }

        Beatmap beatmap = Conductor.instance.beatmap;

        string[] HitObjectData = new string[beatmap.Notes.Count];
        for (int i = 0; i < beatmap.Notes.Count; i++)
        {
            NoteData note = beatmap.Notes[i];
            HitObjectData[i] = note.Column.ToString() + "," + note.Position.ToString() + "," + note.EndPosition.ToString();
        }

        if (beatmap.filePath == null)
        {
            string audioFilename = beatmap.AudioFilename;
            string tempAudioFilePath = Conductor.instance.audioFilePath;

            if (string.IsNullOrEmpty(audioFilename))
                audioFilename = Path.GetFileName(tempAudioFilePath);

            string beatmapName = Path.GetFileNameWithoutExtension(tempAudioFilePath);
            string fileName = beatmapName;
            string fileDirectoryPath = PATH_RESOURCES + "/" + beatmapName;

            //New Beatmap
            if (writer.CreateFile(fileDirectoryPath, fileName, out string filePath))
            {
                string audioPath = Path.Combine(fileDirectoryPath, audioFilename);
                if (!File.Exists(audioPath))
                {
                    File.Move(tempAudioFilePath, audioPath);
                }
                File.Delete(tempAudioFilePath);

                //Write MetaData
                writer.writeGeneralData(filePath, audioFilename);
                writer.writeMetaData(filePath, beatmapName, "test" + "test", "test", "test", "0", "0");
                writer.writeTimingPointsData(filePath, beatmap.Notes[0].Position, 100, 4);
                writer.writeDifficultyData(filePath, 4, 8); //Default overall difficulty (8)
                writer.WriteConvertedOsuHitObjects(HitObjectData, filePath);
                Debug.Log("Done!");
            }

        }
        else
        {
            reader.ReadBeatmapDataFromFile(beatmap.filePath);
            writer.WriteOsuOtherTags(reader.getFileData(), beatmap.filePath);
            writer.WriteConvertedOsuHitObjects(HitObjectData, beatmap.filePath);
            
        }

        //Raise save event;
        onSave();

        //Re-import the file to update the reference in the editor
        //#if UNITY_EDITOR_WIN
        //        AssetDatabase.ImportAsset(filePath);
        //#endif
    }

    public void UpdateNoteIDs()
    {
        List<NoteData> dataList = Conductor.instance.beatmap.Notes;
        List<NoteObject> objectList = notePool.noteObjectList;
        for (int i = 0; i < dataList.Count; i++)
        {
            dataList[i].UpdateID(i);
            objectList[i].ID = i;

            if (dataList[i].ID != objectList[i].ID)
            {
                Debug.Log("Boomz");
                Debug.Break();
            }
        }
    }

    void RegisterHit(int column)
    {
        if (!recording)
            return;

        int position = (int)(Conductor.instance.songPosition);
        int noteIndex = -1;

        if (Conductor.instance.beatmap.Notes.Count > 0)
        {
            for (int i = 0; i < Conductor.instance.beatmap.Notes.Count; i++)
            {
                float notePosition = Conductor.instance.beatmap.Notes[i].Position;
                if (notePosition > position)
                {
                    NoteData note = new NoteData(i, column, position, 0);
                    Conductor.instance.beatmap.Notes.Insert(i, note);
                    noteIndex = i;
                    break;
                }

            }

        }

        if (noteIndex == -1)
        {
            noteIndex = Conductor.instance.beatmap.Notes.Count;
            NoteData note = new NoteData(noteIndex, column, position, 0);
            Conductor.instance.beatmap.Notes.Add(note);
        }

        NoteObject noteObject = Instantiate(PREFAB_NOTE).GetComponent<NoteObject>();
        noteObject.init(Conductor.instance.beatmap.Notes[noteIndex]);
        notePool.noteObjectList.Insert(noteIndex, noteObject);

        StartCoroutine(CheckHold(column, noteObject, noteIndex));

    }

    IEnumerator CheckHold(int column, NoteObject noteObject, int noteIndex)
    {
        float timer = holdThreshold / 1000;
        NoteData data = Conductor.instance.beatmap.Notes[noteIndex];

        while (controller.IsKeyDown(column))
        {
            if (timer > 0)
                timer -= Time.deltaTime;

            else
            {
                int songPosition = (int)Conductor.instance.songPosition;
                Destroy(noteObject.gameObject);

                noteObject = Instantiate(PREFAB_NOTE_LONG).GetComponent<NoteObject>();
                NoteData noteData = new NoteData(data.ID, data.Column, data.Position, songPosition);

                noteObject.init(noteData);
                StartCoroutine(RegisterHold(column, noteObject, noteIndex));

                break;
            }

            yield return null;
        }
    }

    IEnumerator RegisterHold(int column, NoteObject noteObject, int noteIndex)
    {
        while (controller.IsKeyDown(column))
        {
            int songPosition = (int)(Conductor.instance.songPosition);
            noteObject.EndPosition = songPosition;
            noteObject.UpdateLNTransform();
            yield return null;
        }

        NoteData noteData = Conductor.instance.beatmap.Notes[noteIndex];
        Conductor.instance.beatmap.Notes[noteIndex] =
            new NoteData(noteData.ID, noteData.Column, noteData.Position, noteObject.EndPosition);

        //Store indexes of overlapping notes.
        Stack<int> overlapStack = new Stack<int>();
        for (int i = noteIndex + 1; i < Conductor.instance.beatmap.Notes.Count; i++)
        {
            NoteData note = Conductor.instance.beatmap.Notes[i];
            if (note.Column != noteObject.Column)
                continue;

            float notePosition = note.Position;
            if (notePosition <= noteObject.EndPosition)
            {
                overlapStack.Push(i);
                continue;
            }
            else
                break;

        }

        while (overlapStack.Count > 0)
        {
            Conductor.instance.beatmap.Notes.RemoveAt(overlapStack.Pop());
        }


    }

}
