using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float ZoomMax;
    public float ZoomMin;
    public float Sensitivity;

    private Camera camera;

    private Touch touchA;
    private Touch touchB;

    private Vector2 touchADirection;
    private Vector2 touchBDirection;

    private float dstBtwTouchesPositions;
    private float dstBtwTouchesDirections;
    private float zoom;

    void Awake()
    {      
        camera = Camera.main;
    }

    void Update()
    {
        if(Input.touchCount == 2)
        {
            touchA = Input.GetTouch(0);
            touchB = Input.GetTouch(1);

            touchADirection = touchA.position - touchA.deltaPosition;
            touchBDirection = touchB.position - touchB.deltaPosition;

            dstBtwTouchesPositions = Vector2.Distance(touchA.position, touchB.position);
            dstBtwTouchesDirections = Vector2.Distance(touchADirection, touchBDirection);
            zoom = dstBtwTouchesPositions - dstBtwTouchesDirections;
            var currentZoom = camera.orthographicSize - zoom * Sensitivity;
            camera.orthographicSize = Mathf.Clamp(currentZoom, ZoomMin, ZoomMax);

        }
    }
}
