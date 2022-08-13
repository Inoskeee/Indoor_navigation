using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    public Text Login;
    public InputField Password;
    public InputField Email;
    public InputField Name;
    public InputField Surname;
    public Text Role;
    public Text ErrorProvider;
    public Dropdown DefaultPath;
    
    void Start()
    {
    }
    private void OnEnable()
    {
        Login.text = UserHelper.Instance.login;
        Password.text = UserHelper.Instance.password;
        Email.text = UserHelper.Instance.email;
        Name.text = UserHelper.Instance.firstName;
        Surname.text = UserHelper.Instance.secondName;

        if (UserHelper.Instance.isWorker == true)
        {
            Role.text = "Работник";
        }
        else
        {
            Role.text = "Посетитель";
        }

        if (UserHelper.Instance.isFast == true)
        {
            DefaultPath.value = 0;
        }
        else
        {
            DefaultPath.value = 1;
        }
        ErrorProvider.gameObject.SetActive(false);
    }
    public void OnChangeClick()
    {
        if (Name.text == "" || Surname.text == "" || Email.text == "" || Login.text == "" || Password.text == "")
        {
            ErrorProvider.gameObject.SetActive(true);
            ErrorProvider.text = "Все поля должны быть заполнены";
        }
        else
        {
            UserModel user = new UserModel()
            {
                Id = UserHelper.Instance.Id,
                firstName = Name.text,
                secondName = Surname.text,
                email = Email.text,
                login = Login.text,
                password = Password.text,
                isWorker = UserHelper.Instance.isWorker
        };
            if (DefaultPath.value == 0)
            {
                user.isFast = true;
            }
            else
            {
                user.isFast = false;
            }
            string json = JsonUtility.ToJson(user);
            Debug.Log(json);
            string url = "https://team-course-work.herokuapp.com/user/ChangeInformation?jsonString=" + json;
            StartCoroutine(OnChangeClick_Coroutine(url));
        }
    }
    public IEnumerator OnChangeClick_Coroutine(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(url, ""))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                ErrorProvider.gameObject.SetActive(true);
                Debug.Log(request.error);
                ErrorProvider.text = request.error;
            }
            else
            {
                if (request.downloadHandler.text == "ok")
                {
                    UserHelper.Instance.login = Login.text;
                    UserHelper.Instance.password = Password.text;
                    UserHelper.Instance.email = Email.text;
                    UserHelper.Instance.firstName = Name.text;
                    UserHelper.Instance.secondName = Surname.text;
                    if (DefaultPath.value == 0)
                    {
                        UserHelper.Instance.isFast = true;
                    }
                    else
                    {
                        UserHelper.Instance.isFast = false;
                    }
                    if (Role.text == "Работник")
                    {
                        UserHelper.Instance.isWorker = true;
                    }
                    else
                    {
                        UserHelper.Instance.isWorker = false;
                    }
                    ErrorProvider.gameObject.SetActive(true);
                    ErrorProvider.text = "Информация успешно изменена!";
                }
                else
                {
                    ErrorProvider.gameObject.SetActive(true);
                    Debug.Log(request.downloadHandler.text);
                    ErrorProvider.text = request.downloadHandler.text;
                }
            }
        }
    }
}
