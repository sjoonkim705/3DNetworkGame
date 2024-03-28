using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DamageType
{
    Normal,
    Critical
}
public struct DamageInfo
{
    public DamageType DamageType;     // 0:일반, 1:크리티컬
    public int Amount;
    public Vector3 Position;
    public Vector3 Normal;

    public DamageInfo(DamageType damagetype, int amount)
    {
        this.DamageType = damagetype;
        this.Amount = amount;
        this.Position = Vector3.zero;
        this.Normal = Vector3.zero;
    }

}
