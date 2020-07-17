﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using LitJson;

public class ServerManager : MonoBehaviour
{
    [Header("Login & Register")]
    public InputField ID;
    public InputField PW;

    [Header("User Info")]
    public InputField Nickname;
    public InputField email;
    public Text userInfo;

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

            if (saveToken.IsSuccess()) print("비동기 로그인 성공");
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

        if (BRO.IsSuccess()) print("동기방식 로그인 성공");
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


    #endregion // 유저 정보 관리

    #endregion // 유저 관리

    // 에러 코드 확인
    void Error(string errorCode, string type)
    {
        if (errorCode == "DuplicatedParameterException")
        {
            if (type == "UserFunc") print("중복된 사용자 아이디 입니다.");
            else if (type == "UserNickname") print("중복된 닉네임 입니다.");
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
        }
    }
}
