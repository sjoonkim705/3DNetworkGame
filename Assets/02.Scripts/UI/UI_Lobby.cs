using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour
{
    public InputField NickNameInputFieldUI;
    public InputField RoomIDInputFieldUI;

    public bool HasFemaleSelected;
    public bool HasMaleSelected;
    public GameObject MaleCharacter;
    public GameObject FemaleCharacter;


    private void Start()
    {
        FemaleCharacter.SetActive(true);
        HasFemaleSelected = true;
        HasMaleSelected = false;
        MaleCharacter.SetActive(false);
        PhotonManager.instance.HasMaleCharacterSelected = false;
    }


    public void OnClickedMakeRoomButton()
    {
        string nickname = NickNameInputFieldUI.text;
        string roomID = RoomIDInputFieldUI.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(roomID))
        {
            Debug.Log("입력하세요");
            return;
        }
        PhotonNetwork.NickName = nickname;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20; // 입장가능한 최대 플레이어수
        roomOptions.IsVisible = true; // 로비에서 방 목록에 노출할 것인가?
        roomOptions.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom(roomID, roomOptions, TypedLobby.Default);
    }

    public void OnClickedMaleButton()
    {
        if (HasMaleSelected)
        {
            return;
        }
        else
        {
            HasMaleSelected = true;
            HasFemaleSelected = false;
            MaleCharacter.SetActive(true);
            FemaleCharacter.SetActive(false);

            PhotonManager.instance.HasMaleCharacterSelected = true;

        }

    }
    public void OnClickedFemaleButton()
    {
        if (HasFemaleSelected)
        {
            return;
        }
        else
        {
            HasMaleSelected = false;
            HasFemaleSelected = true;
            MaleCharacter.SetActive(false);
            FemaleCharacter.SetActive(true);
            PhotonManager.instance.HasMaleCharacterSelected = false;
        }

    }
}
