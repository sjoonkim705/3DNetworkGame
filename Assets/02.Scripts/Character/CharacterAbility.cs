using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAbility : MonoBehaviour
{
    protected Character _owner {get;private set;}
    protected virtual void Awake()
    {
        _owner = GetComponent<Character>();
    }
}
