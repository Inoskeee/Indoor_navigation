using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserModel
{
    [SerializeField] public string Id;
    [SerializeField] public string login;
    [SerializeField] public string password;
    [SerializeField] public string email;
    [SerializeField] public string firstName;
    [SerializeField] public string secondName;
    [SerializeField] public bool isFast;
    [SerializeField] public bool isWorker;

}
