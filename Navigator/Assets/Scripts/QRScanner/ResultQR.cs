using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultQR : MonoBehaviour
{
    public static ResultQR Instance { get; set; }

    public string PointId;
    public bool isScanned;

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
