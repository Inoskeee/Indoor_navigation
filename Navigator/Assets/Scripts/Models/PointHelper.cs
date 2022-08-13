using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointHelper
{
    public string Id;
    [SerializeField]
    public string information;
    [SerializeField]
    public int Floor;
    [SerializeField]
    public bool isHidden;

    public float x;
    public float y;
    public float z;

}
