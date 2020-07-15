using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class HighScore
{
    public int Score {get; set;}
    public string Name {get; set;}
    public string Mapname {get; set;}
    public int ID {get; set;}

    public HighScore (int id,string name, int score, string mapname)
    {
        this.Score =  score;
        this.Name = name;
        this.Mapname = mapname;
        this.ID = ID;
    }
}
