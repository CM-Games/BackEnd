using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using BackEnd.Tcp;
using LitJson;
using Protocol;

public class BackendMatch : MonoBehaviour
{
    // 디버그 로그
    private string NOTCONNECT_MATCHSERVER = "매치 서버에 연결되어 있지 않습니다.";
    private string RECONNECT_MATCHSERVER = "매치 서버에 접속을 시도합니다.";
    private string FAIL_CONNECT_MATCHSERVER = "매치 서버 접속 실패 : {0}";
    private string SUCCESS_CONNECT_MATCHSERVER = "매치 서버 접속 성공";
    private string SUCCESS_MATCHMAKE = "매칭 성공 : {0}";
    private string SUCCESS_REGIST_MATCHMAKE = "매칭 대기열에 등록되었습니다.";
    private string FAIL_REGIST_MATCHMAKE = "매칭 실패 : {0}";
    private string CANCEL_MATCHMAKE = "매칭 신청 취소 : {0}";
    private string INVAILD_MATCHTYPE = "잘못된 매치 타입입니다.";
    private string INVALID_MODETYPE = "잘못된 모드 타입입니다.";
    private string INVALID_OPERATION = "잘못된 요청입니다\n{0}";
    private string EXCEPTION_OCCUR = "예외 발생 : {0}\n다시 매칭을 시도합니다.";

    public Text Log;
    string inGameRoomToken;
    private bool isSetHost = false;
    public List<SessionId> sessionIdList { get; private set; }  // 매치에 참가중인 유저들의 세션 목록
    // public Dictionary<SessionId, int> teamInfo { get; private set; }    // 매치에 참가중인 유저들의 팀 정보 (MatchModeType이 team인 경우에만 사용)
    public Dictionary<SessionId, MatchUserGameRecord> gameRecords { get; private set; } = null;  // 매치에 참가중인 유저들의 매칭 기록
    public SessionId hostSession { get; private set; }  // 호스트 세션

    #region Host
    private bool isHost = false;                    // 호스트 여부 (서버에서 설정한 SuperGamer 정보를 가져옴)
    private Queue<KeyMessage> localQueue = null;    // 호스트에서 로컬로 처리하는 패킷을 쌓아두는 큐 (로컬처리하는 데이터는 서버로 발송 안함)
    #endregion

    bool isConnectMatchServer = false;

    private void Start()
    {
        MatchMakingHandler();
    }

    IEnumerator UpdateEvent()
    {
        while (true)
        {
            if (Backend.Match.Poll() > 0)
            {
                Log.text += "서버로 부터 이벤트 받음\n";
                print("서버로 부터 이벤트 받음");
            }

            yield return null;
        }
    }

    public void JoinMatchServer()
    {
        
        if (isConnectMatchServer)
        {
            return;
        }

        ErrorInfo errorInfo;
        isConnectMatchServer = true;

        if (!Backend.Match.JoinMatchMakingServer(out errorInfo))
        {
            var errorLog = string.Format(FAIL_CONNECT_MATCHSERVER, errorInfo.ToString());
            Debug.Log(errorLog);
        }
        else
        {
            //  print("서버 접속");
            StartCoroutine(UpdateEvent());
        }
    }

    // 매칭 대기 방 만들기
    // 혼자 매칭을 하더라도 무조건 방을 생성한 뒤 매칭을 신청해야 함
    public void CreateMatchRoom()
    {
        // 매청 서버에 연결되어 있지 않으면 매칭 서버 접속
        if (!isConnectMatchServer)
        {
            Debug.Log(NOTCONNECT_MATCHSERVER);
            Debug.Log(RECONNECT_MATCHSERVER);
            JoinMatchServer();
            return;
        }
        Log.text += "방 생성 요청을 서버로 보냄\n";
        Debug.Log("방 생성 요청을 서버로 보냄");
        Backend.Match.CreateMatchRoom();

    }

    // 매칭
    public void RequestMatch()
    {
        BackendReturnObject BRO = Backend.Match.GetMatchList();

        JsonData jsonData = BRO.GetReturnValuetoJSON()["rows"][0];
        var inDate = jsonData["inDate"]["S"].ToString();

        Backend.Match.RequestMatchMaking(MatchType.Random, MatchModeType.OneOnOne, inDate);
    }

    // 매칭 성공
    public void MatchSuccess(MatchMakingResponseEventArgs args)
    {
        ErrorInfo errorInfo;
        inGameRoomToken = args.RoomInfo.m_inGameRoomToken;

        Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address, args.RoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo);

    }

    //게임시작
    private void GameSetup()
    {
        Log.text += "게임 시작 메시지 수신. 게임 설정 시작\n";
        OnGameReady();
    }

    // 게임 레디 상태일 때 호출됨
    public void OnGameReady()
    {
        if (isSetHost == false)
        {
            // 호스트가 설정되지 않은 상태이면 호스트 설정
            isSetHost = SetHostSession();
        }
        Log.text += "호스트 설정 완료";


        if (IsHost() == true)
        {
            Log.text += "씬 전환 명령";
            SendDataToInGame(new Protocol.LoadRoomSceneMessage());           
        }
    }

    public void SendDataToInGame<T>(T msg)
    {
        var byteArray = DataParser.DataToJsonData<T>(msg);
        Backend.Match.SendDataToInGameRoom(byteArray);
    }

    public bool IsHost()
    {
        return isHost;
    }


    private bool SetHostSession()
    {
        // 호스트 세션 정하기
        // 각 클라이언트가 모두 수행 (호스트 세션 정하는 로직은 모두 같으므로 각각의 클라이언트가 모두 로직을 수행하지만 결과값은 같다.)

        Log.text += "호스트 세션 설정 진입";
        // 호스트 세션 정렬 (각 클라이언트마다 입장 순서가 다를 수 있기 때문에 정렬)
        sessionIdList.Sort();
        isHost = false;

        // 내가 호스트 세션인지
        foreach (var record in gameRecords)
        {
            if (record.Value.m_isSuperGamer == true)
            {
                if (record.Value.m_sessionId.Equals(Backend.Match.GetMySessionId()))
                {
                    isHost = true;
                }
                hostSession = record.Value.m_sessionId;
                break;
            }
        }

        Log.text += "호스트 여부 : " + isHost + "\n";

        // 호스트 세션이면 로컬에서 처리하는 패킷이 있으므로 로컬 큐를 생성해준다
        if (isHost)
        {
            localQueue = new Queue<KeyMessage>();
        }
        else
        {
            localQueue = null;
        }

        // 호스트 설정까지 끝나면 매치서버와 접속 끊음
        Backend.Match.LeaveMatchMakingServer();
        return true;
    }

    private void MatchMakingHandler()
    {
        Backend.Match.OnJoinMatchMakingServer += (args) =>
        {
            Debug.Log("OnJoinMatchMakingServer : " + args.ErrInfo);
            // 매칭 서버에 접속하면 호출
            ProcessAccessMatchMakingServer(args.ErrInfo);
        };

        Backend.Match.OnMatchMakingResponse += (args) =>
        {
            Debug.Log("Match : " + args.ErrInfo);

            ProcessAccessMatchMaking(args);
        };

        Backend.Match.OnSessionListInServer += (args) =>
        {
            ProcessAccessInGame(args);
        };

        Backend.Match.OnSessionJoinInServer += (args) =>
        {
            ProcessAccessInGameServer(args);
        };

        Backend.Match.OnMatchInGameStart += () =>
        {
            // 서버에서 게임 시작 패킷을 보내면 호출
            GameSetup();
        };
    }

    void ProcessAccessInGameServer(JoinChannelEventArgs args)
    {
        Log.text += "인게임 서버 접속 성공\n";
        Backend.Match.JoinGameRoom(inGameRoomToken);
    }


    void ProcessAccessInGame(MatchInGameSessionListEventArgs args)
    {
        switch (args.ErrInfo)
        {
            case ErrorCode.Success:
                Log.text += "방입장 완료\n";

                sessionIdList = new List<SessionId>();
                gameRecords = new Dictionary<SessionId, MatchUserGameRecord>();

                foreach (var record in args.GameRecords)
                {
                    sessionIdList.Add(record.m_sessionId);
                    gameRecords.Add(record.m_sessionId, record);
                }
                sessionIdList.Sort();

                break;

            default:
                break;
        }
    }


    private void ProcessAccessMatchMaking(MatchMakingResponseEventArgs args)
    {
        switch (args.ErrInfo)
        {
            case ErrorCode.Success:
                Log.text += "매칭성사! 곧 이동됩니다!\n";
                MatchSuccess(args);
                break;
            case ErrorCode.Match_InProgress:
                Log.text += "매칭중...\n";
                break;
            default:
                break;
        }
    }

    // 매칭 서버 접속에 대한 리턴값
    private void ProcessAccessMatchMakingServer(ErrorInfo errInfo)
    {
        if (errInfo != ErrorInfo.Success)
        {
            // 접속 실패
            isConnectMatchServer = false;
        }

        if (!isConnectMatchServer)
        {
            var errorLog = string.Format(FAIL_CONNECT_MATCHSERVER, errInfo.ToString());
            // 접속 실패
            Debug.Log(errorLog);
        }
        else
        {
            //접속 성공
            Log.text += "매칭 서버 접속 성공\n";
            Debug.Log(SUCCESS_CONNECT_MATCHSERVER);
        }
    }
}
