using BackEnd;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("Login")]
    public GameObject errorUI;
    public InputField ID;
    public InputField PW;
    public Button LoginBtn;
    public Button sighUPBtn;

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

                ButtonOff(1);

            }
            // 초기화 실패한 경우 실행
            else
            {
                print("뒤끝 초기화 실패");
                ButtonOff(0);
            }
        });
    }

    public void SignUp()
    {
        BackendReturnObject BRO = Backend.BMember.CustomSignUp(ID.text, PW.text);

        if (BRO.IsSuccess()) ErrorUIOn("동기방식 회원가입 성공");
        else Error(BRO.GetErrorCode(), "UserFunc");
    }

    // 동기방식 로그인
    public void Login()
    {
        ButtonOff(0);

        BackendReturnObject BRO = Backend.BMember.CustomLogin(ID.text, PW.text);

        if (BRO.IsSuccess()) SceneManager.LoadSceneAsync("Ingame");
        else Error(BRO.GetErrorCode(), "UserFunc");
    }

    // 버튼 활성화 / 비활성화
    void ButtonOff(int type)
    {
        if (type == 0)
        {
            LoginBtn.interactable = false;
            sighUPBtn.interactable = false;
        }
        else
        {
            LoginBtn.interactable = true;
            sighUPBtn.interactable = true;
        }
    }

    // UI 끄는 함수
    public void UIOFF(int type)
    {
        if (type == 0) errorUI.SetActive(false);
    }

    // 에러 메시지 표현
    void ErrorUIOn(string message)
    {
        errorUI.transform.GetChild(1).GetComponent<Text>().text = message;
        errorUI.SetActive(true);
    }


    #region 예외처리
    // 에러 코드 확인
    void Error(string errorCode, string type)
    {
        string errorMessage = "";

        if (errorCode == "DuplicatedParameterException")
        {
            if (type == "UserFunc") errorMessage = "중복된 사용자 아이디 입니다.";
        }
        else if (errorCode == "BadUnauthorizedException")
        {
            if (type == "UserFunc") errorMessage = "잘못된 사용자 아이디 혹은 비밀번호 입니다.";
        }

        ErrorUIOn(errorMessage);


        ButtonOff(1);
    }
    #endregion


}
