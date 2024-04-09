using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Score : MonoBehaviour
{
    public static UI_Score Instance { get; private set; }
    public Character Character;
    public Text ScoreText;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        RefreshScoreUI();
    }
    public void RefreshScoreUI()
    {
        ScoreText.text = $"{Character.Score.ToString():D3}";

    }
}
