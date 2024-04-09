using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class UI_PlayerRanking : MonoBehaviourPunCallbacks
{
    public List<UI_PlayerRankingSlot> Slots; // 1 ~ 5등
    public UI_PlayerRankingSlot MySlot;

    public override void OnJoinedRoom()
    {
        Refresh();

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Refresh();
    }
    public override void OnLeftRoom()
    {
        Refresh();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Refresh();
    }
    private void Refresh()
    {
        Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
        List<Player> playerList = players.Values.ToList<Player>();
        playerList.RemoveAll(player => !player.CustomProperties.ContainsKey("Score"));

        playerList.Sort((player1, player2) =>
        {
            int player1Score = (int)player1.CustomProperties["Score"];
            int player2Score = (int)player2.CustomProperties["Score"];
            return player2Score.CompareTo(player1Score);

        });

        int playerCount = Mathf.Min(playerList.Count, 5);

        foreach (UI_PlayerRankingSlot slot in Slots)
        {
            slot.gameObject.SetActive(false);
        }
        for (int i = 0; i < playerCount; i++)
        {
            Slots[i].Rank = i + 1;
            Slots[i].gameObject.SetActive(true);
            Slots[i].Set(playerList[i]);
        }
        MySlot.Set(PhotonNetwork.LocalPlayer);
    }

}
