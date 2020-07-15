using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreScript : MonoBehaviour
{
    public GameObject Score;
    public GameObject ScoreName;
    public GameObject Rank;

    public void SetScore(string name, string score, string rank)
    {
        this.ScoreName.GetComponent<Text>().text = name;
        this.Score.GetComponent<Text>().text = score;
        this.Rank.GetComponent<Text>().text = rank;

    } 
}
