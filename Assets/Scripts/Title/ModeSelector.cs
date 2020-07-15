using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static globals;

public class ModeSelector : MonoBehaviour
{
    public void LoadPlayMenu()
    {
        StartCoroutine(LoadYourAsyncScene(SCENE_MENU_NAME));
        Conductor.instance.editorMode = false;
    }

    public void LoadEditorMenu()
    {
        StartCoroutine(LoadYourAsyncScene(SCENE_MENU_NAME));
        Conductor.instance.editorMode = true;
    }

    public void RestartGameplay()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void ExitGameplay()
    {
        Conductor.instance.Exit();
        LoadPlayMenu();
    }

    public void ExitEditor()
    {
        Conductor.instance.Exit();
        LoadEditorMenu();
    }

    public static void LoadEditor()
    {
        SceneManager.LoadSceneAsync(SCENE_EDITOR_NAME);
    }

    public void LoadTitle()
    {
        StartCoroutine(LoadYourAsyncScene(SCENE_TITLE_NAME));
        Conductor.instance.editorMode = false;
    }

    public void UnpauseOnClick()
    {
        Conductor.instance.Pause();
        GameManager.TogglePausePanel();
    }

    public static void LoadResultsScreen()
    {
        SceneManager.LoadScene(SCENE_RESULTS_NAME);
    }

    public IEnumerator LoadYourAsyncScene(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }

}
