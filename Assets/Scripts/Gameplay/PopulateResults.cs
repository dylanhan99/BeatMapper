using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static globals;

public class PopulateResults : MonoBehaviour
{
    GameObject maxObject;
    GameObject perfectObject;
    GameObject greatObject;
    GameObject goodObject;
    GameObject badObject;
    GameObject missObject;
    GameObject scoreObject;
    GameObject comboObject;
    GameObject accuracyObject;
    GameObject gradeObject;
    GameObject titleObject;
    GameObject versionObject;
    GameObject artistObject;

    GameObject keyObject; //to be redone in the .bm file itself in the future.

    enum Grade
    {
       SS, S, A, B, C, D
    }

    float[] accuracyList = { 100f, 95f, 90f, 80f, 70f,};

    // Start is called before the first frame update
    void Start()
    {
        maxObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_MAX);
        perfectObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_PERFECT); ;
        greatObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_GREAT); ;
        goodObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_GOOD); ;
        badObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_BAD); ;
        missObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_MISS);
        scoreObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_SCORE);
        comboObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_COMBO);
        accuracyObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_ACCURACY);
        gradeObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_GRADE);
        titleObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_TITLE);
        artistObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_ARTIST);
        versionObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_VERSION);

        keyObject = GameObject.Find(GAMEOBJECT_RESULTS_NAME_KEY); //to be redone in the .bm file itself in the future.
        
        maxObject.transform.GetChild(0).GetComponent<Text>().text = Conductor.instance.getnHitMax().ToString() + "x";
        perfectObject.transform.GetChild(0).GetComponent<Text>().text = Conductor.instance.getnHitPerfect().ToString() + "x";
        greatObject.transform.GetChild(0).GetComponent<Text>().text = Conductor.instance.getnHitGreat().ToString() + "x";
        goodObject.transform.GetChild(0).GetComponent<Text>().text = Conductor.instance.getnHitGood().ToString() + "x";
        badObject.transform.GetChild(0).GetComponent<Text>().text = Conductor.instance.getnHitBad().ToString() + "x";
        missObject.transform.GetChild(0).GetComponent<Text>().text = Conductor.instance.getnMiss().ToString() + "x";
        scoreObject.transform.GetChild(0).GetComponent<Text>().text = Conductor.instance.getScore().ToString();
        comboObject.transform.GetChild(0).GetComponent<Text>().text = Conductor.instance.getMaxCombo().ToString() + "x";
        titleObject.GetComponent<Text>().text = Conductor.instance.beatmap.Title.ToString();
        artistObject.GetComponent<Text>().text = Conductor.instance.beatmap.Artist.ToString();
        versionObject.GetComponent<Text>().text = Conductor.instance.beatmap.Version.ToString();

        //to be redone in the .bm file itself in the future.
        keyObject.GetComponent<Text>().text = Conductor.instance.beatmap.Title + Conductor.instance.beatmap.Version; 

        float accuracy = Conductor.instance.getAccuracy();
        accuracyObject.transform.GetChild(0).GetComponent<Text>().text = accuracy.ToString("0.00") + "%";

        int x = 0;
        for (int i = 0; i < accuracyList.Length; i++)
        {
            if (accuracy >= accuracyList[i])
                break;
            else
                x++;
        }

        Grade grade = (Grade)x;
        Text gradeText = gradeObject.GetComponentInChildren<Text>();
        switch (grade)
        {
            case Grade.SS:
                gradeText.text = "SS";
                break;
            case Grade.S:
                gradeText.text = "S";
                break;
            case Grade.A:
                gradeText.text = "A";
                break;
            case Grade.B:
                gradeText.text = "B";
                break;
            case Grade.C:
                gradeText.text = "C";
                break;
            case Grade.D:
                gradeText.text = "D";
                break;

        }
    }

}
