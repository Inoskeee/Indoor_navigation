using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigatorManager : MonoBehaviour
{
    public GameObject[] pointObjects;
    public PointModel lastPoint;       //Хранит последнюю точку, куда пришел пользователь

    public List<GameObject> waypoints;
    public List<PointModel> points;
    public List<Button> floorImages;


    public GameObject firstFloor;
    public GameObject secondFloor;
    public GameObject thirdFloor;
    public GameObject warningPoint;
    public Camera mainCamera;

    [Header("Interface")]
    public Button InformationButton;
    public Button CreatePathButton;
    public Button ProfileButton;
    public Button QRScannerButton;
    public Button CloseButton;
    public Button LogoutButton;
    public Button MenuButton;
    public Button ClosePathButton;
    public Button Evacuation;
    public GameObject Panel;
    public GameObject Profile;
    public GameObject InformationPanel;
    public Text InformationText;
    [Header("Scripts")]
    public ClickReceiver clickReceiver;

    private void Awake()
    {
        Client.instance.ConnectToServer();

        if (pointObjects.Length == 0)
        {
            pointObjects = GameObject.FindGameObjectsWithTag("Point");
            firstFloor.SetActive(true);
            secondFloor.SetActive(false);
            thirdFloor.SetActive(false);
        }
        foreach(var pointObject in pointObjects)
        {
            var point = pointObject.GetComponent<PointModel>();
            Color col;
            ColorUtility.TryParseHtmlString("#253957", out col);
            pointObject.GetComponentInChildren<SpriteRenderer>().color = col;
            points.Add(point);
            if (point.isHidden || (!UserHelper.Instance.isWorker && point.isWork))
            {
                point.gameObject.SetActive(false);
            }
        }
        OnChangeFloorButton(floorImages[0].gameObject);
    }

    private void Start()
    {
        lastPoint = points.Where(p=>p.Id == "5").FirstOrDefault();
        OnEvacuationCheck();
        OnBlockLinkCheck();
    }
    private void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && mainCamera.orthographicSize > 2)
        {
            mainCamera.orthographicSize -= 1;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && mainCamera.orthographicSize < 35)
        {
            mainCamera.orthographicSize += 1;
        }
    }

    public void OnClickFloor(GameObject gameObject)
    {
        firstFloor.SetActive(false);
        secondFloor.SetActive(false);
        thirdFloor.SetActive(false);

        gameObject.SetActive(true);

        foreach(var links in waypoints)
        {
            if(links.tag == gameObject.tag)
            {
                links.SetActive(true);
            }
            else
            {
                links.SetActive(false);
            }
        }
        foreach(var point in points)
        {
            if (point.isHidden)
            {
                point.gameObject.SetActive(false);
            }
        }
    }

    public void OnChangeFloorButton(GameObject currentButton)
    {
        Color col;
        ColorUtility.TryParseHtmlString("#253957", out col);
        foreach (var item in floorImages)
        {
            var colors = item.colors;
            colors.normalColor = Color.white;
            item.colors = colors;
            item.gameObject.GetComponentInChildren<Text>().color = col;
        }

        var cols = currentButton.GetComponent<Button>().colors;
        cols.normalColor = col;
        currentButton.GetComponent<Button>().colors = cols;
        currentButton.GetComponentInChildren<Text>().color = Color.white;
    }

    public void OnHoverFloorButton(GameObject button)
    {
        button.GetComponentInChildren<Text>().color = Color.white;
    }
    public void LeaveHoverFloorButton(GameObject button)
    {
        if(button.GetComponent<Button>().colors.normalColor == Color.white)
        {
            Color col;
            ColorUtility.TryParseHtmlString("#253957", out col);
            button.GetComponentInChildren<Text>().color = col;
        }
    }


    public void onClickMenu()
    {
        Panel.SetActive(true);
        QRScannerButton.gameObject.SetActive(false);
        InformationButton.gameObject.SetActive(false);
        CreatePathButton.gameObject.SetActive(false);
        Evacuation.gameObject.SetActive(false);
    }
    public void onClosePathButton()
    {
        foreach(var link in waypoints)
        {
            GameObject.Destroy(link.gameObject);
        }
        waypoints.Clear();
        ClosePathButton.gameObject.SetActive(false);
        QRScannerButton.gameObject.SetActive(true);
        lastPoint = clickReceiver.pointEnd;
        clickReceiver.pointStart = null;
        clickReceiver.pointEnd = null;
        if (AdminModel.Instance.isEvacuation)
        {
            Evacuation.gameObject.SetActive(true);
        }
        ClearPoints();
    }

    public void onClickLogout()
    {
        UserHelper.Instance.login = "";
        UserHelper.Instance.password = "";
        UserHelper.Instance.email = "";
        UserHelper.Instance.firstName = "";
        UserHelper.Instance.secondName = "";
        SceneManager.LoadScene(0);
    }

    public void onClickProfile()
    {
        Profile.SetActive(true);
        Panel.SetActive(false);
    }
    public void onClickClose()
    {
        Panel.SetActive(false);
        if (clickReceiver.pointStart != null)
        {
            InformationButton.gameObject.SetActive(true);
        }
        if (clickReceiver.pointEnd != null)
        {
            CreatePathButton.gameObject.SetActive(true);
        }
        if (clickReceiver.pointStart == null && clickReceiver.pointEnd == null)
        {
            QRScannerButton.gameObject.SetActive(true);
        }
        if (AdminModel.Instance.isEvacuation)
        {
            Evacuation.gameObject.SetActive(true);
        }
        if (ClosePathButton.gameObject.activeSelf == true)
        {
            InformationButton.gameObject.SetActive(false);
            CreatePathButton.gameObject.SetActive(false);
            Evacuation.gameObject.SetActive(false);
            ClosePathButton.gameObject.SetActive(true);
        }
    }
    public void onCloseProfile()
    {
        Profile.SetActive(false);
        if (clickReceiver.pointStart != null)
        {
            InformationButton.gameObject.SetActive(true);
        }
        if (clickReceiver.pointEnd != null)
        {
            CreatePathButton.gameObject.SetActive(true);
        }
        if (clickReceiver.pointStart == null && clickReceiver.pointEnd == null)
        {
            QRScannerButton.gameObject.SetActive(true);
        }
        if (AdminModel.Instance.isEvacuation)
        {
            Evacuation.gameObject.SetActive(true);
        }
        if (ClosePathButton.gameObject.activeSelf == true)
        {
            InformationButton.gameObject.SetActive(false);
            CreatePathButton.gameObject.SetActive(false);
            Evacuation.gameObject.SetActive(false);
            ClosePathButton.gameObject.SetActive(true);
        }
    }
    public void onCloseInfo()
    {
        InformationPanel.SetActive(false);
        if (clickReceiver.pointStart != null)
        {
            InformationButton.gameObject.SetActive(true);
        }
        if (clickReceiver.pointEnd != null)
        {
            CreatePathButton.gameObject.SetActive(true);
        }
        if (clickReceiver.pointStart == null && clickReceiver.pointEnd == null)
        {
            QRScannerButton.gameObject.SetActive(true);
        }
    }
    public void onClickInfo()
    {
        InformationPanel.SetActive(true);
    }

    public void ClearPoints()
    {
        Color defaultColor;
        ColorUtility.TryParseHtmlString("#253957", out defaultColor);
        foreach(var point in pointObjects)
        {
            point.GetComponentInChildren<SpriteRenderer>().color = defaultColor;
        }
    }


    public void EvacuationStart()
    {
        AdminModel.Instance.isEvacuation = true;
        clickReceiver.pointStart = lastPoint;
        QRScannerButton.gameObject.SetActive(false);
        InformationButton.gameObject.SetActive(true);
        InformationText.text = clickReceiver.pointStart.information;

        clickReceiver.pointStart.GetComponentInChildren<SpriteRenderer>().color = Color.green;
        Evacuation.gameObject.SetActive(true);
    }
    public void EvacuationEnd()
    {
        AdminModel.Instance.isEvacuation = false;
        Evacuation.gameObject.SetActive(false);
    }

    public void QRScannerOpen()
    {
        SceneManager.LoadScene(2);
    }


    public void OnEvacuationCheck()
    {
        string url = "https://team-course-work.herokuapp.com/admin/getevac";
        StartCoroutine(OnEvacuationCheck_Coroutine(url));
    }
    public IEnumerator OnEvacuationCheck_Coroutine(string url)
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
                Debug.Log(request.downloadHandler.text);
                if(request.downloadHandler.text == "true")
                {
                    AdminModel.Instance.isEvacuation = true;
                    AdminModel.Instance.EvacuationUpdate();
                }
                else if(request.downloadHandler.text == "false")
                {
                    AdminModel.Instance.isEvacuation = false;
                    AdminModel.Instance.EvacuationUpdate();
                }
            }
        }

    }

    public void OnBlockLinkCheck()
    {
        string url = "https://team-course-work.herokuapp.com/link/getblockedlinks";
        StartCoroutine(OnBlockLinkCheck_Coroutine(url));
    }
    public IEnumerator OnBlockLinkCheck_Coroutine(string url)
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
                string value = "{\"Items\":" + request.downloadHandler.text + "}";
                LinkHelper[] linkHelpers = JsonHelper.FromJson<LinkHelper>(value);
                foreach (var link in linkHelpers)
                {
                    Debug.Log(link.firstPoint);
                    Debug.Log(link.secondPoint);
                    if (link.isBlockedLink)
                    {
                        GameObject warnPoint = GameObject.Instantiate(warningPoint);
                        warnPoint.AddComponent<LinkModel>();
                        LinkModel model = warnPoint.GetComponent<LinkModel>();
                        model.Id = link.Id;
                        PointModel pointModel = warnPoint.GetComponent<PointModel>();
                        pointModel.information = link.blockInfo;
                        PointModel pointStart = null;
                        PointModel pointEnd = null;

                        foreach (var point in points)
                        {
                            if (point.Id == link.firstPoint)
                            {
                                pointStart = point;
                            }
                            if (point.Id == link.secondPoint)
                            {
                                pointEnd = point;
                            }
                        }

                        warnPoint.transform.position = (pointStart.transform.position + pointEnd.transform.position) / 2;

                        if (pointStart.Floor == 1 && pointEnd.Floor == 1)
                        {
                            warnPoint.gameObject.transform.parent = firstFloor.gameObject.transform;
                        }
                        else if (pointStart.Floor == 2 && pointEnd.Floor == 2)
                        {
                            warnPoint.gameObject.transform.parent = secondFloor.gameObject.transform;
                        }
                        else if (pointStart.Floor == 3 && pointEnd.Floor == 3)
                        {
                            warnPoint.gameObject.transform.parent = thirdFloor.gameObject.transform;
                        }
                        else if(pointStart.Floor != pointEnd.Floor)
                        {
                            switch (pointStart.Floor)
                            {
                                case 1:
                                    warnPoint.gameObject.transform.parent = thirdFloor.gameObject.transform;
                                    break;
                                case 2:
                                    warnPoint.gameObject.transform.parent = thirdFloor.gameObject.transform;
                                    break;
                                case 3:
                                    warnPoint.gameObject.transform.parent = thirdFloor.gameObject.transform;
                                    break;
                            }
                        }
                        AdminModel.Instance.blockedLinks.Add(model);
                    }
                }

            }
        }

    }

}
