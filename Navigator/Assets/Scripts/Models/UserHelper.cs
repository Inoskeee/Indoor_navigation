using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHelper : MonoBehaviour
{
    public static UserHelper Instance { get; set; }

    public string Id;
    [Header("User")]
    public string login;
    public string password;
    public string email;
    public string firstName;
    public string secondName;
    [Header("Information")]
    public bool isFast;
    public bool isWorker;

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
}
