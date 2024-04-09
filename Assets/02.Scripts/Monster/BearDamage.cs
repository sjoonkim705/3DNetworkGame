using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearDamage : MonoBehaviourPun, IDamaged
{
    public Bear Owner;

    private void Awake()
    {
        //Owner = GetComponentInParent<Bear>();
    }
    [PunRPC]
    public void Damaged(int damage, int actorNumber)
    {
        Owner.Stat.Health -= damage;
        Owner.RequestPlayAnimation("IsHit");

        if (Owner.Stat.Health <= 0)
        {
            Owner.RequestPlayAnimation("Die");
        }
    }
}
