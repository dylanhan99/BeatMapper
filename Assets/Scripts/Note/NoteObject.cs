using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static globals;

public class NoteObject : MonoBehaviour
{
    //Inspired by Osu!'s HitObject
    public int ID;
    public int Column;
    public float Position;
    public float EndPosition;

    private int holdType;
    private bool hold;

    private Transform bodyTransform; //Long Note Body
    private Transform endTransform; //Long Note End
    private Transform startTransform; //Long Note Start
    float bodySpriteSize;

    private float baseNoteHeight;

    //For dragging
    private Vector3 screenPoint;
    private Vector3 offset;


    public float getPosition()
    {
        return Position;
    }

    public void init(NoteData noteData)
    {
        ID = noteData.ID;
        Column = noteData.Column;
        Position = noteData.Position;
        EndPosition = noteData.EndPosition;

        if (EndPosition > 0)
        {
            baseNoteHeight = transform.transform.GetChild(NOTE_LONG_BODY_START_INDEX).localScale.y;

            bodyTransform = transform.GetChild(NOTE_LONG_BODY_MIDDLE_INDEX);
            bodySpriteSize = bodyTransform.GetComponent<SpriteRenderer>().sprite.rect.height / bodyTransform.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

            endTransform = transform.GetChild(NOTE_LONG_BODY_END_INDEX);
            startTransform = transform.GetChild(NOTE_LONG_BODY_START_INDEX);

            UpdateLNTransform();

        }

        ResetPosition();

    }

    private void Start()
    {

        //Temporary implementation for lane/column-based note colors for clearer reading.
        Color color = new Color();
        switch (Column)
        {
            case 1:
                color = new Color(0.0f, 0.42f, 0.678f, 1.0f);
                break;
            case 2:
                color = new Color(0.757f, 0.255f, 0.337f, 1.0f);
                break;
            case 3:
                color = new Color(0.757f, 0.255f, 0.337f, 1.0f);
                break;
            case 4:
                color = new Color(0.0f, 0.42f, 0.678f, 1.0f);
                break;
            default:
                color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                break;
        }

        if (EndPosition > 0)
        {
            //baseNoteHeight = transform.transform.GetChild(NOTE_LONG_BODY_START_INDEX).localScale.y;
            //bodyTransform = transform.GetChild(NOTE_LONG_BODY_MIDDLE_INDEX).transform;
            //endTransform = transform.GetChild(NOTE_LONG_BODY_END_INDEX);

            //UpdateLNTransform();

            SpriteRenderer[] sr = gameObject.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sr.Length; i++)
            {
                sr[i].color = color;
            }
            Conductor.onScrollSpeedChange += UpdateLNTransform;

        }
        else
        {
            baseNoteHeight = transform.localScale.y;
            gameObject.GetComponent<SpriteRenderer>().color = color;

        }

        //Positions the note offscreen.
        transform.position = new Vector2(-100, 0);

        if (Conductor.instance.editorMode)
        {
            Conductor.onSongTimeChanged += ResetPosition;
            Conductor.onSongStart += ResetPosition;
        }

    }

    private void OnDestroy()
    {
        Conductor.onScrollSpeedChange -= UpdateLNTransform;
        if (Conductor.instance.editorMode)
        {
            Conductor.onSongTimeChanged -= ResetPosition;
            Conductor.onSongStart -= ResetPosition;
        }

    }

    public void UpdateLNTransform() //Update position and scale of long notes to match new scroll speed
    {
        if (EndPosition > 0)
        {
            float songPosition = Conductor.instance.songPosition;
            endTransform.position = new Vector2
                (transform.position.x, transform.position.y + (EndPosition - Position) * Conductor.instance.scrollSpeed / 1000);

            bodyTransform.position = new Vector2
                (transform.position.x, transform.position.y + startTransform.localScale.y / 2);

            float distStartToEnd = (endTransform.position.y - startTransform.position.y);
            bodyTransform.localScale = new Vector2
                (bodyTransform.localScale.x, distStartToEnd / bodySpriteSize);

            BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
            collider.size = new Vector2(collider.size.x, (bodyTransform.localScale.y + endTransform.localScale.y) * bodySpriteSize);
            collider.offset = new Vector2(collider.offset.x, collider.size.y / 2);

        }
        else
            return;

    }

    private void Update()
    {
        if (Conductor.instance != null)
        {

            if (!Conductor.instance.started)
                return;

            if (Conductor.instance.isPaused())
                return;

            float songPosition = Conductor.instance.songPosition;
            if (songPosition > Position - Conductor.instance.NoteUpdateThreshold)
            {
                if (songPosition < Position + Conductor.instance.NoteUpdateThreshold || songPosition < EndPosition + Conductor.instance.NoteUpdateThreshold)
                {
                    //adjusts note position such that it is centered as though its object pivot is on the left side (x=0).
                    //transform.position = new Vector2(Column - transform.localScale.x / 2, (Position - songPosition) * Conductor.instance.scrollSpeed / 1000 + baseNoteHeight);
                    transform.position = new Vector2(Column, (Position - songPosition) * Conductor.instance.scrollSpeed / 1000);
                }
                else
                {
                    ResetPosition();
                }

            }

            if (Conductor.instance.editorMode)
            {

                return;
            }

        }
    }

    void LongNoteMissEffect()
    {
        SpriteRenderer[] sr = gameObject.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sr.Length; i++)
        {
            SpriteRenderer s = sr[i];
            s.color = new Color(s.color.r * (1 - NOTE_MISS_DIM_FACTOR), s.color.g * (1 - NOTE_MISS_DIM_FACTOR), s.color.b * (1 - NOTE_MISS_DIM_FACTOR), NOTE_MISS_OPACITY_FACTOR);
        }
    }

    public void ResetPosition()
    {
       transform.position = new Vector2(NOTE_DEFAULT_OFFSCREEN_POSITION_X, NOTE_DEFAULT_OFFSCREEN_POSITION_Y);
    }

}
