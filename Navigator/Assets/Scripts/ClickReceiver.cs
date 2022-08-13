using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
public class ClickReceiver : MonoBehaviour, IPointerClickHandler
{
    public PointModel pointStart;
    public PointModel pointEnd;
    public NavigatorManager NavigatorManager;
    public Material material;

    public float distance;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(NavigatorManager.ClosePathButton.gameObject.activeSelf == false && eventData.pointerCurrentRaycast.gameObject.GetComponent<PointModel>().isWarningPoint == false)
        {
            if (pointStart != null)
            {
                if (pointEnd == null)
                {
                    pointEnd = eventData.pointerCurrentRaycast.gameObject.GetComponent<PointModel>();
                    NavigatorManager.CreatePathButton.gameObject.SetActive(true);
                    NavigatorManager.InformationText.text = pointEnd.information;

                    Color col;
                    ColorUtility.TryParseHtmlString("#57BF22", out col);
                    pointEnd.GetComponentInChildren<SpriteRenderer>().color = col;
                    distance = Vector3.Distance(pointStart.transform.position, pointEnd.transform.position);
                }
                else
                {
                    Color defaultColor;
                    ColorUtility.TryParseHtmlString("#253957", out defaultColor);
                    pointEnd.GetComponentInChildren<SpriteRenderer>().color = defaultColor;
                    pointStart.GetComponentInChildren<SpriteRenderer>().color = defaultColor;

                    pointStart = eventData.pointerCurrentRaycast.gameObject.GetComponent<PointModel>();
                    pointEnd = null;
                    NavigatorManager.CreatePathButton.gameObject.SetActive(false);
                    NavigatorManager.InformationText.text = pointStart.information;

                    Color col;
                    ColorUtility.TryParseHtmlString("#57BF22", out col);
                    pointStart.GetComponentInChildren<SpriteRenderer>().color = col;
                }
            }
            else
            {
                pointStart = eventData.pointerCurrentRaycast.gameObject.GetComponent<PointModel>();
                NavigatorManager.QRScannerButton.gameObject.SetActive(false);
                NavigatorManager.InformationButton.gameObject.SetActive(true);
                NavigatorManager.InformationText.text = pointStart.information;

                Color col;
                ColorUtility.TryParseHtmlString("#57BF22", out col);
                pointStart.GetComponentInChildren<SpriteRenderer>().color = col;
            }
        }
        else if(NavigatorManager.ClosePathButton.gameObject.activeSelf == false)
        {
            PointModel warning = eventData.pointerCurrentRaycast.gameObject.GetComponent<PointModel>();
            NavigatorManager.InformationPanel.gameObject.SetActive(true);
            NavigatorManager.InformationText.text = warning.information;
        }

    }

    public void OnCreatePathClick()
    {
        PathModel path = new PathModel()
        {
            pointStart = pointStart.Id,
            pointEnd = pointEnd.Id,
            isFastPath = UserHelper.Instance.isFast,
            isWorkerPath = UserHelper.Instance.isWorker
        };
        string msg = $"firstPointId={path.pointStart}&endPointId={path.pointEnd}&isFast={path.isFastPath}&isWork={path.isWorkerPath}";
        string url = "https://team-course-work.herokuapp.com/admin/getastarpath?" + msg;
        StartCoroutine(OnCreatePathClick_Coroutine(url));
    }

    public IEnumerator OnCreatePathClick_Coroutine(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(url, ""))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else 
            {
                PathModel path = JsonUtility.FromJson<PathModel>(request.downloadHandler.text);
                Debug.Log(path.pointModels);
                string value = "{\"Items\":" + path.pointModels + "}";

                PointHelper[] pointModels = JsonHelper.FromJson<PointHelper>(value);

                foreach (var point in pointModels)
                {
                    pointEnd = NavigatorManager.points.Where(p=>p.Id==point.Id).FirstOrDefault();

                    if (pointStart.Floor == 1 && pointEnd.Floor == 1)
                    {
                        GameObject lineRender = CreateLine();
                        lineRender.tag = "FirstFloor";
                        if(NavigatorManager.firstFloor.activeSelf == false)
                        {
                            lineRender.SetActive(false);
                        }
                        NavigatorManager.waypoints.Add(lineRender);
                    }
                    else if (pointStart.Floor == 2 && pointEnd.Floor == 2)
                    {
                        GameObject lineRender = CreateLine();
                        lineRender.tag = "SecondFloor";
                        if (NavigatorManager.secondFloor.activeSelf == false)
                        {
                            lineRender.SetActive(false);
                        }
                        NavigatorManager.waypoints.Add(lineRender);
                    }
                    else if (pointStart.Floor == 3 && pointEnd.Floor == 3)
                    {
                        GameObject lineRender = CreateLine();
                        lineRender.tag = "ThirdFloor";
                        if (NavigatorManager.thirdFloor.activeSelf == false)
                        {
                            lineRender.SetActive(false);
                        }
                        NavigatorManager.waypoints.Add(lineRender);
                    }
                    pointStart = pointEnd;
                }
                NavigatorManager.InformationButton.gameObject.SetActive(false);
                NavigatorManager.CreatePathButton.gameObject.SetActive(false);
                NavigatorManager.Evacuation.gameObject.SetActive(false);
                NavigatorManager.ClosePathButton.gameObject.SetActive(true);
            }
        }

    }


    public void OnCreateEvacuationClick()
    {
        string url = "https://team-course-work.herokuapp.com/admin/getevacpath?firstPointId=" + NavigatorManager.lastPoint.Id;
        pointStart = NavigatorManager.lastPoint;
        Color col;
        ColorUtility.TryParseHtmlString("#57BF22", out col);
        pointStart.GetComponentInChildren<SpriteRenderer>().color = col;
        StartCoroutine(OnCreateEvacuationClick_Coroutine(url));
    }
    public IEnumerator OnCreateEvacuationClick_Coroutine(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(url, ""))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            }
            else
            {
                PathModel path = JsonUtility.FromJson<PathModel>(request.downloadHandler.text);
                Debug.Log(path.pointModels);
                string value = "{\"Items\":" + path.pointModels + "}";

                PointHelper[] pointModels = JsonHelper.FromJson<PointHelper>(value);

                foreach (var point in pointModels)
                {
                    pointEnd = NavigatorManager.points.Where(p => p.Id == point.Id).FirstOrDefault();

                    if (pointStart.Floor == 1 && pointEnd.Floor == 1)
                    {
                        GameObject lineRender = CreateLine();
                        lineRender.tag = "FirstFloor";
                        if (NavigatorManager.firstFloor.activeSelf == false)
                        {
                            lineRender.SetActive(false);
                        }
                        NavigatorManager.waypoints.Add(lineRender);
                    }
                    else if (pointStart.Floor == 2 && pointEnd.Floor == 2)
                    {
                        GameObject lineRender = CreateLine();
                        lineRender.tag = "SecondFloor";
                        if (NavigatorManager.secondFloor.activeSelf == false)
                        {
                            lineRender.SetActive(false);
                        }
                        NavigatorManager.waypoints.Add(lineRender);
                    }
                    else if (pointStart.Floor == 3 && pointEnd.Floor == 3)
                    {
                        GameObject lineRender = CreateLine();
                        lineRender.tag = "ThirdFloor";
                        if (NavigatorManager.thirdFloor.activeSelf == false)
                        {
                            lineRender.SetActive(false);
                        }
                        NavigatorManager.waypoints.Add(lineRender);
                    }
                    pointStart = pointEnd;

                    

                    NavigatorManager.InformationButton.gameObject.SetActive(false);
                    NavigatorManager.CreatePathButton.gameObject.SetActive(false);
                    NavigatorManager.Evacuation.gameObject.SetActive(false);
                    NavigatorManager.ClosePathButton.gameObject.SetActive(true);
                }
                Color col;
                ColorUtility.TryParseHtmlString("#57BF22", out col);
                pointStart.GetComponentInChildren<SpriteRenderer>().color = col;
            }
        }
    }
    public GameObject CreateLine()
    {
        GameObject lineRender = new GameObject();
        lineRender.AddComponent<LineRenderer>();

        LineRenderer renderer = lineRender.GetComponent<LineRenderer>();

        renderer.SetPosition(0, pointStart.transform.position + new Vector3(0, 0, -1));
        renderer.SetPosition(1, pointEnd.transform.position + new Vector3(0, 0, -1));
        renderer.startWidth = 0.4f;
        renderer.endWidth = 0.4f;
        renderer.useWorldSpace = false;
        renderer.numCapVertices = 90;

        renderer.material = material;


        lineRender.AddComponent<LinkModel>();
        lineRender.GetComponent<LinkModel>().firstPoint = pointStart.Id;
        lineRender.GetComponent<LinkModel>().secondPoint = pointEnd.Id;
        lineRender.GetComponent<LinkModel>().distance = (double)System.Math.Round(Vector2.Distance(pointStart.transform.position, pointEnd.transform.position), 1);
        lineRender.gameObject.transform.parent = transform;
        return lineRender;
    }


    void Start()
    {
        if (ResultQR.Instance.isScanned)
        {
            foreach(var point in NavigatorManager.points)
            {
                if(point.Id == ResultQR.Instance.PointId)
                {
                    pointStart = point;
                    NavigatorManager.QRScannerButton.gameObject.SetActive(false);
                    NavigatorManager.InformationButton.gameObject.SetActive(true);
                    NavigatorManager.InformationText.text = pointStart.information;

                    Color col;
                    ColorUtility.TryParseHtmlString("#57BF22", out col);
                    pointStart.GetComponentInChildren<SpriteRenderer>().color = col;
                    ResultQR.Instance.PointId = "NaN";
                    ResultQR.Instance.isScanned = false;

                }
            }
        }
    }

    void Update()
    {
        
    }

}
