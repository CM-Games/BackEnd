using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using UnityEngine.SceneManagement;
using LitJson;
using System;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    [Header("Title")]
    public Button startBtn;
    public Transform tempNotice;

    [Header("Main")]
    public Transform noticeObj;

    public static bool backendInit = false;

    string id = "ccm11441";
    string pw = "cjfals12";

    void Awake()
    {
        if (!backendInit)
        {
            startBtn.interactable = false;
            Backend.Initialize(HandleBackendCallback);
        }
        else
        {
            getNoti();
        }
    }
    

    void HandleBackendCallback()
    {
        if (Backend.IsInitialized)
        {
            Debug.Log("뒤끝SDK 초기화 완료");
            startBtn.interactable = true;
            backendInit = true;

            getTempNoti();
            // example 
            // 버전체크 -> 업데이트 

            // 구글 해시키 획득 
            //if (!Backend.Utils.GetGoogleHash().Equals(""))
            //Debug.Log(Backend.Utils.GetGoogleHash());

            // 서버시간 획득
            //Debug.Log(Backend.Utils.GetServerTime());
        }
        // 실패
        else
        {
            Debug.LogError("Failed to initialize the backend");
        }
    }

    void getTempNoti()
    {
        Backend.Notice.GetTempNotice(callback =>
        {
            // 이후 처리
            Debug.Log(callback);

            JsonData data = JsonMapper.ToObject(callback);

            try
            {
                bool isUse = (bool)data["isUse"];
                string contents = data["contents"].ToString();

                if (isUse)
                {
                    print("임시 공지를 가져옴");
                    tempNotice.GetChild(0).GetComponent<Text>().text = contents;
                    tempNotice.gameObject.SetActive(true);
                }
                else print("임시 공지가 등록은 되어있으나 공개되지 않음");
            }
            catch (Exception)
            {
                print("등록된 임시공지가 없음");
            }
        });
    }

    void getNoti()
    {
        BackendReturnObject bro = Backend.Notice.NoticeList();

        if (bro.IsSuccess())
        {
            JsonData noticeData = bro.GetReturnValuetoJSON()["rows"][0];

            string date = noticeData["postingDate"][0].ToString();
            string title = noticeData["title"][0].ToString();
            string content = noticeData["content"][0].ToString().Substring(0,10);
            string URL = "http://upload-console.thebackend.io" + noticeData["imageKey"][0];

            noticeObj.GetChild(0).GetComponent<Text>().text = date;
            noticeObj.GetChild(1).GetComponent<Text>().text = title;
            noticeObj.GetChild(2).GetComponent<Text>().text = content;
            StartCoroutine(WWWImageDown(URL));
            
        }
    }

    IEnumerator WWWImageDown(string url)
    {
        UnityWebRequest wr = new UnityWebRequest(url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        yield return wr.SendWebRequest();

        if (!(wr.isNetworkError || wr.isHttpError))
        {
            if (texDl.texture != null)
            {
                Texture2D t = texDl.texture;
                Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
                noticeObj.GetChild(3).GetComponent<Image>().sprite = s;

            }

            noticeObj.gameObject.SetActive(true);
        }

        else
        {
            Debug.LogError(wr.error);
        }
    }

    // 뒤끝 로그인 시도
    // 아이디가 없으면 자동으로 회원가입 실행 후 자동 로그인
    public void Login()
    {
        print("로그인 시도");

        string error = Backend.BMember.CustomLogin(id, pw).GetErrorCode();

        switch (error)
        {
            case "BadUnauthorizedException":
                print("아이디가 존재하지 않습니다.");
                SignUp();
                break;
            default:
                print("로그인 성공");
                SceneManager.LoadSceneAsync(1);
                break;
        }
    }

    // 뒤끝 회원가입
    void SignUp()
    {
        print("회원가입 시도");
        string error = Backend.BMember.CustomSignUp(id, pw).GetErrorCode();

        switch (error)
        {
            default:
                Debug.Log("회원가입 성공");
                Login();
                break;
        }
    }

    // 임시 공지 닫기
    public void offTempNotice() => tempNotice.gameObject.SetActive(false);

    // 공지 닫기
    public void offNotice() => noticeObj.gameObject.SetActive(false);
}
