using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitManager : MonoBehaviour
{
    public void OnApplicationQuitClick()
    {
        Debug.Log("Game Quit Executed");
        Application.Quit();
    }
}
