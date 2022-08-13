using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointModel : MonoBehaviour
{
    public string Id;
    [SerializeField]
    public string information;
    [SerializeField]
    public int Floor;
    [SerializeField]
    public bool isHidden;
    [SerializeField]
    public bool isWork;

    public bool isWarningPoint;

    public double x;
    public double y;
    public double z;

}
