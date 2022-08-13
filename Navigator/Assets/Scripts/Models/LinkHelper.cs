using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LinkHelper
{
    [SerializeField] public string Id;

    [SerializeField] public string firstPoint;
    [SerializeField] public string secondPoint;

    [SerializeField] public double distance;
    [SerializeField] public bool isBlockedLink;
    [SerializeField] public bool isWorkerLink;
    [SerializeField] public string blockInfo;
    [SerializeField] public int weight;
}
