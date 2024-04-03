using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerRankingSlot : MonoBehaviour
{
    public Text RankingTextUI;
    public Text NicknameTextUI;
    public Text KillCountTextUI;
    public Text ScoreTextUI;

    public void Set(Player player)
    {
        RankingTextUI.text = "1";
        NicknameTextUI.text = player.NickName;
        if (player.CustomProperties != null)
        {
            KillCountTextUI.text = $"{player.CustomProperties["KillCount"]}";
            ScoreTextUI.text = $"{player.CustomProperties["Score"]}";
        }
        else
        {
            KillCountTextUI.text = "0";
            ScoreTextUI.text = "0";
        }

    }
}
