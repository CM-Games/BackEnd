using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;

public class ServerManager : MonoBehaviour
{
    [Header("Login & Register")]
    public InputField ID;
    public InputField PW;

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
            else Error(bro.GetErrorCode());

            isSuccess = false;
            bro.Clear();
        }
        #endregion
    }

    #region 유저 관리

    // 동기방식 회원가입
    public void Register()
    {
        BackendReturnObject BRO = Backend.BMember.CustomSignUp(ID.text, PW.text);

        if (BRO.IsSuccess()) print("동기방식 회원가입 성공");
        else Error(BRO.GetErrorCode());
    }

    // 동기방식 로그인
    public void Login()
    {
        BackendReturnObject BRO = Backend.BMember.CustomLogin(ID.text, PW.text);

        if (BRO.IsSuccess()) print("동기방식 로그인 성공");
        else Error(BRO.GetErrorCode());
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
        BackendAsyncClass.BackendAsync(Backend.BMember.Logout, (callback) => {
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

    #endregion

    // 에러 코드 확인
    void Error(string errorCode)
    {
        switch (errorCode)
        {
            case "DuplicatedParameterException":
                print("중복된 사용자 아이디 입니다.");
                break;
            case "BadUnauthorizedException":
                print("잘못된 사용자 아이디 혹은 비밀번호 입니다.");
                break;
            default:
                break;
        }
    }
}
