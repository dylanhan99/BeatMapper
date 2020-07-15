using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class globals
{
    //PATH
    //public static string PATH_RESOURCES = "Assets/Resources/";
    public static string PATH_RESOURCES = Application.persistentDataPath + "/Beatmaps"; 
    //public static string PATH_DB = Application.persistentDataPath + "/insertname here.db"; 
    public static string PATH_TEMPORARY_ASSETS = Application.persistentDataPath + "/temp";
    public static string PATH_BEATMAPS = "Beatmaps/";
    public static string PATH_PREFABS = "Prefabs/";

    //.osu DATATAGS
    public static string /*OSU_*/DATATAG_GENERAL = "[General]";
    public static string /*OSU_*/DATATAG_EDITOR = "[Editor]";
    public static string /*OSU_*/DATATAG_METADATA = "[Metadata]";
    public static string /*OSU_*/DATATAG_DIFFICULTY = "[Difficulty]";
    public static string /*OSU_*/DATATAG_EVENTS = "[Events]";
    public static string /*OSU_*/DATATAG_TIMINGPOINTS = "[TimingPoints]";
    public static string /*OSU_*/DATATAG_COLOURS = "[Colours]";
    public static string /*OSU_*/DATATAG_HITOBJECTS = "[HitObjects]";

    //.osu HITOBJECTS X INTERVALS and MISC
    public static int[] OSU_X_INTERVALS = new int[9] 
    { 0, 256, 171, 128, 102, 86, 73, 64, 57 };
    public static int OSU_BASE_X = 256;
    public static string OSU_VARNAME_NUMKEYS = "CircleSize";
    public static string OSU_FILE_EXTENSION = ".osu";
    public static string BM_FILE_EXTENSION = ".bm";

    //BEATMAP DATATAGS
    //public static string DATATAG_GENERAL = "[General]";
    //public static string DATATAG_METADATA = "[MetaData]";
    //public static string DATATAG_EDITORDATA = "[EditorData]";
    //public static string DATATAG_TIMINGPOINTS = "[TimingPoints]";
    //public static string DATATAG_NOTEDATA = "[NoteData]";
    //public static string DATATAG_DIFFICULTYDATA = "[Difficulty]";

    //PREFABS
    public static GameObject PREFAB_NOTE = (GameObject)Resources.Load(PATH_PREFABS + "Note");
    public static GameObject PREFAB_NOTE_LONG = (GameObject)Resources.Load(PATH_PREFABS + "LongNote");
    public static GameObject PREFAB_BEATBAR = (GameObject)Resources.Load(PATH_PREFABS + "BeatBar");
    public static GameObject PREFAB_LINERENDER = (GameObject)Resources.Load(PATH_PREFABS + "LineRender");
    public static GameObject PREFAB_MENU_SONG_INFO_OBJECT = (GameObject)Resources.Load(PATH_PREFABS + "SongInfo"); 

    //HITWINDOW VALUES
    public static int HIT_WINDOW_MAX     = 15;
    public static int HIT_WINDOW_PERFECT = 30;
    public static int HIT_WINDOW_GREAT   = 50;
    public static int HIT_WINDOW_GOOD    = 70;
    public static int HIT_WINDOW_BAD = 100;

    //NOTE JUDGEMENT TYPES
    public const int HIT_TYPE_MAX          = 0;
    public const int HIT_TYPE_PERFECT      = 1;
    public const int HIT_TYPE_GREAT        = 2;
    public const int HIT_TYPE_GOOD         = 3;
    public const int HIT_TYPE_BAD          = 4;
    public const int HIT_TYPE_MISS         = -1;

    //SCORE VALUES
    public static int SCORE_MAX         = 1000000;      //Scoremax is calculated using the max score of 1000000 / numNotes
    public static int SCORE_PERFECT     = 300;
    public static int SCORE_GREAT       = 200;
    public static int SCORE_GOOD        = 100;
    public static int SCORE_BAD         = 50;

    //ARRAYS
    public static int[] ARRAY_HIT_WINDOWS = { HIT_WINDOW_MAX, HIT_WINDOW_PERFECT, HIT_WINDOW_GREAT, HIT_WINDOW_GOOD, HIT_WINDOW_BAD, TIMINGWINDOW };
    public static int[] ARRAY_HIT_TYPES = { HIT_TYPE_MAX, HIT_TYPE_PERFECT, HIT_TYPE_GREAT, HIT_TYPE_GOOD, HIT_TYPE_BAD, HIT_TYPE_MISS };

    public static int TIMINGWINDOW      = 200; //ms
    public static int EDITOR_OFFSET = 2000;

    //ANIMATIONS AND EFFECTS
    public static int THRESHOLD_COMBO_BURST = 100; //100 Combos
    public static float GAMEPLAY_EFFECT_COMBO_BURST_SPEED = 0.5f;

    //Alpha value per frame.
    public static float GAMEPLAY_EFFECT_COMBO_BURST_FADE_RATE = 0.1f;
    public static float UI_MESSAGE_FADE_RATE = 0.01f; 

    //KEY BINDINGS
    public static KeyCode KEY_SCROLL_SPEED_DECREASE = KeyCode.F3;
    public static KeyCode KEY_SCROLL_SPEED_INCREASE = KeyCode.F4;

    public static int SCROLL_SPEED_MAX = 50;

    //NOTE DEFAULTS
    public static int NOTE_DEFAULT_OFFSCREEN_POSITION_X = 100;
    public static int NOTE_DEFAULT_OFFSCREEN_POSITION_Y = 0;

    public static int NOTE_LONG_BODY_START_INDEX = 0;
    public static int NOTE_LONG_BODY_MIDDLE_INDEX = 1;
    public static int NOTE_LONG_BODY_END_INDEX = 2;

    public static float NOTE_MISS_DIM_FACTOR = 0.5f;
    public static float NOTE_MISS_OPACITY_FACTOR = 0.5f;

    //DEFAULT GAMEPLAY CONSTANTS
    public static int GAMEPLAY_BARS_TO_WAIT = 3;
    public static float GAMEPLAY_END_DELAY = 3f; //seconds

    //SCENE NAMES
    public static string SCENE_TITLE_NAME = "Title";
    public static string SCENE_MENU_NAME = "Menu";
    public static string SCENE_EDITOR_NAME = "Editor";
    public static string SCENE_GAMEPLAY_NAME = "Gameplay";
    public static string SCENE_RESULTS_NAME = "GameResults";

    //GAME TAGS
    public static string TAG_UI_STAGE_BASE = "StageBase";
    public static string TAG_UI_LANE_EFFECTS_OBJECT = "LaneEffectsUI";
    public static string TAG_UI_COMBO_BURST_OBJECT = "ComboBurst";
    public static string TAG_UI_MENU_BUTTON_IMPORT_PARENT = "Import";
    public static string TAG_UI_MENU_LEADERBOARD = "Leaderboard";
    public static string TAG_MANAGER_INPUT_MANAGER = "InputManager";
    public static string TAG_MANAGER_SCENE_MANAGER = "SceneManager";
    public static string TAG_MANAGER_OBJECT_NOTE_MANAGER = "NoteManager";
    public static string TAG_MANAGER_OBJECT_NOTE_RECORDER = "NoteRecorder";
    public static string TAG_UI_MESSAGE_OBJECT = "MessageUI";
    public static string TAG_UI_TUTORIAL_OBJECT = "TutorialUI";
    public static string TAG_UI_LEADERBOARD_SCORES = "ScoreDisplayUI";

    //GAMEOBJECT NAMES (For GameObject.Find())
    public static string GAMEOBJECT_RESULTS_NAME_MAX = "MAX";
    public static string GAMEOBJECT_RESULTS_NAME_PERFECT = "PERFECT";
    public static string GAMEOBJECT_RESULTS_NAME_GREAT = "GREAT";
    public static string GAMEOBJECT_RESULTS_NAME_GOOD = "GOOD";
    public static string GAMEOBJECT_RESULTS_NAME_BAD = "BAD";
    public static string GAMEOBJECT_RESULTS_NAME_MISS = "MISS";
    public static string GAMEOBJECT_RESULTS_NAME_SCORE = "ScoreLabel";
    public static string GAMEOBJECT_RESULTS_NAME_COMBO = "ComboLabel";
    public static string GAMEOBJECT_RESULTS_NAME_ACCURACY = "AccuracyLabel";
    public static string GAMEOBJECT_RESULTS_NAME_GRADE = "Grade";
    public static string GAMEOBJECT_RESULTS_NAME_TITLE = "SongTitle";
    public static string GAMEOBJECT_RESULTS_NAME_ARTIST = "SongArtist";
    public static string GAMEOBJECT_RESULTS_NAME_VERSION = "SongVersion";
    public static string GAMEOBJECT_RESULTS_NAME_KEY = "SongKey";

    //DICTIONARY KEYS
    //META DATA KEYS
    public static string DICTIONARY_KEY_BEATMAP_METADATA_TITLE = "Title";
    public static string DICTIONARY_KEY_BEATMAP_METADATA_TITLEUNICODE = "TitleUnicode";
    public static string DICTIONARY_KEY_BEATMAP_METADATA_ARTIST = "Artist";
    public static string DICTIONARY_KEY_BEATMAP_METADATA_ARTISTUNICODE = "ArtistUnicode";
    public static string DICTIONARY_KEY_BEATMAP_METADATA_CREATOR = "Creator";
    public static string DICTIONARY_KEY_BEATMAP_METADATA_VERSION = "Version";
    public static string DICTIONARY_KEY_BEATMAP_METADATA_SOURCE = "Source";
    public static string DICTIONARY_KEY_BEATMAP_METADATA_TAGS = "Tags";
    public static string DICTIONARY_KEY_BEATMAP_METADATA_BEATMAPID = "BeatmapID";
    public static string DICTIONARY_KEY_BEATMAP_METADATA_BEATMAPSETID = "BeatmapSetID";

    //GENERAL DATA KEYS
    public static string DICTIONARY_KEY_BEATMAP_DATA_GENERAL_AUDIO_FILENAME = "AudioFilename";
    public static string DICTIONARY_KEY_BEATMAP_DATA_GENERAL_AUDIO_LEADIN = "AudioLeadIn";
    public static string DICTIONARY_KEY_BEATMAP_DATA_GENERAL_PREVIEW_TIME = "PreviewTime";
    public static string DICTIONARY_KEY_BEATMAP_DATA_GENERAL_COUNTDOWN_TYPE = "Countdown";
    public static string DICTIONARY_KEY_BEATMAP_DATA_GENERAL_SAMPLESET = "SampleSet";
    public static string DICTIONARY_KEY_BEATMAP_DATA_GENERAL_COUNTDOWN_OFFSET = "CountdownOffset";

    //DIFFICULTY DATA KEYS
    public static string DICTIONARY_KEY_BEATMAP_DATA_HPDRAINRATE = "HPDrainRate";
    public static string DICTIONARY_KEY_BEATMAP_DATA_DIFFICULTY_KEYCOUNT = "CircleSize";
    public static string DICTIONARY_KEY_BEATMAP_DATA_DIFFICULTY_OVERALLDIFFICULTY = "OverallDifficulty";

    //Dictionary Helper
    public static string TryGetValueFromKey(Dictionary<string, string> dict, string key)
    {
        if (dict != null)
        {
            if (dict.ContainsKey(key))
                return dict[key];
        }
        return null;

    }

    public enum ReadState { None, General, Meta, Difficulty, Events, Editor, Timing, Colour, HitObject };

    //DATATAG Parse
    public static bool isTag_blank_comment(string s, ref ReadState CurrentState)
    {
        if (s == "" || s.Contains("//"))
            return true;
        else if (s.Contains(DATATAG_GENERAL))
            CurrentState = ReadState.General;
        else if (s.Contains(DATATAG_EDITOR))
            CurrentState = ReadState.Editor;
        else if (s.Contains(DATATAG_METADATA))
            CurrentState = ReadState.Meta;
        else if (s.Contains(DATATAG_DIFFICULTY))
            CurrentState = ReadState.Difficulty;
        else if (s.Contains(DATATAG_EVENTS))
            CurrentState = ReadState.Events;
        else if (s.Contains(DATATAG_TIMINGPOINTS))
            CurrentState = ReadState.Timing;
        else if (s.Contains(DATATAG_COLOURS))
            CurrentState = ReadState.Colour;
        else if (s.Contains(DATATAG_HITOBJECTS))
            CurrentState = ReadState.HitObject;
        else
            return false;
        return true;
    }

}
