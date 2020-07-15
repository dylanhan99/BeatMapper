using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rewindornot : MonoBehaviour
{
    public GameObject scriptObj;

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name == "BtnRewind")
                {
                    if (scriptObj.GetComponent<BeatBarController>().CurrentDirection == BeatBarController.Direction.Rewind)
                        scriptObj.GetComponent<BeatBarController>().CurrentDirection = BeatBarController.Direction.Play;
                    else
                        scriptObj.GetComponent<BeatBarController>().CurrentDirection = BeatBarController.Direction.Rewind;
                }
            }
        }
    }
}
