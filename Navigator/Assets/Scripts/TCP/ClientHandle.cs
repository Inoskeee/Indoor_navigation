using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{

    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();
        if (_msg.ToString().StartsWith("isEvacuation"))
        {
            string[] parametres = _msg.ToString().Split('=');
            if(parametres[1] == "True")
            {
                AdminModel.Instance.isEvacuation = true;
                AdminModel.Instance.EvacuationUpdate();
            }
            else
            {
                AdminModel.Instance.isEvacuation = false;
                AdminModel.Instance.EvacuationUpdate();
            }
        }
        else if (_msg.ToString().StartsWith("LinkModel"))
        {
            string[] parametres = _msg.ToString().Split('=');
            AdminModel.Instance.ManageLink(parametres[1]);
        }
        else if (_msg.ToString().StartsWith("UserModel"))
        {
            string[] parametres = _msg.ToString().Split('=');
            AdminModel.Instance.ManageUser(parametres[1]);
        }
        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        //ClientSend.WelcomeReceived();
    }
}
