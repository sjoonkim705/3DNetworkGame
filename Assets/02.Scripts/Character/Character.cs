using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]

public class Character : MonoBehaviour
{
    public Stat Stat;
    public PhotonView PhotonView { get; private set; }


    private void Awake()
    {

        PhotonView = GetComponent<PhotonView>();
        if (PhotonView.IsMine)
        { 
            UI_CharacterStat.Instance.MyCharacter = this;
            Stat.OnHealthChanged += UI_CharacterStat.Instance.RefreshHealthUI;
            Stat.OnStaminaChanged += UI_CharacterStat.Instance.RefreshStaminaUI;
            Stat.Init();
        }
    }
}
