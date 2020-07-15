using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class editornot : MonoBehaviour
{
    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name == "BtnPlay")
                {
                    Conductor.instance.editorMode = false;
                }
                if (hit.collider.gameObject.name == "BtnEdit")
                {
                    Conductor.instance.editorMode = true;
                }
            }
        }
    }
}
