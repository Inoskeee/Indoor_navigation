using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PathModel
{
    public string Id;
    [SerializeField]
    public string pointStart;
    [SerializeField]
    public string pointEnd;
    [SerializeField]
    public string pathName;
    [SerializeField]
    public bool isFastPath;
    [SerializeField]
    public bool isWorkerPath;
    [SerializeField]
    public string pointModels;

}
