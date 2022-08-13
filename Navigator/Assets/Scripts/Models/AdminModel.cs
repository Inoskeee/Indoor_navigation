using System;
using System.Collections;
using System.Collections.Generic;
using NotificationSamples;
using UnityEngine;

public class AdminModel : MonoBehaviour
{
    public static AdminModel Instance { get; set; }

    public bool isEvacuation;
    public List<LinkModel> blockedLinks;

    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EvacuationUpdate()
    {
        NavigatorManager navigatorManager = GameObject.FindObjectOfType<NavigatorManager>();
        if (AdminModel.Instance.isEvacuation)
        {
            GameNotificationsManager notificationsManager = GameObject.FindObjectOfType<GameNotificationsManager>();
            GameNotificationChannel channel = new GameNotificationChannel("testNotification", "Just Notification", "Notification");
            notificationsManager.Initialize(channel);

            navigatorManager.EvacuationStart();

            CreateNotification("ЭВАКУАЦИЯ!", "Началась эвакуация, срочно покиньте здание!", DateTime.Now, notificationsManager);
        }
        else
        {
            navigatorManager.EvacuationEnd();
        }
    }

    public void ManageLink(string json)
    {
        try
        {
            NavigatorManager navigatorManager = GameObject.FindObjectOfType<NavigatorManager>();

            LinkHelper link = JsonUtility.FromJson<LinkHelper>(json);
            if (link.isBlockedLink)
            {
                GameObject warnPoint = GameObject.Instantiate(navigatorManager.warningPoint);
                warnPoint.AddComponent<LinkModel>();
                LinkModel model = warnPoint.GetComponent<LinkModel>();
                model.Id = link.Id;
                PointModel pointModel = warnPoint.GetComponent<PointModel>();
                pointModel.information = link.blockInfo;
                PointModel pointStart = null;
                PointModel pointEnd = null;

                foreach (var point in navigatorManager.points)
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
                    warnPoint.gameObject.transform.parent = navigatorManager.firstFloor.gameObject.transform;
                }
                else if (pointStart.Floor == 2 && pointEnd.Floor == 2)
                {
                    warnPoint.gameObject.transform.parent = navigatorManager.secondFloor.gameObject.transform;
                }
                else if (pointStart.Floor == 3 && pointEnd.Floor == 3)
                {
                    warnPoint.gameObject.transform.parent = navigatorManager.thirdFloor.gameObject.transform;
                }
                blockedLinks.Add(model);
            }
            else
            {
                foreach(var model in blockedLinks.ToArray())
                {
                    if(model.Id == link.Id)
                    {
                        GameObject.Destroy(model.gameObject);
                        blockedLinks.Remove(model);
                    }
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
        
    }

    public void ManageUser(string json)
    {
        NavigatorManager navigatorManager = GameObject.FindObjectOfType<NavigatorManager>();
        UserModel user = JsonUtility.FromJson<UserModel>(json);
        if(user.login == UserHelper.Instance.login)
        {
            UserHelper.Instance.isWorker = user.isWorker;
            foreach (var pointObject in navigatorManager.pointObjects)
            {
                var point = pointObject.GetComponent<PointModel>();

                navigatorManager.points.Add(point);
                if (point.isHidden || (!UserHelper.Instance.isWorker && point.isWork))
                {
                    point.gameObject.SetActive(false);
                }
                else if (point.isWork && user.isWorker)
                {
                    point.gameObject.SetActive(true);
                }
            }
        }
    }

    private void CreateNotification(string title, string message, DateTime time, GameNotificationsManager notificationsManager)
    {
        IGameNotification notification = notificationsManager.CreateNotification();
        if(notification != null)
        {
            notification.Title = title;
            notification.Body = message;
            notification.DeliveryTime = time;
            notificationsManager.ScheduleNotification(notification);
        }
    }
}
