using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snap : MonoBehaviour
{
    //in note.cs, add this script to the note object if isEditor = true
    public float currentpos = 0;
    NoteData note;
    // Start is called before the first frame update
    void Start()
    {
        note = gameObject.GetComponent<NoteData>();
    }

    // Update is called once per frame
    void Update()
    {
        currentpos = note.Position;

    }
}
