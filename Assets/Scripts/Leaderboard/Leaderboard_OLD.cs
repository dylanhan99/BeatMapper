using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    public Text [] highScores;
    int [] highScoresValues;
    //string [] highScoresNames;

    // Start is called before the first frame update
    void Start()
    {
        highScoresValues = new int[highScores.Length];
        //highScoresNames = new string[highScoresNames.Length];

        for (int x =0; x < highScores.Length; x++){
            highScoresValues[x] = PlayerPrefs.GetInt("highScoresValues" + x);
            //highScoresNames[x] = PlayerPrefs.GetString("highScoresNames" + x);
        }
        DrawScores ();
    }

    void SaveScores(){
        for (int x =0; x < highScores.Length; x++){
            PlayerPrefs.SetInt("highScoresValues" + x, highScoresValues[x]);
            //PlayerPrefs.SetString("highScoresNames" + x, highScoresNames[x]);
            
        }        
    }

    public void CheckForHighScore(int _value, string __userName, string __songName){//Better to swap songName to songID
        for (int x = 0; x < highScores.Length; x++){
            if (_value > highScoresValues[x]){
                for (int y = highScores.Length - 1; y > x; y--){
                    highScoresValues [y] = highScoresValues[y - 1];
                    //highScoresNames [y] = highScoresNames[y - 1];
                }
                highScoresValues[x] = _value;
                //highScoresNames [x] = __userName;
                DrawScores ();
                SaveScores ();
                break;
            }
        }
    }

    void DrawScores(){
        for (int x =0; x < highScores.Length; x++){
            highScores[x].text = highScoresValues[x]. ToString ();
            //highScores[x].text = highScoresNames [x] + ":" + highScoresValues[x]. ToString ();
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
