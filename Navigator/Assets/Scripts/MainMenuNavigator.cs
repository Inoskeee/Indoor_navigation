using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Http;
using System.Text;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Net.Mail;
using System.Net;

public class MainMenuNavigator : MonoBehaviour
{

    public GameObject registration;
    public GameObject autorization;
    public GameObject recoveryPassword;

    [Header("Authorization")]
    public InputField Username;
    public InputField UserPassword;
    public Text AuthErrorProvider;

    [Header("Registration")]
    public InputField Name;
    public InputField Surname;
    public InputField Login;
    public InputField Password;
    public InputField Email;
    public Dropdown PathType;
    public Text RegErrorProvider;

    [Header("RecoveryPassword")]
    public InputField RecoveryEmail;
    public Text RecoveryErrorProvider;


    public void onClickRegistration()
    {
        registration.SetActive(true);
        autorization.SetActive(false);
        AuthErrorProvider.gameObject.SetActive(false);
        RegErrorProvider.gameObject.SetActive(false);
        RecoveryErrorProvider.gameObject.SetActive(false);
    }

    public void onClickRecovery()
    {
        recoveryPassword.SetActive(true);
        autorization.SetActive(false);
        AuthErrorProvider.gameObject.SetActive(false);
        RegErrorProvider.gameObject.SetActive(false);
        RecoveryErrorProvider.gameObject.SetActive(false);
    }

    public void onClickCancel(GameObject cancelled)
    {
        cancelled.SetActive(false);
        autorization.SetActive(true);
        AuthErrorProvider.gameObject.SetActive(false);
        RegErrorProvider.gameObject.SetActive(false);
        RecoveryErrorProvider.gameObject.SetActive(false);
    }

    public void Registration()
    {
        if (Name.text == "" || Surname.text == "" || Email.text == "" || Login.text == "" || Password.text == "")
        {
            RegErrorProvider.gameObject.SetActive(true);
            RegErrorProvider.text = "Все поля должны быть заполнены";
        }
        else
        {
            UserModel user = new UserModel()
            {
                firstName = Name.text,
                secondName = Surname.text,
                email = Email.text,
                login = Login.text,
                password = Password.text
            };
            if (PathType.itemText.text == "Быстрый")
            {
                user.isFast = true;
            }
            else
            {
                user.isFast = false;
            }
            string json = JsonUtility.ToJson(user);
            string url = "https://team-course-work.herokuapp.com/user/registration?jsonstring=" + json;
            StartCoroutine(Registration_Coroutine(url));
        }
    }

    public IEnumerator Registration_Coroutine(string url)
    {
        using(UnityWebRequest request = UnityWebRequest.Post(url, ""))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if(request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                RegErrorProvider.gameObject.SetActive(true);
                RegErrorProvider.text = request.error;
            }
            else 
            {
                Debug.Log(request.downloadHandler.text);
                if(request.downloadHandler.text == "\"ok\"")
                {
                    UserHelper.Instance.login = Login.text;
                    UserHelper.Instance.password = Password.text;
                    UserHelper.Instance.email = Email.text;
                    UserHelper.Instance.firstName = Name.text;
                    UserHelper.Instance.secondName = Surname.text;
                    if (PathType.value == 0)
                    {
                        UserHelper.Instance.isFast = true;
                    }
                    else
                    {
                        UserHelper.Instance.isFast = false;
                    }
                    UserHelper.Instance.isWorker = false;

                    SceneManager.LoadScene(0);
                }
                if(request.downloadHandler.text == "emailerror")
                {
                    RegErrorProvider.gameObject.SetActive(true);
                    RegErrorProvider.text = "Введите корректный email";
                }

            }
        }
    }
    public void Authorization()
    {
        AuthErrorProvider.gameObject.SetActive(true);
        AuthErrorProvider.text = "Выполняется вход в систему...";
        if (Username.text == "" || UserPassword.text == "")
        {
            AuthErrorProvider.gameObject.SetActive(true);
            AuthErrorProvider.text = "Поля должны быть заполнены";
        }
        else
        {
            UserModel user = new UserModel()
            {
                login = Username.text,
                password = UserPassword.text
            };
            string json = JsonUtility.ToJson(user);
            string url = "https://team-course-work.herokuapp.com/user/login?jsonstring=" + json;
            StartCoroutine(Authorization_Coroutine(url));
        }
    }
    public IEnumerator Authorization_Coroutine(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(url, ""))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                AuthErrorProvider.gameObject.SetActive(true);
                AuthErrorProvider.text = request.error;
            }
            else 
            { 
                if(request.downloadHandler.text == "null")
                {
                    AuthErrorProvider.gameObject.SetActive(true);
                    AuthErrorProvider.text = "Вы ввели неверный логин или пароль";
                }
                else
                {
                    UserModel user = JsonUtility.FromJson<UserModel>(request.downloadHandler.text);

                    UserHelper.Instance.Id = user.Id;
                    UserHelper.Instance.login = user.login;
                    UserHelper.Instance.password = user.password;
                    UserHelper.Instance.email = user.email;
                    UserHelper.Instance.firstName = user.firstName;
                    UserHelper.Instance.secondName = user.secondName;
                    UserHelper.Instance.isFast = user.isFast;
                    UserHelper.Instance.isWorker = user.isWorker;

                    SceneManager.LoadScene(1);
                }
            }
        }

    }
    public void RecoveryPassword()
    {
        if (RecoveryEmail.text == "")
        {
            RecoveryErrorProvider.gameObject.SetActive(true);
            RecoveryErrorProvider.color = Color.red;
            RecoveryErrorProvider.text = "Email должен быть заполнен";
        }
        else
        {
            UserModel user = new UserModel()
            {
                email = RecoveryEmail.text
            };
            string json = JsonUtility.ToJson(user);
            Debug.Log(json);
            string url = "https://team-course-work.herokuapp.com/user/changepassword?jsonString=" + json;
            StartCoroutine(RecoveryPassword_Coroutine(url));
        }
    }
    public IEnumerator RecoveryPassword_Coroutine(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(url, ""))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
            {
                RecoveryErrorProvider.gameObject.SetActive(true);
                RecoveryErrorProvider.color = Color.red;
                RecoveryErrorProvider.text = "Вы ввели неверный адрес электронной почты";
                Debug.Log(request.result);
            }
            else 
            { 
                if(request.downloadHandler.text != "false")
                {
                    using (MailMessage mail = new MailMessage())
                    {

                        string emailFrom;
                        emailFrom = Environment.GetEnvironmentVariable("building.navigation.course@gmail.com");
                        if (emailFrom == null)
                        {
                            emailFrom = "building.navigation.course@gmail.com";
                        }

                        mail.From = new MailAddress(emailFrom); // Откуда сообщение
                        mail.To.Add(RecoveryEmail.text); // Кому сообщение

                        // Тема письма
                        mail.Subject = "Password change (Building Navigation)";
                        // Текст письма
                        mail.Body = "<h1>New Password: " + request.downloadHandler.text + "</h1>";
                        // письмо представляет код html
                        mail.IsBodyHtml = true;


                        string emailSmtp;
                        emailSmtp = Environment.GetEnvironmentVariable("smtp.gmail.com");
                        if (emailSmtp == null)
                        {
                            emailSmtp = "smtp.gmail.com";
                        }

                        using (SmtpClient smtp = new SmtpClient(emailSmtp))
                        {
                            // логин и пароль

                            string emailPassword;
                            emailPassword = Environment.GetEnvironmentVariable("buildingnavigationcourse7894562231a!");
                            if (emailPassword == null)
                            {
                                emailPassword = "buildingnavigationcourse7894562231a!";
                            }

                            smtp.Credentials = new NetworkCredential(emailFrom, emailPassword);
                            smtp.EnableSsl = true;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.Send(mail);
                        }
                    }
                    RecoveryErrorProvider.gameObject.SetActive(true);
                    RecoveryErrorProvider.color = Color.green;
                    RecoveryErrorProvider.text = "Новый пароль отправлен на email!";
                }
                else
                {
                    RecoveryErrorProvider.gameObject.SetActive(true);
                    RecoveryErrorProvider.color = Color.red;
                    RecoveryErrorProvider.text = "Вы ввели неверный адрес электронной почты";
                }
            }
        }

    }

    private void Awake()
    {
        ResultQR.Instance.PointId = "NaN";
    }

}
