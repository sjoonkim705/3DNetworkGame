using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Roominfo : MonoBehaviourPunCallbacks
{
    public static UI_Roominfo instance {  get; private set; }


    public Text RoomnameTextUI;
    public Text PlayerCountTextUI;
    public Text LogTextUI;

    private bool _init = false;
    private string _logText = string.Empty;

    private void Awake()
    {
        instance = this;

    }
    public override void OnJoinedRoom()
    { 
        if (!_init)
        {
            Init();
        }

    }
    private void Init()
    {
        _init = true;
        RoomnameTextUI.text = PhotonNetwork.CurrentRoom.Name;
        PlayerCountTextUI.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        _logText += "방에 입장하였습니다.\n";
        Refresh();

    }
    void Start()
    {
        if (!_init && PhotonNetwork.InRoom)
        {
            Init();
        }
    }

    private void Refresh()
    {
        LogTextUI.text = _logText;
        PlayerCountTextUI.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    // 새로운 플레이어가 입장했을 때 호출되는 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _logText += $"{newPlayer.NickName}님이 입장했습니다.\n";
        Refresh();
    }
    // 플레이어가 룸에서 퇴장했을 때 호출되는 콜백함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _logText += $"{otherPlayer.NickName}님이 퇴장했습니다.\n";
        Refresh();
    }

    [PunRPC]
    public void AddLog(string logMessage)
    {
        _logText += logMessage;
        Refresh();
    }
}
