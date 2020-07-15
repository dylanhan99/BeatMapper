using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static globals;

public class tile : MonoBehaviour
{
    public float fraction; // 1/1 1/2 1/3 1/4 1/6 1/8 1/12 1/16
    List<BeatBar> beatBars = new List<BeatBar>();
    float interval;
    public GameObject bar;
    public GameObject TileParent;

    // Start is called before the first frame update
    void Awake()
    {
        //fraction = 1 / 2;
    }

    void Start()
    {
        bar = GameObject.FindGameObjectWithTag("Bar");
        TileParent = GameObject.FindGameObjectWithTag("pink");
        interval = bar.transform.position.y;
        populate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void populate()
    {
        fraction = 1 / 4;

        Beatmap beatmap = Conductor.instance.beatmap;
        if (beatmap == null)
            return;

        //beatmap.initFileData(Conductor.instance);
        //tempBm.initTimingPoints();

        for (int i = 0; i < beatmap.BeatsPerSong; i++)
        {
            Object.Instantiate(PREFAB_LINERENDER);
            print(fraction);
            print(interval);
            PREFAB_LINERENDER.GetComponent<BeatBar>().Position = interval;
            //testPrefab.transform.position = new Vector2(2, (float)((interval - Conductor.instance.songPosition) * Conductor.instance.scrollSpeed / 1000 + transform.localScale.y / 2 + Conductor.instance.getOffset()));
            LineRenderer lineRenderer = PREFAB_LINERENDER.GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            float x = TileParent.transform.position.x - TileParent.transform.localScale.x / 2;
            float y = interval;
            float z = TileParent.transform.localScale.z - 1;
            lineRenderer.SetPosition(0, new Vector3((float)x, (float)y, (float)z));
            x += TileParent.transform.localScale.x / 2;
            lineRenderer.SetPosition(1, new Vector3((float)x, (float)y, (float)z));
            lineRenderer.gameObject.transform.parent = TileParent.transform;
            //BeatBar newBeat = testPrefab.GetComponent<BeatBar>();

            //newBeat.init(interval);
            //beatBars.Add(newBeat);
            interval += (float)beatmap.BeatLength;
        }
    }
}
