using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropCanvas : MonoBehaviour
{

    /*public bool smartDrag = true;
    public bool isDraggable = true;
    public bool isDragged = false;

    Vector2 initialPositionMouse;
    Vector2 initialPositionObject;

    private void Update()
    {
        if (isDragged && Input.touchCount == 1)
        {
            if (!smartDrag)
            {
                transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                transform.position = initialPositionObject + (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - initialPositionMouse;
            }
        }
    }

    private void OnMouseOver()
    {
        if(isDraggable && Input.GetMouseButtonDown(0))
        {
            if (smartDrag)
            {
                initialPositionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                initialPositionObject = transform.position;
            }
            if(Input.touchCount == 1)
            {
                isDragged = true;
            }
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
    }*/
    public string draggingTag;
    public Camera cam;

    private Vector3 dis;
    private float posX;
    private float posY;
    private bool touched = false;
    private bool dragging = false;

    private Transform toDrag;
    private Rigidbody toDragRigidbody;
    private Vector3 previousPos;

    private void FixedUpdate()
    {
        if(gameObject.transform.position.x < -18)
        {
            gameObject.transform.position = new Vector2(-18, gameObject.transform.position.y);
            return;
        }
        if (gameObject.transform.position.x > 18)
        {
            gameObject.transform.position = new Vector2(18, gameObject.transform.position.y);
            return;
        }
        if (gameObject.transform.position.y < -18)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, -18);
            return;
        }
        if (gameObject.transform.position.y  > 18)
        {
            gameObject.transform.position = new Vector2(gameObject.transform.position.x, 18);
            return;
        }

        if (Input.touchCount != 1)
        {
            dragging = false;
            touched = false;
            return;
        }
        Touch touch = Input.touches[0];
        Vector3 pos = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(pos);
            if(Physics.Raycast(ray,out hit) && hit.collider.tag == draggingTag)
            {
                toDrag = hit.transform;
                previousPos = toDrag.position;
                toDragRigidbody = toDrag.GetComponent<Rigidbody>();
                dis = cam.WorldToScreenPoint(previousPos);
                posX = Input.GetTouch(0).position.x - dis.x;
                posY = Input.GetTouch(0).position.y - dis.y;
                SetDraggingProperties(toDragRigidbody);
                touched = true;
            }
        }

        if(touched && touch.phase == TouchPhase.Moved)
        {
            dragging = true;

            float posXNow = Input.GetTouch(0).position.x - posX;
            float posYNow = Input.GetTouch(0).position.y - posY;
            Vector3 curPos = new Vector3(posXNow, posYNow, dis.z);

            Vector3 worldPos = cam.ScreenToWorldPoint(curPos) - previousPos;
            worldPos = new Vector3(worldPos.x, worldPos.y, 0.0f);

            toDragRigidbody.velocity = worldPos / (Time.deltaTime * 10);
            previousPos = toDrag.position;
        }
        if(dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            dragging = false;
            touched = false;
            previousPos = new Vector3(0.0f, 0.0f, 0.0f);
            SetFreeProperties(toDragRigidbody);
        }
    }
    private void SetDraggingProperties(Rigidbody rb)
    {
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.drag = 20;
    }

    private void SetFreeProperties(Rigidbody rb)
    {
        rb.isKinematic = true;
        rb.drag = 20;
    }
}
