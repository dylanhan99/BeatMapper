using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static void TogglePausePanel()
    {
        GameObject pauseObject = GameObject.FindGameObjectWithTag("PauseUI");
        if (pauseObject != null)
        {
            GameObject panel = pauseObject.transform.GetChild(0).gameObject;

            if (panel.activeSelf)
                panel.SetActive(false);
            else
                panel.SetActive(true);

        }

    }

    public static void ShowGameOverPanel()
    {
        GameObject gameOverObject = GameObject.FindGameObjectWithTag("GameOverUI");
        if (gameOverObject != null)
        {
            gameOverObject.transform.GetChild(0).gameObject.SetActive(true);

        }

    }

    //public static void ShowResultsPanel()
    //{
    //    GameObject resultsObject = GameObject.FindGameObjectWithTag("ResultsUI");
    //    if (resultsObject != null)
    //    {
    //        resultsObject.transform.GetChild(0).gameObject.SetActive(true);

    //    }

    //}

}
