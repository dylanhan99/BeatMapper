using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteData
{
    public int ID { get; private set; }
    public int Column;
    public float Position;
    public float EndPosition;

    public NoteData(int id, int column, float position, float endPosition)
    {
        ID = id;
        Column = column;
        Position = position;
        EndPosition = endPosition;

    }

    public void UpdateID(int id)
    {
        ID = id;
    }

}
