using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using UnityEngine.SceneManagement;
using LitJson;
using System;
using UnityEngine.Networking;

public static class userInfo{
    public static string inDate;
}

public class ServerManager : MonoBehaviour
{
    [Header("Title")]
    public Button startBtn;
    public Transform tempNotice;

    [Header("Main")]
    public Transform noticeObj;

    //  public static bool backendInit = false;

    string id = "ccm11441";
    string pw = "cjfals12";

    string serverTime;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            startBtn.interactable = false;
            Backend.Initialize(HandleBackendCallback);
        }
        else
        {
            Backend.Initialize(HandleBackendCallback);
            // getNoti();
        }
    }

    // 서버 초기화
    void HandleBackendCallback()
    {
        if (Backend.IsInitialized)
        {
            Debug.Log("뒤끝SDK 초기화 완료");
            if (SceneManager.GetActiveScene().name == "Title") startBtn.interactable = true;

            getTempNoti();
            // example 
            // 버전체크 -> 업데이트 

            // 구글 해시키 획득 
            //if (!Backend.Utils.GetGoogleHash().Equals(""))
            //Debug.Log(Backend.Utils.GetGoogleHash());

            // 서버시간 획득
            getTime();
        }
        // 실패
        else
        {
            Debug.LogError("Failed to initialize the backend");
        }
    }

    #region 서버 유틸(시간,서버상태, 기타 등등)
    void getTime()
    {
        JsonData data = JsonMapper.ToObject(Backend.Utils.GetServerTime().GetReturnValue());
        serverTime = data["utcTime"].ToString();
    }
    #endregion

    #region 서버 공지
    // 뒤끝 서버가 점검 혹은 오류가 있을떄
    // 긴급하게 임시로 공지를 띄움
    void getTempNoti()
    {
        Backend.Notice.GetTempNotice(callback =>
        {
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

    // 서버에 있는 공지를 동기 방식으로 받아옴
    // 이미지는 코루틴을 통해 받아옴
    public void getNoti()
    {
        BackendReturnObject bro = Backend.Notice.NoticeList();

        if (bro.IsSuccess())
        {

            JsonData noticeData = bro.GetReturnValuetoJSON()["rows"][0];

            string date = noticeData["postingDate"][0].ToString();
            print(date);
            //string title = noticeData["title"][0].ToString();
            //string content = noticeData["content"][0].ToString().Substring(0,10);
            //string URL = "http://upload-console.thebackend.io" + noticeData["imageKey"][0];

            //noticeObj.GetChild(0).GetComponent<Text>().text = date;
            //noticeObj.GetChild(1).GetComponent<Text>().text = title;
            //noticeObj.GetChild(2).GetComponent<Text>().text = content;
            //StartCoroutine(WWWImageDown(URL));

        }
    }

    // 비동기로 공지 로드
    public void getAsyncNoti()
    {
        BackendAsyncClass.BackendAsync(Backend.Notice.NoticeList, (callback) =>
         {
             if (callback.IsSuccess())
             {
                 JsonData noticeData = callback.GetReturnValuetoJSON()["rows"][0];

                 string date = noticeData["postingDate"][0].ToString();
                 string title = noticeData["title"][0].ToString();
                 string content = noticeData["content"][0].ToString().Substring(0, 10);
                 string URL = "http://upload-console.thebackend.io" + noticeData["imageKey"][0];

                 noticeObj.GetChild(0).GetComponent<Text>().text = date;
                 noticeObj.GetChild(1).GetComponent<Text>().text = title;
                 noticeObj.GetChild(2).GetComponent<Text>().text = content;

                 StartCoroutine(WWWImageDown(URL));
             }
         });
    }

    // 공지 이미지를 받아온 뒤 최종적으로 공지 활성화
    IEnumerator WWWImageDown(string url)
    {
        print("이미지 로드 중");
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

    // 임시 공지 닫기
    public void offTempNotice() => tempNotice.gameObject.SetActive(false);

    // 공지 닫기
    public void offNotice() => noticeObj.gameObject.SetActive(false);
    #endregion

    #region 로그인, 회원가입
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
               // getIndate();
                //  SceneManager.LoadSceneAsync(1);
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

    // 로그인 후 유저 Indate 값 가져옴
    public void getIndate()
    {
        BackendReturnObject bro = Backend.BMember.GetUserInfo();

        if (bro.IsSuccess())
        {
            JsonData indateJson = bro.GetReturnValuetoJSON()["row"];

            userInfo.inDate = indateJson["inDate"].ToString();
            print("user inDate : " + userInfo.inDate);

            print(Backend.GameInfo.GetTableList());
        }
    }
    #endregion

    #region 유저 정보 업데이트 및 로드

    // 유저 정보 최초 테이블 생성
    public void userInfoInit()
    {
        // 유저 돈
        Param param = new Param();
        param.Add("money", 0);
        param.Add("gravity", info.itemUpgradeValue[0]);

        // 정보 삽입
        Backend.GameInfo.Insert("character", param).GetReturnValue();


        print("유저 정보 생성 완료");
    }

    // 유저 정보 업데이트
    public void userInfoUpdate()
    {
        //string inDate =  Backend.GameInfo.Insert("character").GetReturnValue();

        // 유저 돈
        Param param = new Param();
        param.Add("money", info.rockCount);
        param.Add("gravity", info.itemUpgradeValue[0]);

        // 업데이트
        BackendReturnObject BRO = Backend.GameInfo.Update("character", userInfo.inDate, param);

        if (BRO.IsSuccess()) print("유저 정보 업데이트 완료");
        else
        {
            print(BRO.GetErrorCode());
        }
    }

    // 유저 정보 로드
    public void userInfoLoad()
    {
        BackendReturnObject bro = Backend.GameInfo.GetPrivateContents("character");

        if (bro.IsSuccess())
        {
            GetGameInfo(bro.GetReturnValuetoJSON());
        }
    }

    void GetGameInfo(JsonData returnData)
    {
        // ReturnValue가 존재하고, 데이터가 있는지 확인
        if (returnData != null)
        {
            Debug.Log("데이터가 존재합니다.");

            // rows 로 전달받은 경우 
            if (returnData.Keys.Contains("rows"))
            {
                JsonData rows = returnData["rows"];
                for (int i = 0; i < rows.Count; i++)
                {
                    GetData(rows[i]);
                }
            }

            // row 로 전달받은 경우
            else if (returnData.Keys.Contains("row"))
            {
                JsonData row = returnData["row"];
                GetData(row[0]);
            }
        }
        else
        {
            Debug.Log("데이터가 없습니다.");
        }
    }

    void GetData(JsonData data)
    {
        //var money = data["money"][0];
        //var gravity = data["gravity"][0];

        if (data.Keys.Contains("inDate"))
        {
            userInfo.inDate = data["inDate"][0].ToString();
            print("user inDate : " + userInfo.inDate);
        }


        //Debug.Log("money: " + money);
        //print("gravity: " + gravity);

        // 아래는 해당 키가 존재하는지 확인하고 데이터를 파싱하는 방법입니다. 
        if (data.Keys.Contains("money"))
        {
            info.rockCount = int.Parse(data["money"][0].ToString());
            UIManager.instance.setRockCount();
        }
        else
        {
            Debug.Log("존재하지 않는 키 입니다.");
        }

        // 해당 값이 배열로 저장되어 있을 경우는 아래와 같이 키가 존재하는지 확인합니다.
        if (data.Keys.Contains("equipItem"))
        {
            JsonData equipData = data["equipItem"][0];

            if (equipData.Keys.Contains("weapon"))
            {
                Debug.Log("weapon: " + equipData["weapon"][0]);
            }
            else
            {
                Debug.Log("존재하지 않는 키 입니다.");
            }
        }
    }
    #endregion
}
