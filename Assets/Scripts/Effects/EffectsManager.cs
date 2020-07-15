using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static globals;

public class EffectsManager : MonoBehaviour
{
    GameObject stageBase;
    GameObject lanesObject;
    GameObject comboBurstObject;
    GameObject hitEffectsObject;

    Canvas trackCanvas;
    Canvas backgroundCanvas;

    List<GameObject> keysList;
    List<GameObject> lanesList;

    Image comboBurstImage;
    Transform leftTarget;
    Transform rightTarget;

    Vector3 comboBurstOriginalPosition;

    void Start()
    {
        stageBase = GameObject.FindGameObjectWithTag(TAG_UI_STAGE_BASE);
        lanesObject = GameObject.FindGameObjectWithTag(TAG_UI_LANE_EFFECTS_OBJECT);
        comboBurstObject = GameObject.FindGameObjectWithTag(TAG_UI_COMBO_BURST_OBJECT);
        hitEffectsObject = GameObject.FindGameObjectWithTag(TAG_UI_HIT_EFFECTS_OBJECT);

        lanesList = new List<GameObject>();
        keysList = new List<GameObject>();

        //Key Effects
        for (int i = 0; i < stageBase.transform.childCount; i++)
        {
            keysList.Add(stageBase.transform.GetChild(i).GetChild(0).gameObject);
            keysList[i].SetActive(false);
        }

        //Lane Effects
        for (int i = 0; i < lanesObject.transform.childCount; i++)
        {
            lanesList.Add(lanesObject.transform.GetChild(i).gameObject);
            lanesList[i].SetActive(false);
        }

        //ComboBurst
        comboBurstImage = comboBurstObject.transform.GetChild(2).GetComponent<Image>();
        comboBurstOriginalPosition = comboBurstImage.transform.position;
        leftTarget = comboBurstObject.transform.GetChild(0);
        rightTarget = comboBurstObject.transform.GetChild(1);

        //Event subscription
        Controller.onKeyPress += KeyFlashOn;
        Controller.onKeyRelease += KeyFlashOff;

        Controller.onKeyPress += LaneFlashOn;
        Controller.onKeyRelease += LaneFlashOff;

        Conductor.onComboBurst += ComboBurst;
        Conductor.onNoteHit += StartHitEffect;

    }

    private void OnDestroy()
    {
        Controller.onKeyPress -= KeyFlashOn;
        Controller.onKeyRelease -= KeyFlashOff;

        Controller.onKeyPress -= LaneFlashOn;
        Controller.onKeyRelease -= LaneFlashOff;

        Conductor.onComboBurst -= ComboBurst;
        Conductor.onNoteHit -= StartHitEffect;

    }

    void StartHitEffect(int type, int lane)
    {
        int index = lane - 1;
        GameObject effectObject = hitEffectsObject.transform.GetChild(index).gameObject;
        if (effectObject == null)
            return;

        effectObject.SetActive(true);
        StartCoroutine(PlayHitEffect(effectObject));
    }

    IEnumerator PlayHitEffect(GameObject effectObject)
    {
        Animation animation = effectObject.GetComponent<Animation>();
        if (animation.clip != null)
            animation.Play();
        else
            yield break;

        while (animation.isPlaying)
        {
            yield return null;
        }
        effectObject.SetActive(false);

    }

    //Temporary implementation to prove concept.
    void KeyFlashOn(int column)
    {
        if (column > keysList.Count)
            return;

        keysList[column - 1].gameObject.SetActive(true);

    }

    void KeyFlashOff(int column)
    {
        if (column > keysList.Count)
            return;

        keysList[column - 1].gameObject.SetActive(false);

    }

    void LaneFlashOn(int column)
    {
        if (column > lanesList.Count)
            return;

        lanesList[column - 1].gameObject.SetActive(true);

    }

    void LaneFlashOff(int column)
    {
        if (column > lanesList.Count)
            return;

        lanesList[column - 1].gameObject.SetActive(false);
    }

    void NoteFlash()
    {

    }

    void ComboBurst()
    {
        float startTime = Time.time;
        Transform target;
        int roll = Random.Range(0, 100);

        if (roll > 50)
            target = leftTarget;
        else
            target = rightTarget;

        StartCoroutine(ComboBurstStart(target.position, startTime));

    }

    IEnumerator ComboBurstStart(Vector3 target, float startTime)
    {
        Color color = comboBurstImage.color;
        color.a = 1f;
        comboBurstImage.color = color;

        float distance = (target - comboBurstImage.transform.position).magnitude;
        while (distance > 0)
        {
            float step = (Time.time - startTime) * GAMEPLAY_EFFECT_COMBO_BURST_SPEED;
            comboBurstImage.transform.position = Vector3.Lerp(comboBurstImage.transform.position, target, step);
            distance = (target - comboBurstImage.transform.position).magnitude;
            yield return null;
        }

        StartCoroutine(ComboBurstFade());

    }

    IEnumerator ComboBurstFade()
    {
        for (float f = 1f; f >= 0; f -= GAMEPLAY_EFFECT_COMBO_BURST_FADE_RATE)
        {
            Color fadeColor = comboBurstImage.color;
            fadeColor.a = Mathf.Lerp(0, 1, f);
            comboBurstImage.color = fadeColor;
            yield return null;
        }

        comboBurstImage.transform.position = comboBurstOriginalPosition;

    }


}
