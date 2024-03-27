using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public CharacterAttackAbility MyCharacterAttackAbility;
    public void OnTriggerEnter(Collider other)
    {
        MyCharacterAttackAbility.OnTriggerEnter(other);
    }

}
