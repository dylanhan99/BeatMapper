using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTutorial : MonoBehaviour
{

    public delegate void OnTutorial();
    public static event OnTutorial onTutorial;

    public void toggleTutorial()
    {
        onTutorial();
    }
}
