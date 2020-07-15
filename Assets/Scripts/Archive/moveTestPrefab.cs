using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveTestPrefab : MonoBehaviour
{
    public float position = 0;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(6, (position - Conductor.instance.songPosition) * Conductor.instance.scrollSpeed / 1000 + transform.localScale.y / 2);
    }
}
