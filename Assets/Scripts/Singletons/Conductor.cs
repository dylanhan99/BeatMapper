using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static globals;
using static BeatmapScanner;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class Conductor : MonoBehaviour
{
    //public GameObject slider;
    //public GameObject TileParent;

    public static Conductor instance { get; private set; }
    public float scrollSpeed { get; private set; } = 3;

    //Log for debugging in runtime
    public List<string> Log { get; set; }/* = new List<string>()*/

    //public Controller controller;
    public delegate void SongPosChange();
    public event SongPosChange OnSongPosChange;

    public delegate void OnScrollSpeedChange();
    public static event OnScrollSpeedChange onScrollSpeedChange;

    public delegate void OnSongTimeChanged();
    public static event OnSongTimeChanged onSongTimeChanged;

    public delegate void OnNoteHit(int type, int lane);
    public static event OnNoteHit onNoteHit;

    public delegate void OnNoteMiss();
    public static event OnNoteMiss onNoteMiss;

    public delegate void OnComboBurst();
    public static event OnComboBurst onComboBurst;

    public delegate void OnSongStart();
    public static event OnSongStart onSongStart;

    public delegate void OnSongPause();
    public static event OnSongPause onSongPause;

    public string beatmapFilePath;
    public string audioFilePath;

    public Beatmap beatmap { get; private set; }
    public BeatmapReader beatmapReader { get; private set; }

    public int columns = 4;

    private float m_songPosition = 0;     //time passed since the start
    public float songPosition
    {
        get { return m_songPosition; }
        set
        {
            if (m_songPosition == value) return;
            m_songPosition = value;
            if (BeatBarController.instance != null)
            {
                OnSongPosChange?.Invoke();
            }
        }
    }
    public float songBeatPosition = 0; //number of beats since the start 
    public float crotchet { get; private set; } //seconds per beat
    public int bpm = 0;

    public double songDspTime = 0;
    public float skippedTime = 0;
    private double previousFrameDspTime = 0; //dspTime of previous frame
    public float NoteUpdateThreshold { get; private set; }

    private int playerOffset = 0; //player set offset (in settings)

    [SerializeField]
    private int offset = 0; //in ms
    public float overallDifficulty = 1;

    private int score = 0;
    private int scoreMax = 0;
    private int nHitMax = 0;
    private int nHitPerfect = 0;
    private int nHitGreat = 0;
    private int nHitGood = 0;
    private int nHitBad = 0;
    private int nMiss = 0;

    private int combo = 0;
    private int maxCombo = 0;
    private float accuracy = 0f;

    private bool paused;
    public bool started { get; private set; }
    private bool ended;

    [SerializeField]
    private bool isAutoEnabled = false;
    [SerializeField]
    public bool editorMode { get; set; } = false;

    private List<NoteData> notes = new List<NoteData>();
    private AudioSource song;

    public GameObject testPrefab;
    private List<BeatBar> beatBars = new List<BeatBar>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Log = new List<string>();
        //beatmapReader = GetComponent<BeatmapReader>();

        if (song == null)
            this.song = GetComponent<AudioSource>();

        //if (controller == null && SceneManager.GetActiveScene().name == "Gameplay") //temp
        //    controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();

        onNoteHit += CalculateHit;
        onNoteMiss += CalculateMiss;
        started = false;
        paused = false;

        //Log = new List<string>();
    }
    /*
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Level Loaded");
            Debug.Log(scene.name);
            Debug.Log(mode);
        }
    */
    void Update()
    {
        if (started)
        {
            songPosition = (float)(AudioSettings.dspTime - songDspTime) * song.pitch * 1000 + skippedTime; //Audio time reference point
            songBeatPosition = Mathf.Floor(songPosition / (crotchet * 1000));   //Position of song in beats

            //Correct for difference between previous frame, given that dspTime has not changed since last frame.
            if (AudioSettings.dspTime == previousFrameDspTime && !paused)
            {
                songPosition += Time.unscaledDeltaTime;
            }

            previousFrameDspTime = AudioSettings.dspTime;

            if (beatmap.Notes.Count > 0)
            {
                if (!ended & !editorMode)
                {
                    NoteData lastNote = beatmap.Notes[beatmap.Notes.Count - 1];
                    float finalPosition = lastNote.Position;
                    if (lastNote.EndPosition > 0)
                        finalPosition = lastNote.EndPosition;

                    if (songPosition > finalPosition)
                    {
                        ended = true;
                        StartCoroutine(EndGameplay());

                    }
                }

            }

        }

        //if (!song.isPlaying && songPosition > 0)
        //{
        //    if (editorMode)
        //        return;
        //    //Check if beatmap/song has ended
        //    if (Math.Floor(songPosition / 1000) == Math.Floor(song.clip.length))
        //    {
        //        //ResetConductor();
        //        //GameManager.ShowResultsPanel(); //Alternate results implementation
        //        ModeSelector.LoadResultsScreen();

        //    }

        //}

    }

    IEnumerator EndGameplay()
    {
        float timer = GAMEPLAY_END_DELAY;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        started = false;
        ModeSelector.LoadResultsScreen();

    }

    void ResetConductor()
    {
        started = false;
        paused = false;
        song.Stop();
        song.clip = null;

        //bpm = 0;
        songDspTime = 0;
        songPosition = 0;
        songBeatPosition = 0;
        skippedTime = 0f;
        previousFrameDspTime = 0;
        crotchet = 0;
        NoteUpdateThreshold = 0;
        offset = 0;
        overallDifficulty = 0;
        columns = 0;

        beatmap = null;
        notes = new List<NoteData>();
        beatBars = new List<BeatBar>();

        beatmapFilePath = "";
        audioFilePath = "";

    }

    public bool LoadBeatmap()
    {
        if (beatmapFilePath != "")
        {
            beatmapReader = new BeatmapReader();
            beatmap = beatmapReader.LoadBeatmap(beatmapFilePath);
            return true;
        }
        else
        {
            beatmap = new Beatmap();
            return false;
        }

    }

    public void ImportMP3File(string audioFilepath)
    {
        MP3Import mp3Importer = new MP3Import
        {
            //Audio file path
            mPath = audioFilepath,
            audioSource = getAudioSource()
        };
        mp3Importer.StartImport();

    }

    public void StartMusic()
    {
        onSongStart?.Invoke();
        song.Play();
    }

    public void Play()
    {
        score = 0;
        scoreMax = 0;
        nHitBad = 0;
        nHitGood = 0;
        nHitGreat = 0;
        nHitMax = 0;
        nHitPerfect = 0;
        nMiss = 0;

        combo = 0;
        maxCombo = 0;
        accuracy = 0f;
        songPosition = 0f;
        songDspTime = 0f;
        skippedTime = 0f;

        //string tempPath = PATH_BEATMAPS + currentMp3 + "/" + currentMp3;
        //song.clip = Resources.Load<AudioClip>(tempPath);
        if (!editorMode)
        {
            if (LoadBeatmap())
            {
                notes = beatmap.Notes;
                if (notes.Count > 0)
                {
                    offset = beatmap.Offset;
                    scoreMax = SCORE_MAX / notes.Count;
                }

            }

        }

        crotchet = 60f / bpm;
        started = true;
        ended = false;
        paused = false;
        float delay = crotchet * GAMEPLAY_BARS_TO_WAIT;
        if (editorMode)
            delay = 0f;

        //songDspTime = AudioSettings.dspTime + (float)(offset + playerOffset) / 1000 + delay;
        songDspTime = AudioSettings.dspTime + delay + playerOffset / 1000;
        //songDspTime = AudioSettings.dspTime + (float)offset / 1000;

        NoteUpdateThreshold = bpm / 60 * 1000;

        if (editorMode)
            StartMusic();
        else
            Invoke("StartMusic", delay);
            

    }

    public void Pause() 
    {
        if (editorMode)
        {
            if (!started)
            {
                Play();
                return;
            }

            onSongPause();

        }

        if (!paused)
        {
            AudioListener.pause = true;
            song.Pause();

            paused = true;
        }
        else
        {
            AudioListener.pause = false;
            song.UnPause();

            paused = false;
        }

    }

    public void Exit()
    {
        //Exit gameplay from Pause menu.
        song.Stop();
        AudioListener.pause = false;

        paused = false;
        started = false;
        ended = false;

        songPosition = 0f;
        songDspTime = 0f;
        song.time = 0f;

    }

    public void Stop()
    {
        AudioListener.pause = false;
        paused = false;
        started = false;
        song.Stop();

    }

    public void ScrollSpeedUp()
    {
        if (scrollSpeed < SCROLL_SPEED_MAX)
        {
            scrollSpeed++;
            onScrollSpeedChange();
        }

    }

    public void ScrollSpeedDown()
    {
        if (scrollSpeed > 1)
        {
            scrollSpeed--;
            onScrollSpeedChange();
        }

    }

    public void RaiseSongTimeChangedEvent()
    {
        onSongTimeChanged?.Invoke();
    }

    public void RaiseNoteHitEvent(int type, int lane)
    {
        if (onNoteHit != null)
            onNoteHit(type, lane);
    }

    public void RaiseNoteMissEvent()
    {
        if (onNoteMiss != null)
            onNoteMiss();
    }

    public void RaiseNoteReleaseEvent(int type, int lane)
    {
        if (onNoteHit != null)
            onNoteHit(type, lane);

        //CancelInvoke("ComboPlusPlus");
    }

    public void RaiseNoteHoldEvent(int type)
    {
        //InvokeRepeating("ComboPlusPlus", 0f, crotchet);

    }

    private void ComboPlusPlus()
    {
        combo++;
    }

    public float CalculateAccuracy()
    {
        if ((nMiss + nHitBad + nHitGood + nHitGreat + nHitPerfect + nHitMax) > 0)
        {
            float s = (SCORE_BAD * nHitBad + SCORE_GOOD * nHitGood + SCORE_GREAT * nHitGreat + SCORE_PERFECT * (nHitPerfect + nHitMax));
            float ms = 300 * (nMiss + nHitBad + nHitGood + nHitGreat + nHitPerfect + nHitMax);
            return s/ms * 100;
        }
        else
            return accuracy;
    }

    void CalculateHit(int type, int lane)
    {
        switch (type)
        {
            case HIT_TYPE_MISS:
                onNoteMiss();
                return;
            case HIT_TYPE_MAX:
                score += scoreMax;
                nHitMax++;
                break;
            case HIT_TYPE_PERFECT:
                score += SCORE_PERFECT;
                nHitPerfect++;
                break;
            case HIT_TYPE_GREAT:
                score += SCORE_GREAT;
                nHitGreat++;
                break;
            case HIT_TYPE_GOOD:
                score += SCORE_GOOD;
                nHitGood++;
                break;
            case HIT_TYPE_BAD:
                score += SCORE_BAD;
                nHitBad++;
                break;
            default:
                break;
        }

        combo++;
        if (combo > maxCombo)
            maxCombo = combo;

        if (combo % THRESHOLD_COMBO_BURST == 0)
            onComboBurst();

        accuracy = CalculateAccuracy();

    }

    void CalculateMiss()
    {
        combo = 0;
        nMiss++;
        accuracy = CalculateAccuracy();
    }

    public bool isPaused()
    {
        return paused;
    }

    public AudioSource getAudioSource()
    {
        return song;
    }
    public void setAudioSource(AudioSource song)
    {
        this.song = song;
    }

    public bool isPlaying()
    {
        if (song != null)
            return song.isPlaying;

        return false;
    }

    public int getScore()
    {
        return score;
    }
    public void setScore(int val)
    {
        score = val;
    }

    public int getScoreMax()
    {
        return scoreMax;
    }
    public void setScoreMax(int val)
    {
        scoreMax = val;
    }

    public int getnHitMax()
    {
        return nHitMax;
    }
    public void setnHitMax(int val)
    {
        nHitMax = val;
    }

    public int getnHitPerfect()
    {
        return nHitPerfect;
    }
    public void setnHitPerfect(int val)
    {
        nHitPerfect = val;
    }

    public int getnHitGreat()
    {
        return nHitGreat;
    }
    public void setnHitGreat(int val)
    {
        nHitGreat = val;
    }

    public int getnHitGood()
    {
        return nHitGood;
    }
    public void setnHitGood(int val)
    {
        nHitGood = val;
    }

    public int getnHitBad()
    {
        return nHitBad;
    }
    public void setnHitBad(int val)
    {
        nHitBad = val;
    }

    public int getnMiss()
    {
        return nMiss;
    }
    public void setnMiss(int val)
    {
        nMiss = val;
    }

    public int getCombo()
    {
        return combo;
    }

    public int getMaxCombo()
    {
        return maxCombo;
    }

    public void setCombo(int val)
    {
        combo = val;
    }

    public float getAccuracy()
    {
        return accuracy;
    }
    public void setAccuracy(float val)
    {
        accuracy = val;
    }

    public bool autoEnabled()
    {
        return isAutoEnabled;
    }

    public int getOffset()
    {
        return offset;
    }
}
