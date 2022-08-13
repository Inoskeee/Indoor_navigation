using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecoderQR : MonoBehaviour
{
    public QRCodeDecodeController qRCodeDecode;
    public Text textResult;
    public GameObject resetGameObj;
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    public void QRCodeCallback(string result)
    {
        textResult.text = result;
        resetGameObj.SetActive(true);
    }

    public void ResetScanner()
    {
        qRCodeDecode.Reset();
        resetGameObj.SetActive(false);
        textResult.text = "scanning";

    }

    public void StartWork()
    {
        qRCodeDecode.StartWork();
        resetGameObj.SetActive(false);
        textResult.text = "scanning..";
    }

    public void StopWork()
    {
        qRCodeDecode.StopWork();
        resetGameObj.SetActive(true);
        textResult.text = "";
    }
}
