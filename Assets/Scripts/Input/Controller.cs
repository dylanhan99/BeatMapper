using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static globals;

public class Controller : MonoBehaviour
{
    private KeyCode key1 = KeyCode.S;
    private KeyCode key2 = KeyCode.D;
    private KeyCode key3 = KeyCode.F;
    private KeyCode keyMid = KeyCode.Space;
    private KeyCode key4 = KeyCode.J;
    private KeyCode key5 = KeyCode.K;
    private KeyCode key6 = KeyCode.L;

    private List<KeyCode> mode4k_keys = new List<KeyCode>();
    private List<KeyCode> mode7k_keys = new List<KeyCode>();
    List<KeyCode> activeKeys = new List<KeyCode>();

    public delegate void OnKeyPress(int column);
    public static event OnKeyPress onKeyPress;

    public delegate void OnKeyRelease(int column);
    public static event OnKeyRelease onKeyRelease;

    public delegate void OnMouseShiftClick(Vector2 position);
    public static event OnMouseShiftClick onMouseShiftClick;


    //public delegate void OnKeyHold(int column);
    //public static event OnKeyHold onKeyHold;

    Conductor conductor;
    int numKeys;

    private void Awake()
    {
        conductor = Conductor.instance;
        if (conductor == null)
            gameObject.SetActive(false);
    }

    void Start()
    {
        numKeys = conductor.columns;

        mode4k_keys.Add(key2);
        mode4k_keys.Add(key3);
        mode4k_keys.Add(key4);
        mode4k_keys.Add(key5);

        mode7k_keys.Add(key1);
        mode7k_keys.Add(key2);
        mode7k_keys.Add(key3);
        mode7k_keys.Add(keyMid);
        mode7k_keys.Add(key4);
        mode7k_keys.Add(key5);
        mode7k_keys.Add(key6);

        switch (numKeys)
        {
            case 4:
                activeKeys = mode4k_keys;
                break;
            case 7:
                activeKeys = mode7k_keys;
                break;
            default:
                activeKeys = mode4k_keys;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        EvaluateKeyPress();

        if (!Conductor.instance.editorMode)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Conductor.instance.Pause();
                GameManager.TogglePausePanel();

            }
        }

        if (Conductor.instance.editorMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Conductor.instance.Pause();
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            if (Conductor.instance.editorMode)
            {
                BeatLogger logger = GameObject.FindGameObjectWithTag
                    (TAG_MANAGER_OBJECT_NOTE_MANAGER).GetComponent<BeatLogger>();
                logger.SaveBeatMap();
            }

        }

        //If Shift and Left Click
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            if (!Conductor.instance.editorMode)
                return;

            Vector2 position = Camera.main.ScreenToWorldPoint
                (new Vector2(Input.mousePosition.x, Input.mousePosition.y));

            onMouseShiftClick(position);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!Conductor.instance.isPlaying())
                Conductor.instance.Play();
        }

        if (Input.GetKeyDown(KEY_SCROLL_SPEED_INCREASE))
            Conductor.instance.ScrollSpeedUp();
        else if (Input.GetKeyDown(KEY_SCROLL_SPEED_DECREASE))
            Conductor.instance.ScrollSpeedDown();

    }

    void EvaluateKeyPress()
    {
        for (int i = 0; i < numKeys; i++)
        {
            if (Input.GetKeyDown(activeKeys[i]))
                RaiseKeyPressEvent(i + 1);

            if (Input.GetKeyUp(activeKeys[i]))
                RaiseKeyReleaseEvent(i + 1);
        }
        return;
    }

    public bool IsKeyDown(int column) //Is Key held down
    {
        if (Input.GetKey(activeKeys[column - 1]))
            return true;
        else
            return false;
            
    }

    private void RaiseKeyPressEvent(int column)
    {
        onKeyPress?.Invoke(column);
    }

    private void RaiseKeyReleaseEvent(int column)
    {
        onKeyRelease?.Invoke(column);
    }

    //private void RaiseKeyHoldEvent(int column)
    //{
    //    onKeyHold?.Invoke(column);
    //}

}
