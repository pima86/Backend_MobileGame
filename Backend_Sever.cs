using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using UnityEngine;
using BackEnd;
using TMPro;

public class Login : MonoBehaviour
{
    public static Login Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] TMP_Text user_name;
    private GameObject obj;

    void Start()
    {
        BackendReturnObject bro = Backend.BMember.LoginWithTheBackendToken();
        if (bro.IsSuccess())
        {
            DebugLog.Inst.Log("자동 로그인에 성공했습니다");

            obj = GameObject.Find("SetManager_로그인화면").gameObject;
            obj.GetComponent<SetFalse>().OnMouseDown(false);

            if (isGuest())
                user_name.text = "현재 게스트 계정으로 로그인되어있습니다.\n1년 이상의 미접속 혹은 로그아웃 이후 계정을 복구하실 수 없습니다.\n온전한 데이터 보관을 위해 구글과 연동하시는 걸 추천드립니다.";
        }
        else
        {
            obj = GameObject.Find("SetManager_로그인화면").gameObject;
            obj.GetComponent<SetFalse>().OnMouseDown(true);
        }
    }

    public bool isGuest()
    {
        string id = Backend.BMember.GetGuestID();
        if (id != "" && id != null)
            return true;
        else
            return false;
    }

    #region 로그아웃
    public void LogOut()
    {
        if (isGuest())
        {
            Backend.BMember.DeleteGuestInfo();
            Backend.BMember.Logout(); //로그아웃
        }
        else
            PlayGamesPlatform.Instance.SignOut(); //구글 로그아웃


        obj = GameObject.Find("SetManager_로그인화면").gameObject;
        obj.GetComponent<SetFalse>().OnMouseDown(true);

        user_name.text = "";
    }
    #endregion

    #region 구글 로그인
    public void GoogleAuth()
    {
        // GPGS 플러그인 설정
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail() // 이메일 권한을 얻고 싶지 않다면 해당 줄(RequestEmail)을 지워주세요.
            .RequestIdToken()
            .Build();
        //커스텀 된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true; // 디버그 로그를 보고 싶지 않다면 false로 바꿔주세요.
                                                  //GPGS 시작.
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                // 로그인 성공 -> 뒤끝 서버에 획득한 구글 토큰으로 가입 요청
                BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");

                obj = GameObject.Find("SetManager_로그인화면").gameObject;
                obj.GetComponent<SetFalse>().OnMouseDown(false);

                DebugLog.Inst.Log("구글 로그인 성공");
                //string tro = Story_Pointer.Inst.GameInDate("");
            }
            else
            {
                // 로그인 실패
                DebugLog.Inst.Log("Login failed for some reason");
            }
        });
    }

    string GetTokens()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
            return _IDtoken;
        }
        else
        {
            DebugLog.Inst.Log("접속되어 있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
    }
    #endregion

    public void CustomSignUp()
    {
        BackendReturnObject bro = Backend.BMember.GuestLogin("게스트 로그인으로 로그인함");
        if (bro.IsSuccess())
        {
            Debug.Log("게스트 로그인");
            obj = GameObject.Find("SetManager_로그인화면").gameObject;
            obj.GetComponent<SetFalse>().OnMouseDown(false);

            user_name.text = "현재 게스트 계정으로 로그인되어있습니다.\n1년 이상의 미접속 혹은 로그아웃 이후 계정을 복구하실 수 없습니다.\n온전한 데이터 보관을 위해 구글과 연동하시는 걸 추천드립니다.";
        }
        else
            DebugLog.Inst.Log("회원가입에 실패했습니다.:" + bro);
    }
}
