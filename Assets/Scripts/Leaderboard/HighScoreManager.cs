using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.UI;
using System.IO;
using static globals;


public class HighScoreManager : MonoBehaviour
{

    public Text enterName;
    public Text mapName;
    public Text playerScore ;

    //Connnection String
    private string connectionString;
    private List<HighScore> highScores = new List<HighScore>();

    public GameObject scorePrefab;

    public Transform scoreParent;

    // Start is called before the first frame update
    void Start()
    {
        string connectionDirectory = Application.persistentDataPath + "/Database/";
        connectionString = Application.persistentDataPath + "/Database/game.db";
        if (!Directory.Exists(connectionDirectory))
        {
            Directory.CreateDirectory(connectionDirectory);
        }

        if (!File.Exists(connectionString))
        {
            File.Create(connectionString);
        }
        CreateTable();
        //Debug.Log(connectionString);
        //InsertScore("asd","100","Gravity Falls Theme Song");
    }

   
    // Update is called once per frame
    void Update()
    {

    }

    private void CreateTable(){
        Debug.Log("Table Created0");
        using (IDbConnection dbConnection = new SqliteConnection("Data Source =" + connectionString + ";Version=3;"))
        {
            Debug.Log("Table Created1");
            dbConnection.Open();
            Debug.Log("Table Created1.4");
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                Debug.Log(connectionString);
                string sqlQuery = "CREATE TABLE IF NOT EXISTS songMap( mapKey STRING, mapName STRING); CREATE TABLE IF NOT EXISTS playerscore ( PlayerID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, name STRING, score STRING, mapname STRING, mapkey STRING );";
                Debug.Log(sqlQuery);
                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteReader();
                dbConnection.Close();
                Debug.Log("Table Created2");
            }
        }

    }

    public void GetScores(Text mapName)
    {
        if (!Conductor.instance.editorMode)
        {
            highScores.Clear();
            using (IDbConnection dbConnection = new SqliteConnection("Data Source =" + connectionString + ";Version=3;"))
            {
                dbConnection.Open();
                Debug.Log("GET SCORES");
                Debug.Log(mapName.text);
                using (IDbCommand dbCmd = dbConnection.CreateCommand())
                {
                    string sqlQuery = string.Format("SELECT * FROM playerscore WHERE mapname=\"{0}\" ORDER BY score DESC", mapName.text);
                    Debug.Log(sqlQuery);

                    dbCmd.CommandText = sqlQuery;

                    using (IDataReader reader = dbCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                highScores.Add(new HighScore(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3)));
                            }
                            catch (Exception ex)
                            {
                                Debug.Log("Error: " + ex.Message);
                                //Debug.Log("kaboom");
                            }
                        }

                        dbConnection.Close();
                        reader.Close();
                    }
                }
            }
        }
    }

    public string generateID()
    {
        return Guid.NewGuid().ToString("N");
    }
    
    private void InsertScore(string playerName, string playerScore, string mapName)
    {
        using (IDbConnection dbConnection = new SqliteConnection("Data Source =" + connectionString + ";Version=3;"))
        {
            dbConnection.Open();
            Debug.Log("INSERT SCORE");
            Debug.Log(playerName);
            Debug.Log(playerScore);
            Debug.Log(mapName);
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                Debug.Log("TESda");
                string sqlQuery = string.Format("INSERT INTO playerscore(name, score, mapname) VALUES (\"{0}\",\"{1}\",\"{2}\")", playerName, playerScore, mapName);
                Debug.Log(sqlQuery);
                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
                dbConnection.Close();
                Debug.Log("INSERT SCORE Close");
            }
        }

    }

    public void ShowScores()
    {
        if (!Conductor.instance.editorMode)
        {
            Transform scoreDisplay = GameObject.FindGameObjectWithTag(TAG_UI_LEADERBOARD_SCORES).transform;

            GetScores(mapName);

            foreach (GameObject score in GameObject.FindGameObjectsWithTag("Score"))
            {
                Destroy(score);
            }

            for (int i = 0; i < highScores.Count; i++)
            {
                GameObject tmpObject = Instantiate(scorePrefab, scoreDisplay);
                HighScore tmpScore = highScores[i];
                tmpObject.GetComponent<HighScoreScript>().SetScore(tmpScore.Name, tmpScore.Score.ToString(), "#" + (i + 1).ToString());
                //tmpObject.transform.SetParent(scoreParent); //SCORE Prefab is created but not under its parent
            }
        }
    }

    public void EnterName(){
        if (enterName.text != string.Empty)
        {
            InsertScore(enterName.text, playerScore.text, mapName.text);
            Debug.Log(enterName.text);
            Debug.Log(playerScore.text);
            Debug.Log(mapName.text);
            enterName.text = string.Empty;
        }
    }

}
