using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using LitJson;

public static class PlayerInfo
{
    public static string nickname;
    public static string eMail;
}

public class IngameManager : MonoBehaviour
{
    [Header("setUserInfo")]
    public GameObject setUserInfoUI;
    public InputField nickname;
    public InputField email;
    public Text stateText;
    public Text playerNIckname;


    private void Start()
    {
        Init();
    }

    void Init()
    {
        getUserInfo();

    }

    // 동기 방식 유저 정보 가져오기
    public void getUserInfo()
    {
        BackendReturnObject BRO = Backend.BMember.GetUserInfo();

        JsonData returnJson = BRO.GetReturnValuetoJSON()["row"];

        if (returnJson["nickname"] == null)
        {           
            setUserInfoUI.SetActive(true);
        }
        else
        {
            PlayerInfo.nickname = returnJson["nickname"].ToString();
            PlayerInfo.eMail = returnJson["emailForFindPassword"].ToString();
            playerNIckname.text = PlayerInfo.nickname;
            PlayerControler.instance.lockedMove = false;
        }
    }

    public void setUserInfo()
    {
        if(nickname.text == "" || email.text == "")
        {
            stateText.text = "모든 칸을 채워주세요!";
            return;
        }

        if (UpdateUserNickname(nickname.text))
            if (UpdateEmail(email.text))
            {
                PlayerControler.instance.lockedMove = true;
                setUserInfoUI.SetActive(false);
            }

    }


    // 동기 방식 이메일 등록
    bool UpdateEmail(string Email)
    {
        BackendReturnObject BRO = Backend.BMember.UpdateCustomEmail(Email);

        if (BRO.IsSuccess()) return true;

        return false;
    }

    // 동기 방식 유저 닉네임 업데이트
    bool UpdateUserNickname(string nickname)
    {
        BackendReturnObject BRO = Backend.BMember.UpdateNickname(nickname);

        if (BRO.IsSuccess()) return true;
        else
        {
            Error(BRO.GetErrorCode(), "UserNickname");
            return false;
        }
    }

    #region 예외처리
    // 에러 코드 확인
    void Error(string errorCode, string type)
    {
        if (errorCode == "DuplicatedParameterException")
        {
            if (type == "UserFunc") print("중복된 사용자 아이디 입니다.");
            else if (type == "UserNickname") stateText.text = "중복된 닉네임 입니다!";
            else if (type == "Friend") print("이미 요청되었거나 친구입니다.");
        }
        else if (errorCode == "BadUnauthorizedException")
        {
            if (type == "UserFunc") print("잘못된 사용자 아이디 혹은 비밀번호 입니다.");
            else if (type == "Message") print("잘못된 닉네임입니다.");
        }
        else if (errorCode == "UndefinedParameterException")
        {
            if (type == "UserNickname") stateText.text = "닉네임을 다시 입력해주세요";
        }
        else if (errorCode == "BadParameterException")
        {
            if (type == "UserNickname") stateText.text = "닉네임 앞/뒤 공백이 있거나 20자 이상입니다.";
            else if (type == "UserPW") print("잘못된 이메일입니다.");
            else if (type == "gameData") print("잘못된 유형의 테이블 입니다.");
            else if (type == "Message") print("보내는 사람, 받는 사람이 같습니다.");
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
            else if (type == "Message") print("설정한 글자수를 초과하였습니다.");
        }
        else if (errorCode == "ServerErrorException")
        {
            if (type == "gameData") print("하나의 row이 400KB를 넘습니다");
        }
        else if (errorCode == "ForbiddenError")
        {
            if (type == "gameData") print("타인의 정보는 삭제가 불가능합니다.");
            else if (type == "Message") print("콘솔에서 쪽지 최대보유수를 설정해주세요");
        }
        else if (errorCode == "MethodNotAllowedParameterException")
        {
            if (type == "Message") print("상대방의 쪽지가 가득 찾습니다.");
        }
    }
    #endregion
}
