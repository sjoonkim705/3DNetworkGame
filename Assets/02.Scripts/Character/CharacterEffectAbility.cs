using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectAbility : CharacterAbility
{
    public List<ParticleSystem> ItemEffects;

    public void RequestPlay(int effectIndex)
    {
        _owner.PhotonView.RPC(nameof(Play), RpcTarget.All, effectIndex);
    }    

    [PunRPC]
    private void Play(int effectIndex)
    {
        ItemEffects[effectIndex].Play();
    }

}
