using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScrollview : MonoBehaviour
{
    private RectTransform rectTransform;
    private GridLayoutGroup gridLayoutGroup;

    public bool setObject(GameObject obj)
    {
        rectTransform = obj.GetComponent<RectTransform>();
        gridLayoutGroup = obj.GetComponent<GridLayoutGroup>();

        if (rectTransform != null && gridLayoutGroup != null)
        {
            gridLayoutGroup.padding.left = 190;
            gridLayoutGroup.padding.top = 10;
            rectTransform.position.Set(0, 0, 0);

            return true;
        }
        return false;
    }
}
