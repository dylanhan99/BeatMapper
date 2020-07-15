using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static globals;

public class MessageManager : MonoBehaviour
{
    GameObject MessageUI;
    GameObject TutorialUI;
    Text MessageText;
    Image MessagePanel;

    Image TutorialPanel;
    List<Transform> steps;

    // Start is called before the first frame update
    void Start()
    {
        BeatLogger.onSave += ShowSaveMessage;
        ShowTutorial.onTutorial += toggleTutorial;

        MessageUI = GameObject.FindGameObjectWithTag(TAG_UI_MESSAGE_OBJECT);
        TutorialUI = GameObject.FindGameObjectWithTag(TAG_UI_TUTORIAL_OBJECT);

        MessagePanel = MessageUI.transform.GetChild(0).GetComponent<Image>();
        MessageText = MessagePanel.transform.GetChild(0).GetComponent<Text>();

        steps = new List<Transform>();
        //https://forum.unity.com/threads/iterating-child-game-objects-in-c.22860/
        for(int i = 0; i < TutorialUI.transform.childCount; i++)
        {
            steps.Add(TutorialUI.transform.GetChild(i));
            steps[i].gameObject.SetActive(false);
        }

        MessagePanel.gameObject.SetActive(false);

    }

    private void OnDisable()
    {
        Debug.Log("bleh");
        BeatLogger.onSave -= ShowSaveMessage;
        ShowTutorial.onTutorial -= toggleTutorial;
    }

    void ShowSaveMessage()
    {
        ShowMessage("Beatmap Saved.");
    }

    void ShowMessage(string text)
    {
        MessagePanel.gameObject.SetActive(true);
        MessageText.text = text;

        StartCoroutine(MessageFade());

    }

    void toggleTutorial()
    {
        //TutorialPanel.gameObject.SetActive(!TutorialPanel.gameObject.active);
        foreach(var i in steps)
        {
            i.gameObject.SetActive(!i.gameObject.active);
        }
    }

    IEnumerator MessageFade()
    {
        Color panelColor = MessagePanel.color;
        Color textColor = MessageText.color;

        for (float f = 1f; f >= 0; f -= UI_MESSAGE_FADE_RATE)
        {
            float alpha = Mathf.Lerp(0, 1, f);

            panelColor.a = alpha;
            textColor.a = alpha;

            MessagePanel.color = panelColor;
            MessageText.color = textColor;

            yield return null;
        }

        //Reset
        MessagePanel.gameObject.SetActive(false);
        panelColor.a = 1f;
        textColor.a = 1f;

        MessagePanel.color = panelColor;
        MessageText.color = textColor;
        MessageText.text = null;
    }

}
