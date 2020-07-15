using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static globals;

public class BeatBarController : MonoBehaviour
{
    public static BeatBarController instance { get; private set; }

    private List<GameObject> pooledObjects;
    //private GameObject objectToPool;
    private int amountToPool = 100; //amount to pool should scale to screen size

    private void InitPool(int amountToPool)
    {
        pooledObjects = new List<GameObject>();
        for(int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(PREFAB_BEATBAR);
            //obj.SetActive(false); prefab shd be flase alr
            pooledObjects.Add(obj);
        }
    }

    private GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
                return pooledObjects[i];
        }
        return null;
    }
    //get current song, init tempBm
    //have a 'first' and 'last' beat to signify which current beat is at the bottom and top of CAM
    //enum scroll direction > if scrolling up (normal play), 
    //if scrolling down (rewind)

    private Beatmap CurrentBm;
    private GameObject ObjBar;
    private List<GameObject> BeatBarList;
    private float BeatInterval, BeatIntervalFraction;
    //Number of SubBeats per main beat = RhythmFraction - 1
    public float RhythmFraction { get; set; } // 1/1 1/2 1/3 1/4 1/6 1/8 1/12 1/16 Number of beats per measure

    public BeatBar TopBeat { get; set; }
    public BeatBar BottomBeat { get; set; }

    private delegate void DirectionChange();
    private event DirectionChange OnDirectionChange;
    public enum Direction { Rewind = -1, Play = 1}
    private Direction m_CurrentDirection = Direction.Play;
    public Direction CurrentDirection
    {
        get { return m_CurrentDirection; }
        set
        {
            if (m_CurrentDirection == value) return;
            m_CurrentDirection = value;

            //OnDirectionChange?.Invoke();
        }
    }

    // Depending on rewind or play direction, CurrentBeatType will be based on the TopBeat's beat type
    private enum BeatType { MainBeat = 0, SubBeat }
    private BeatType CurrentBeatType = BeatType.MainBeat;

    // Whether it be Main or Sub beat, this is the song pos for the next beat
    public float NextBeatTime { get; set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // prepare the beatbar object pool
        InitPool(amountToPool);


        Conductor.instance.OnSongPosChange += SpawnBar;

        ObjBar = GameObject.FindGameObjectWithTag("Bar");
        BeatBarList = new List<GameObject>();
        BeatInterval = ObjBar.transform.position.y;
        RhythmFraction = 4; // 1/4
        InitBM();
        InitBeatBars();
    }

    void OnDisable()
    {
        Conductor.instance.OnSongPosChange -= SpawnBar;
    }

    private void SpawnBar()
    {
        /* if NextBeatTime >= songPos
            init next bar type at the top
            update TopBeat
            update NextBeatTime */
        if ((NextBeatTime - 2000) < Conductor.instance.songPosition)
            ActivateBeatBar();
    }

    private void InitBM()
    {
        //Debug.Log(Conductor.instance.beatmapFilePath);
        //Debug.Log(Conductor.instance.beatmapReader);
        Conductor.instance.LoadBeatmap();
        CurrentBm = Conductor.instance.beatmap;
    }

    public void InitBeatBars()
    {
        // set all the bars in the pool to inactive
        foreach (var beatbar in pooledObjects)
            beatbar.SetActive(false);

        BeatIntervalFraction = (float)CurrentBm.BeatLength / RhythmFraction;

        Vector3 barScreenPos = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().
            WorldToScreenPoint(new Vector3(ObjBar.transform.position.x, ObjBar.transform.position.y, ObjBar.transform.position.z));

        //barScreenPos.y -= PREFAB_NOTE.transform.localScale.y;

        int numOfMainBarAtInit = (int)((Screen.height - barScreenPos.y) * Conductor.instance.scrollSpeed / 1000 /
                        //(CurrentBm.getBeatLength() * Conductor.instance.scrollSpeed / 1000));
                        (BeatIntervalFraction * Conductor.instance.scrollSpeed / 1000));
        //Debug.Log("num of main bar at init = " + numOfMainBarAtInit);

        float oldNextBeatTime = NextBeatTime;

        NextBeatTime = oldNextBeatTime;
        for (int i = 0; i <= numOfMainBarAtInit; i++)
            ActivateBeatBar();
    }

    public void ActivateBeatBar() // set beatbar active = true and change beatbar propities accordinly
    {
        GameObject newBar = GetPooledObject();
        if (newBar != null)
        {
            newBar.SetActive(true);

            BeatBar newBeat = newBar.GetComponent<BeatBar>();
            newBeat.Position = NextBeatTime;

            NextBeatTime += BeatIntervalFraction;
        }
    }

    // Old InitBeatBars() and CreateBeatBars()
    //public void InitBeatBars()
    //{
    //    //foreach (var beatbar in BeatBarList)
    //    foreach (var beatbar in pooledObjects)
    //    {
    //        //Destroy(beatbar);
    //        beatbar.SetActive(false);
    //    }
    //    BottomBeat = new BeatBar { BeatIndex = -1 };
    //    CurrentDirection = Direction.Play;
    //    Debug.Log(CurrentBm.BeatLength);
    //    Debug.Log(RhythmFraction);

    //    BeatIntervalFraction = (float)CurrentBm.BeatLength / RhythmFraction;

    //    Vector3 barScreenPos = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().
    //        WorldToScreenPoint(new Vector3(ObjBar.transform.position.x, ObjBar.transform.position.y, ObjBar.transform.position.z));

    //    int numOfMainBarAtInit = (int)((Screen.height - barScreenPos.y) * Conductor.instance.scrollSpeed / 1000 /
    //                    //(CurrentBm.getBeatLength() * Conductor.instance.scrollSpeed / 1000));
    //                    (BeatIntervalFraction * Conductor.instance.scrollSpeed / 1000));
    //    Debug.Log("num of main bar at init = " + numOfMainBarAtInit);

    //    //numOfMainBarAtInit += 1;

    //    float oldNextBeatTime = NextBeatTime;

    //    //for (int i = 0; i < RhythmFraction; i++)
    //    //{
    //    //    NextBeatTime -= (/*num * */RhythmFraction * BeatIntervalFraction);
    //    //    float tempNextBeat2 = NextBeatTime;
    //    //    //hardcode to spawn more bars at the bottom
    //    //    for (int j = 0; j < RhythmFraction; j++)
    //    //    {
    //    //        CreateBeatBar();
    //    //    }
    //    //    NextBeatTime = tempNextBeat2;
    //    //}

    //    NextBeatTime = oldNextBeatTime;
    //    for(int i = 0; i <= numOfMainBarAtInit; i++)
    //    //while(TopBeat.BeatIndex <= numOfMainBarAtInit)
    //    {
    //        CreateBeatBar();
    //    }

    //    //(interval - Conductor.instance.songPosition) * Conductor.instance.scrollSpeed / 1000 
    //    //  + transform.localScale.y / 2 + Conductor.instance.getOffset())
    //}

    //private void CreateBeatBar() // 'create' is more like setActive = true and change beatbar propities accordinly
    //{
    //    GameObject newBar = GetPooledObject();
    //    if (newBar != null)
    //    {
    //        newBar.SetActive(true);

    //        BeatBar tempBeat = newBar.GetComponent<BeatBar>();
    //        tempBeat.Position = NextBeatTime;
    //        if (BottomBeat.BeatIndex >= 0)
    //        {
    //            if (TopBeat.SubBeatIndex >= RhythmFraction - 1) //if main beat
    //            {
    //                newBar.GetComponent<SpriteRenderer>().color = new Color(0.184f, 0.184f, 0.184f, 1.0f);
    //                tempBeat.BeatIndex = TopBeat.BeatIndex + (int)CurrentDirection;
    //                tempBeat.SubBeatIndex = 0;
    //            }
    //            else //if sub beat
    //            {
    //                tempBeat.BeatIndex = TopBeat.BeatIndex;
    //                tempBeat.SubBeatIndex = TopBeat.SubBeatIndex + (int)CurrentDirection;
    //            }
    //            //TopBeat = new BeatBar();
    //            TopBeat.BeatIndex = tempBeat.BeatIndex;
    //            TopBeat.SubBeatIndex = tempBeat.SubBeatIndex;
    //        }
    //        else
    //        {
    //            newBar.GetComponent<SpriteRenderer>().color = new Color(0.141f, 0.141f, 0.141f, 1.0f);
    //            tempBeat.BeatIndex = 0;
    //            tempBeat.SubBeatIndex = 0;

    //            TopBeat = new BeatBar
    //            {
    //                BeatIndex = tempBeat.BeatIndex,
    //                SubBeatIndex = tempBeat.SubBeatIndex
    //            };
    //            BottomBeat = new BeatBar
    //            {
    //                BeatIndex = TopBeat.BeatIndex,
    //                SubBeatIndex = TopBeat.SubBeatIndex
    //            };
    //        }

    //        //newBar.transform.position = new Vector2(2,
    //        //    (float)((NextBeatTime - Conductor.instance.songPosition) * 10 / 1000 +
    //        //    ObjBar.transform.position.y
    //        //    /* + transform.localScale.y / 2+ Conductor.instance.getOffset() */));
    //        //BeatBarList.Add(newBar);
    //        //print("Temp: " + tempBeat.BeatIndex + ", " + tempBeat.SubBeatIndex + ", " + tempBeat.Position);
    //        //print("Bot: " + BottomBeat.BeatIndex + ", " + BottomBeat.SubBeatIndex + ", " + BottomBeat.Position);
    //        //print("Top: " + TopBeat.BeatIndex + ", " + TopBeat.SubBeatIndex + ", " + TopBeat.Position);

    //        NextBeatTime += BeatIntervalFraction;
    //    }
    //}

    public void CalculateNextBeatTIme()
    {
        int numberofbeats = (int)(Conductor.instance.songPosition / BeatInterval);
        NextBeatTime = numberofbeats * BeatInterval + Conductor.instance.getOffset();
        //then depending on the beat and fraction, calculate how many main beat and sub beat NextBeatTime is on

        /*if (BottomBeat == null)
            BottomBeat = new BeatBar();
        BottomBeat.BeatIndex = (int)(numberofbeats / RhythmFraction);
        if (numberofbeats % RhythmFraction == 0)
            BottomBeat.SubBeatIndex = 0;*/

        //else
        //print("test: " + numberofbeats % RhythmFraction);
        //BottomBeat.SubBeatIndex = (int)(numberofbeats - (BottomBeat.BeatIndex * RhythmFraction));
        //print(BottomBeat.BeatIndex);
        //print(BottomBeat.SubBeatIndex);    }
    }
}
