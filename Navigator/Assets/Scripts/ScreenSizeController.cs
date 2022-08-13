using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeController : MonoBehaviour
{
    public RectTransform canvas;
    void Start()
    {
        float ratio = (float)Screen.height / Screen.width;
        float toHeight = (float)canvas.rect.width * ratio;

        float ortSize = (float)toHeight / 200f;
        Camera.main.orthographicSize = ortSize;
    }

}
