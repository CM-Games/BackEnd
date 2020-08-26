using BackEnd;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("User Info")]
    public InputField ID;
    public InputField PW;
    public Button LoginBtn;

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

                LoginBtn.interactable = true;
                
            }
            // 초기화 실패한 경우 실행
            else
            {
                print("뒤끝 초기화 실패");
                LoginBtn.interactable = false;
            }
        });
    }

    public void Register()
    {
        BackendReturnObject BRO = Backend.BMember.CustomSignUp(ID.text, PW.text);

        if (BRO.IsSuccess()) print("동기방식 회원가입 성공");
        else Error(BRO.GetErrorCode(), "UserFunc");
    }

    // 동기방식 로그인
    public void Login()
    {
        LoginBtn.interactable = false;

        BackendReturnObject BRO = Backend.BMember.CustomLogin(ID.text, PW.text);

        if (BRO.IsSuccess())
        {
            print("동기방식 로그인 성공");
            SceneManager.LoadSceneAsync("Ingame");
        }
        else Register();
    }

    #region 예외처리
    // 에러 코드 확인
    void Error(string errorCode, string type)
    {
        if (errorCode == "DuplicatedParameterException")
        {
            if (type == "UserFunc") print("중복된 사용자 아이디 입니다.");
        }
        else if (errorCode == "BadUnauthorizedException")
        {
            if (type == "UserFunc") print("잘못된 사용자 아이디 혹은 비밀번호 입니다.");
        }

        LoginBtn.interactable = true;
    }
    #endregion
}
