using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using LitJson;
using System;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class ServerManager : MonoBehaviour
{
    [Header("Login & Register")]
    public InputField ID;
    public InputField PW;

    [Header("User Info")]
    public InputField Nickname;
    public InputField email;
    public InputField newPW;
    public Text userInfo;
    public Text loginState;
    string dataIndate;

    [Header("Game Manager")]
    public Transform tempNotice;
    public Transform Notice;
    public Transform Event;
    public InputField coupon;
    string linkURL;

    [Header("Social")]
    public InputField GamerNickname;
    public InputField FriendNickname;
    public Transform ReceivedFriendList;
    public Transform FriendList;
    string indate = null;

    Dictionary<string, int> weapon = new Dictionary<string, int>
{
    { "gun", 1 },
    { "knife", 5 },
    { "punch", 3 }
};

    // 비동기 회원가입 및 로그인을 구현 할때 사용할 변수
    BackendReturnObject bro = new BackendReturnObject();
    bool isSuccess = false;

    void Start()
    {
        // 초기화
        // [.net4][il2cpp] 사용 시 필수 사용
        Backend.Initialize(() =>
        {
            // 초기화 성공한 경우 실행
            if (Backend.IsInitialized)
            {
                print("뒤끝 초기화 성공");

                // example
                // 버전체크 -> 업데이트
            }
            // 초기화 실패한 경우 실행
            else
            {
                print("뒤끝 초기화 실패");
            }
        });
    }

    // 비동기 메소드는 update()문에서 SaveToken을 꼭 적용해야 합니다.
    void Update()
    {
        #region 회원가입 및 로그인(비동기)
        if (isSuccess)
        {
            // SaveToken( BackendReturnObject bro ) -> void
            // 비동기 메소드는 update()문에서 SaveToken을 꼭 적용해야 합니다.
            BackendReturnObject saveToken = Backend.BMember.SaveToken(bro);

            if (saveToken.IsSuccess())
            {
                print("비동기 로그인 성공");
                loginState.text = "로그인 상태 : 로그인";
            }
            else Error(bro.GetErrorCode(), "UserFunc");

            isSuccess = false;
            bro.Clear();
        }
        #endregion
    }

    #region 유저 관리

    #region 유저 회원가입, 로그인, 로그아웃, 회원탈퇴
    // 동기방식 회원가입
    public void Register()
    {
        BackendReturnObject BRO = Backend.BMember.CustomSignUp(ID.text, PW.text);

        if (BRO.IsSuccess()) print("동기방식 회원가입 성공");
        else Error(BRO.GetErrorCode(), "UserFunc");
    }

    // 동기방식 로그인
    public void Login()
    {
        BackendReturnObject BRO = Backend.BMember.CustomLogin(ID.text, PW.text);

        if (BRO.IsSuccess())
        {
            print("동기방식 로그인 성공");
            loginState.text = "로그인 상태 : 로그인";
        }
        else Error(BRO.GetErrorCode(), "UserFunc");
    }

    // 비동기 방식 회원가입
    public void RegisterAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.BMember.CustomSignUpAsync, ID.text, PW.text, (callback) =>
        {
            bro = callback;
            isSuccess = true;
        });
    }

    // 비동기 방식 로그인
    public void LoginAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.BMember.CustomLoginAsync, ID.text, PW.text, (callback) =>
        {
            bro = callback;
            isSuccess = true;
        });
    }

    // 동기 방식 로그아웃
    public void LogOut()
    {
        Backend.BMember.Logout();
        ID.text = PW.text = "";
        loginState.text = "로그인 상태 : 비로그인";
        print("동기 방식 로그아웃 성공");

    }

    // 비동기 방식 로그아웃
    public void LogOutAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.BMember.Logout, (callback) =>
        {
            if (callback.IsSuccess())
            {
                ID.text = PW.text = "";
                loginState.text = "로그인 상태 : 비로그인";
                print("비동기 방식 로그아웃 성공");
            }
        });
    }

    // 동기 방식 회원탈퇴
    public void SignOut()
    {
        // Backend.BMember.SignOut("reason");
        // 이유도 작성할 수 있습니다.
        Backend.BMember.SignOut();
        ID.text = PW.text = "";
        print("동기 방식 회원탈퇴 성공");
    }

    // 비동기 방식 회원탈퇴
    public void SignOutAsync()
    {
        // BackendAsyncClass.BackendAsync(Backend.BMember.SignOut, "reason", ( callback )
        // 비동기 방식도 이유를  작성할 수 있습니다.
        BackendAsyncClass.BackendAsync(Backend.BMember.SignOut, (callback) =>
        {
            if (callback.IsSuccess())
            {
                ID.text = PW.text = "";
                print("비동기 방식 회원탈퇴 성공");
            }
        });
    }
    #endregion // 유저 로그인~~~

    #region 유저 정보 관리
    // 동기 방식 유저 닉네임 생성
    public void CreateUserNickname()
    {
        BackendReturnObject BRO = Backend.BMember.CreateNickname(Nickname.text);

        if (BRO.IsSuccess()) print("동기 방식 닉네임 생성 완료");
        else Error(BRO.GetErrorCode(), "UserNickname");
    }

    // 동기 방식 유저 닉네임 업데이트
    public void UpdateUserNickname()
    {
        BackendReturnObject BRO = Backend.BMember.UpdateNickname(Nickname.text);

        if (BRO.IsSuccess()) print("동기 방식 닉네임 업데이트 완료");
        else Error(BRO.GetErrorCode(), "UserNickname");
    }

    // 비동기 방식 유저 닉네임 생성
    public void CreateUserNicknameAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.BMember.CreateNickname, Nickname.text, (callback) =>
        {
            if (callback.IsSuccess()) print("비동기 방식 닉네임 생성 완료");
            else Error(callback.GetErrorCode(), "UserNickname");
        });
    }

    // 비동기 방식 유저 닉네임 업데이트
    public void UpdateUserNicknameAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.BMember.UpdateNickname, Nickname.text, (callback) =>
        {
            if (callback.IsSuccess()) print("비동기 방식 닉네임 업데이트 완료");
            else Error(callback.GetErrorCode(), "UserNickname");
        });
    }

    // 동기 방식 유저 정보 가져오기
    public void getUserInfo()
    {
        BackendReturnObject BRO = Backend.BMember.GetUserInfo();

        JsonData returnJson = BRO.GetReturnValuetoJSON()["row"];

        userInfo.text = "nickName : " + returnJson["nickname"] +
                        "\ninDate : " + returnJson["inDate"].ToString() +
                        "\nsubscriptionType : " + returnJson["subscriptionType"].ToString() +
                        "\nemailForFindPassword : " + returnJson["emailForFindPassword"];
    }

    // 비동기 방식 유저 정보 가져오기
    public void getUserInfoAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.BMember.GetUserInfo, (callback) =>
        {
            JsonData returnJson = callback.GetReturnValuetoJSON()["row"];

            userInfo.text = "nickName : " + returnJson["nickname"] +
                            "\ninDate : " + returnJson["inDate"].ToString() +
                            "\nsubscriptionType : " + returnJson["subscriptionType"].ToString() +
                            "\nemailForFindPassword : " + returnJson["emailForFindPassword"];
        });
    }

    // 동기 방식 이메일 등록
    public void UpdateEmail()
    {
        BackendReturnObject BRO = Backend.BMember.UpdateCustomEmail(email.text);

        if (BRO.IsSuccess()) print("동기 방식 이메일 등록 완료");
    }

    // 비동기 방식 이메일 등록
    public void UpdateEmailAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.BMember.UpdateCustomEmail, email.text, callback =>
        {
            if (callback.IsSuccess()) print("비동기 방식 이메일 등록 완료");
        });
    }

    // 동기 방식 비밀번호 초기화
    public void ResetPW()
    {
        BackendReturnObject BRO = Backend.BMember.ResetPassword(ID.text, email.text);

        if (BRO.IsSuccess()) print("동기 방식 초기화된 비밀번호 발송 완료");
        else Error(BRO.GetErrorCode(), "UserPW");
    }

    // 동기 방식 비밀번호 변경
    public void UpdatePW()
    {
        BackendReturnObject BRO = Backend.BMember.UpdatePassword(PW.text, newPW.text);

        if (BRO.IsSuccess()) print("동기 방식 비밀번호 변경 완료");
        else Error(BRO.GetErrorCode(), "UserPW");
    }

    // 비동기 방식 비밀번호 초기화
    public void ResetPWAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.BMember.ResetPassword, ID.text, email.text, (callback) =>
        {
            if (callback.IsSuccess()) print("비동기 방식 초기화된 비밀번호 발송 완료");
            else Error(callback.GetErrorCode(), "UserPW");
        });
    }

    // 비동기 방식 비밀번호 변경
    public void UpdatePWAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.BMember.UpdatePassword, PW.text, newPW.text, (callback) =>
         {
             if (callback.IsSuccess()) print("비동기 방식 비밀번호 변경 완료");
             else Error(callback.GetErrorCode(), "UserPW");
         });
    }


    #endregion // 유저 정보 관리

    #endregion // 유저 관리

    #region 운영 관리

    #region 임시공지사항, 공지사항
    // 비동기 방식 임시 공지사항
    // 임시 공지사항은 비동기 방식으로만 작동합니다.
    public void getTempNotice()
    {
        Backend.Notice.GetTempNotice(callback =>
        {
            JsonData data = JsonMapper.ToObject(callback);

            bool isUse = (bool)data["isUse"];
            string contents = data["contents"].ToString();

            if (isUse)
            {
                print("임시 공지사항을 가져왔습니다.");
                tempNotice.GetChild(2).GetComponent<Text>().text = contents;
                tempNotice.gameObject.SetActive(true);
            }
            else print("임시 공지사항이 등록되어 있지 않습니다.");
        });
    }

    // 동기 방식 공지사항
    public void getNotice()
    {
        BackendReturnObject BRO = Backend.Notice.NoticeList();

        if (BRO.IsSuccess())
        {

            JsonData noticeData = BRO.GetReturnValuetoJSON()["rows"][0];

            string date = noticeData["postingDate"][0].ToString();
            string title = noticeData["title"][0].ToString();
            string content = noticeData["content"][0].ToString().Substring(0, 10);
            string imgURL = "http://upload-console.thebackend.io" + noticeData["imageKey"][0];
            linkURL = noticeData["linkUrl"][0].ToString();

            Notice.GetChild(6).GetComponent<Text>().text = date;
            Notice.GetChild(5).GetComponent<Text>().text = title;
            Notice.GetChild(7).GetComponent<Text>().text = content;
            StartCoroutine(WWWImageDown(imgURL, Notice.GetChild(4).GetComponent<Image>()));
            Notice.gameObject.SetActive(true);

            print("동기 방식 공지사항 받아오기 완료");
        }
    }

    // 비동기 방식 공지사항
    public void getNoticeAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.Notice.NoticeList, (callback) =>
        {
            if (callback.IsSuccess())
            {
                JsonData noticeData = callback.GetReturnValuetoJSON()["rows"][0];

                string date = noticeData["postingDate"][0].ToString();
                string title = noticeData["title"][0].ToString();
                string content = noticeData["content"][0].ToString().Substring(0, 10);
                string imgURL = "http://upload-console.thebackend.io" + noticeData["imageKey"][0];
                linkURL = noticeData["linkUrl"][0].ToString();

                Notice.GetChild(6).GetComponent<Text>().text = date;
                Notice.GetChild(5).GetComponent<Text>().text = title;
                Notice.GetChild(7).GetComponent<Text>().text = content;

                StartCoroutine(WWWImageDown(imgURL, Notice.GetChild(4).GetComponent<Image>()));
                Notice.gameObject.SetActive(true);
                print("비동기 방식 공지사항 받아오기 완료");
            }
        });
    }
    #endregion // 임시공지사항, 공지사항

    #region 이벤트, 쿠폰
    // 동기 방식 이벤트 받아오기
    public void getEvent()
    {
        BackendReturnObject BRO = Backend.Event.EventList();

        if (BRO.IsSuccess())
        {
            JsonData eventData = BRO.GetReturnValuetoJSON()["rows"][0];

            string title = eventData["title"][0].ToString();
            string contents = eventData["content"][0].ToString();
            string startDate = eventData["startDate"][0].ToString().Substring(0, 10);
            string endDate = eventData["endDate"][0].ToString().Substring(0, 10);
            string imgURL = "http://upload-console.thebackend.io" + eventData["contentImageKey"][0];

            Event.GetChild(1).GetComponent<Text>().text = title;
            Event.GetChild(2).GetComponent<Text>().text = contents;
            Event.GetChild(3).GetComponent<Text>().text = startDate + " ~ " + endDate;

            StartCoroutine(WWWImageDown(imgURL, Event.GetChild(5).GetComponent<Image>()));

            Event.gameObject.SetActive(true);
            print("동기 방식 이벤트 받아오기 완료");
        }
    }

    // 비동기 방식 이벤트 받아오기
    public void getEventAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.Event.EventList, (callback) =>
        {
            JsonData eventData = callback.GetReturnValuetoJSON()["rows"][0];

            string title = eventData["title"][0].ToString();
            string contents = eventData["content"][0].ToString();
            string startDate = eventData["startDate"][0].ToString().Substring(0, 10);
            string endDate = eventData["endDate"][0].ToString().Substring(0, 10);
            string imgURL = "http://upload-console.thebackend.io" + eventData["contentImageKey"][0];

            Event.GetChild(1).GetComponent<Text>().text = title;
            Event.GetChild(2).GetComponent<Text>().text = contents;
            Event.GetChild(3).GetComponent<Text>().text = startDate + " ~ " + endDate;

            StartCoroutine(WWWImageDown(imgURL, Event.GetChild(5).GetComponent<Image>()));

            Event.gameObject.SetActive(true);
            print("비동기 방식 이벤트 받아오기 완료");
        });
    }

    // 동기 방식 쿠폰 사용
    public void useCoupon()
    {
        BackendReturnObject BRO = Backend.Coupon.UseCoupon(coupon.text);

        if (BRO.IsSuccess()) print("쿠폰 사용 성공");
        else Error(BRO.GetErrorCode(), "Coupon");
    }

    // 비동기 방식 쿠폰 사용
    public void useCouponAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.Coupon.UseCoupon, coupon.text, (callback) =>
        {
            if (callback.IsSuccess()) print("쿠폰 사용 성공");
            else Error(callback.GetErrorCode(), "Coupon");
        });
    }
    #endregion // 이벤트, 쿠폰

    #region 이미지로드, 기타 함수
    // 이미지 로드
    IEnumerator WWWImageDown(string url, Image image)
    {
        UnityWebRequest wr = new UnityWebRequest(url);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        yield return wr.SendWebRequest();

        if (!(wr.isNetworkError || wr.isHttpError))
        {
            if (texDl.texture != null)
            {
                print("이미지 로드 완료");
                Texture2D t = texDl.texture;
                Sprite s = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.zero);
                image.sprite = s;
            }
        }
        else print("이미지가 없습니다.");
    }

    // 연결 링크가 있다면 링크 오픈
    public void openUrl()
    {
        if (linkURL != null)
        {
            Application.OpenURL(linkURL);
            print(linkURL + " 연결 완료");
        }
        else print("연결 할 링크가 없습니다");
    }
    #endregion // 이미지로드, 기타 함수

    #endregion // 운영관리

    #region 정보 관리
    // 동기 방식 정보 삽입
    public void insertData()
    {
        Param param = new Param();
        param.Add("exp", 100);
        param.Add("level", 30);
        param.Add("weapon", weapon);

        BackendReturnObject BRO = Backend.GameInfo.Insert("character", param);

        if (BRO.IsSuccess()) print("동기 방식 데이터 삽입 성공");
        else Error(BRO.GetErrorCode(), "gameData");

    }

    // 비동기 방식 정보 삽입
    public void insertDataAsync()
    {
        Param param = new Param();
        param.Add("exp", 100);
        param.Add("level", 30);
        param.Add("weapon", weapon);

        BackendAsyncClass.BackendAsync(Backend.GameInfo.Insert, "character", param, (callback) =>
        {
            if (callback.IsSuccess())
            {
                print("비동기 방식 데이터 삽입 성공");
            }
            else Error(callback.GetErrorCode(), "gameData");
        });
    }

    // 동기 방식 정보 읽기
    public void readData()
    {
        BackendReturnObject BRO = Backend.GameInfo.GetPrivateContents("character");

        if (BRO.IsSuccess())
        {
            JsonData jsonData = BRO.GetReturnValuetoJSON()["rows"][0];
            string level = jsonData["level"][0].ToString();
            string exp = jsonData["exp"][0].ToString();
            string gunLevel = jsonData["weapon"][0]["gun"][0].ToString();
            string knifeLevel = jsonData["weapon"][0]["knife"][0].ToString();
            string punchLevel = jsonData["weapon"][0]["punch"][0].ToString();

            dataIndate = jsonData["inDate"][0].ToString();

            print($"Level : {level}    Exp : {exp}");
            print($"Gun : LV.{gunLevel}    Knife : LV.{knifeLevel}    Punch : LV.{punchLevel}");
            print("동기 방식 정보 읽기 완료");
        }
        else Error(BRO.GetErrorCode(), "gameData");
    }

    // 비동기 방식 정보 읽기
    public void readDataAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.GameInfo.GetPrivateContents, "character", (callback) =>
        {
            if (callback.IsSuccess())
            {
                JsonData jsonData = callback.GetReturnValuetoJSON()["rows"][0];
                string level = jsonData["level"][0].ToString();
                string exp = jsonData["exp"][0].ToString();
                string gunLevel = jsonData["weapon"][0]["gun"][0].ToString();
                string knifeLevel = jsonData["weapon"][0]["knife"][0].ToString();
                string punchLevel = jsonData["weapon"][0]["punch"][0].ToString();

                dataIndate = jsonData["inDate"][0].ToString();

                print($"Level : {level}    Exp : {exp}");
                print($"Gun : LV.{gunLevel}    Knife : LV.{knifeLevel}    Punch : LV.{punchLevel}");
                print("비동기 방식 정보 읽기 완료");
            }
            else Error(callback.GetErrorCode(), "gameData");
        });
    }

    // 동기 방식 정보 수정
    public void updateData()
    {
        Param param = new Param();
        param.Add("exp", 110);
        param.Add("level", 31);
        weapon["gun"] = 11;
        param.Add("weapon", weapon);

        BackendReturnObject BRO = Backend.GameInfo.Update("character", dataIndate, param);

        if (BRO.IsSuccess()) print("동기 방식 정보 수정 성공");
        else print(BRO.GetErrorCode());
    }

    // 비동기 방식 정보 수정
    public void updateDataAsync()
    {
        Param param = new Param();
        param.Add("exp", 120);
        param.Add("level", 41);
        weapon["knife"] = 7;
        param.Add("weapon", weapon);

        BackendAsyncClass.BackendAsync(Backend.GameInfo.Update, "character", dataIndate, param, (callback) =>
        {
            if (callback.IsSuccess()) print("비동기 방식 정보 수정 성공");
            else print(callback.GetErrorCode());
        });
    }

    // 동기 방식 정보 삭제
    public void deleteData()
    {
        BackendReturnObject BRO = Backend.GameInfo.Delete("character", dataIndate);

        if (BRO.IsSuccess()) print("동기 방식 정보 삭제 성공");
        else Error(BRO.GetErrorCode(), "gameData");
    }

    // 비동기 방식 정보 삭제
    public void deleteDataAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.GameInfo.Delete, "character", dataIndate, (callback) =>
        {
            if (callback.IsSuccess()) print("비동기 방식 정보 삭제 성공");
            else Error(callback.GetErrorCode(), "gameData");
        });
    }
    #endregion // 정보 관리

    #region 소셜 기능
    // 동기 방식 유저 찾기
    public void getGammerIndate()
    {
        BackendReturnObject BRO = Backend.Social.GetGamerIndateByNickname(GamerNickname.text);

        if (BRO.IsSuccess())
        {
            JsonData GamerIndate = BRO.GetReturnValuetoJSON()["rows"][0];

            string indate = GamerIndate["inDate"][0].ToString();

            print($"[동기] {GamerNickname.text} 님의 inDate : {indate}");
        }
    }

    // 동기 방식 유저찾기 오버로드
    // 친구 요청을 할때 inDate 값이 필요하기 때문
    string getGammerIndate(string nickname)
    {
        BackendReturnObject BRO = Backend.Social.GetGamerIndateByNickname(nickname);

        if (BRO.IsSuccess())
        {
            JsonData GamerIndate = BRO.GetReturnValuetoJSON()["rows"][0];

            string indate = GamerIndate["inDate"][0].ToString();

            return indate;
        }
        return null;
    }

    // 비동기 방식 유저 찾기
    public void getGammerIndateAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.Social.GetGamerIndateByNickname, GamerNickname.text, (callback) =>
        {
            if (callback.IsSuccess())
            {
                JsonData GamerIndate = callback.GetReturnValuetoJSON()["rows"][0];

                string indate = GamerIndate["inDate"][0].ToString();

                print($"[비동기] {GamerNickname.text} 님의 inDate : {indate}");
            }
        });
    }

    // 동기 방식 친구 요청 리스트 조회
    public void getReceivedFriendList()
    {
        BackendReturnObject BRO = Backend.Social.Friend.GetReceivedRequestList();

        if (BRO.IsSuccess())
        {
            JsonData jsonData = BRO.GetReturnValuetoJSON()["rows"];

            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData Data = jsonData[i];

                string nickname = Data["nickname"][0].ToString();
                string inDate = Data["inDate"][0].ToString();

                for (int j = 0; j < ReceivedFriendList.childCount; j++)
                {
                    if (!ReceivedFriendList.GetChild(j).gameObject.activeSelf)
                    {
                        ReceivedFriendList.GetChild(j).GetChild(1).GetComponent<Text>().text = nickname;
                        ReceivedFriendList.GetChild(j).GetChild(2).GetComponent<Text>().text = inDate;
                        ReceivedFriendList.GetChild(j).gameObject.SetActive(true);
                        break;
                    }
                }
            }
            print("동기 방식 친구 요청 리스트 조회 성공");
        }
    }

    // 비동기 방식 친구 요청 리스트 조회
    public void getReceivedFriendListAsync()
    {
        Backend.Social.Friend.GetReceivedRequestList((callback) =>
        {
            if (callback.IsSuccess())
            {
                JsonData jsonData = callback.GetReturnValuetoJSON()["rows"];

                for (int i = 0; i < jsonData.Count; i++)
                {
                    JsonData Data = jsonData[i];

                    string nickname = Data["nickname"][0].ToString();
                    string inDate = Data["inDate"][0].ToString();

                    // 이후 처리
                }
                print("비동기 방식 친구 요청 리스트 조회 성공");
            }
            else print(callback.GetErrorCode());

        });
    }

    // 동기 방식 친구 리스트 조회
    public void getFriendList()
    {
        BackendReturnObject BRO = Backend.Social.Friend.GetFriendList();

        if (BRO.IsSuccess())
        {
            JsonData jsonData = BRO.GetReturnValuetoJSON()["rows"];

            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData Data = jsonData[i];

                string nickname = Data["nickname"][0].ToString();
                string indate = Data["inDate"][0].ToString();

                for (int j = 0; j < FriendList.childCount; j++)
                {
                    if (!FriendList.GetChild(j).gameObject.activeSelf)
                    {
                        FriendList.GetChild(j).GetChild(1).GetComponent<Text>().text = nickname;
                        FriendList.GetChild(j).GetChild(2).GetComponent<Text>().text = indate;
                        FriendList.GetChild(j).gameObject.SetActive(true);
                        break;
                    }
                }
            }

            print("동기 방식 친구 리스트 조회 성공");
        }
    }

    // 비동기 방식 친구 리스트 조회
    public void getFriendListAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.Social.Friend.GetFriendList, (callback) =>
        {
            if (callback.IsSuccess())
            {
                JsonData jsonData = callback.GetReturnValuetoJSON()["rows"];

                for (int i = 0; i < jsonData.Count; i++)
                {
                    JsonData Data = jsonData[i];

                    string nickname = Data["nickname"][0].ToString();
                    string indate = Data["inDate"][0].ToString();

                    // 이후처리
                }
                print("비동기 방식 친구 리스트 조회 성공");
            }
        });
    }

    // 동기 방식 친구 요청
    public void requestFriend()
    {
        BackendReturnObject BRO = Backend.Social.Friend.RequestFriend(getGammerIndate(FriendNickname.text));

        if (BRO.IsSuccess()) print($"동기 방식 {FriendNickname.text}님에게 친구요청 성공");
        else Error(BRO.GetErrorCode(), "Friend");

        FriendNickname.text = "";
    }

    // 비동기 방식 친구 요청
    public void requestFriendAsync()
    {
        BackendAsyncClass.BackendAsync(Backend.Social.Friend.RequestFriend, getGammerIndate(FriendNickname.text), (callback) =>
        {
            if (callback.IsSuccess()) print($"비동기 방식 {FriendNickname.text}님에게 친구요청 성공");
            else Error(callback.GetErrorCode(), "Friend");

            FriendNickname.text = "";
        });
    }

    // 동기 방식 친구 수락
    public void AcceptFriend()
    {
        string inDate = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(2).GetComponent<Text>().text;

        BackendReturnObject BRO = Backend.Social.Friend.AcceptFriend(inDate);

        if (BRO.IsSuccess())
        {
            EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
            print("동기 방식 친구 수락 완료");
        }
        else Error(BRO.GetErrorCode(), "Friend");
    }

    // 비동기 방식 친구 수락
    public void AcceptFriendAsync()
    {
        string inDate = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(2).GetComponent<Text>().text;

        BackendAsyncClass.BackendAsync(Backend.Social.Friend.AcceptFriend, inDate, (callback) =>
        {
            if (callback.IsSuccess())
            {
                EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
                print("비동기 방식 친구 수락 완료");
            }
            else Error(callback.GetErrorCode(), "Friend");
        });
    }

    // 동기 방식 친구 거절
    public void RejectFriend()
    {
        string inDate = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(2).GetComponent<Text>().text;

        BackendReturnObject BRO = Backend.Social.Friend.RejectFriend(inDate);

        if (BRO.IsSuccess())
        {
            EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
            print("동기 방식 친구 거절 완료");
        }
    }

    // 비동기 방식 친구 거절
    public void RejectFriendAsync()
    {
        string inDate = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(2).GetComponent<Text>().text;

        BackendAsyncClass.BackendAsync(Backend.Social.Friend.RejectFriend, inDate, (callback) =>
        {
            if (callback.IsSuccess())
            {
                EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
                print("비동기 방식 친구 거절 완료");
            }
        });
    }

    // 동기 방식 친구 삭제
    public void BreakFriend()
    {
        string inDate = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(2).GetComponent<Text>().text;

        BackendReturnObject BRO = Backend.Social.Friend.BreakFriend(inDate);

        if (BRO.IsSuccess())
        {
            EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
            print("동기 방식 친구 삭제 완료");
        }
    }

    // 비동기 방식 친구 삭제
    public void BreakFriendAsync()
    {
        string inDate = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(2).GetComponent<Text>().text;

        BackendAsyncClass.BackendAsync(Backend.Social.Friend.BreakFriend, inDate, (callback) =>
        {
            if (callback.IsSuccess())
            {
                EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
                print("비동기 방식 친구 삭제 완료");
            }
        });
    }
    #endregion


    #region 예외처리
    // 에러 코드 확인
    void Error(string errorCode, string type)
    {
        if (errorCode == "DuplicatedParameterException")
        {
            if (type == "UserFunc") print("중복된 사용자 아이디 입니다.");
            else if (type == "UserNickname") print("중복된 닉네임 입니다.");
            else if (type == "Friend") print("이미 요청되었거나 친구입니다.");
        }
        else if (errorCode == "BadUnauthorizedException")
        {
            if (type == "UserFunc") print("잘못된 사용자 아이디 혹은 비밀번호 입니다.");
        }
        else if (errorCode == "UndefinedParameterException")
        {
            if (type == "UserNickname") print("닉네임을 다시 입력해주세요");
        }
        else if (errorCode == "BadParameterException")
        {
            if (type == "UserNickname") print("닉네임 앞/뒤 공백이 있거나 20자 이상입니다.");
            else if (type == "UserPW") print("잘못된 이메일입니다.");
            else if (type == "gameData") print("잘못된 유형의 테이블 입니다.");
        }
        else if (errorCode == "NotFoundException")
        {
            if (type == "UserPW") print("등록된 이메일이 없습니다.");
            else if (type == "Coupon") print("중복 사용이거나 기간이 만료된 쿠폰입니다.");
            else if (type == "gameData") print("해당 테이블을 찾을 수 없습니다.");
        }
        else if (errorCode == "Too Many Request")
        {
            if (type == "UserPW") print("요청 횟수를 초과하였습니다. (1일 5회)");
        }
        else if (errorCode == "PreconditionFailed")
        {
            if (type == "gameData") print("해당 테이블은 비활성화 된 테이블 입니다.");
            else if (type == "Friend") print("받는 사람 혹은 보내는 사람의 요청갯수가 꽉 찬 상태입니다.");
        }
        else if (errorCode == "ServerErrorException")
        {
            if (type == "gameData") print("하나의 row이 400KB를 넘습니다");
        }
        else if (errorCode == "ForbiddenError")
        {
            if (type == "gameData") print("타인의 정보는 삭제가 불가능합니다.");
        }
    }
    #endregion
}
