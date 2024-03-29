using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class CharacterMotionAbility : CharacterAbility
{
    private Animator _animator;
    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _owner.PhotonView.RPC(nameof(PlayMotion), RpcTarget.All, 1);
        }
    }
    [PunRPC]
    private void PlayMotion(int number)
    {
        switch (number)
        {
            case 1:
                break;
            default:
                break;

        }

    }
}
