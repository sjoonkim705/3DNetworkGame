using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviourPun
{
    public Character _character;

    public CharacterAttackAbility MyCharacterAttackAbility;
    private Vector3 _orginalScale;
    private int _scaleUpLevel;
    private void Awake()
    {
        _character = GetComponentInParent<Character>();
    }
    private void Start()
    {
        _orginalScale = transform.localScale;
        _scaleUpLevel = 0;
    }

    public void OnTriggerEnter(Collider other)
    {
        MyCharacterAttackAbility.OnTriggerEnter(other);
    }

    private void Update()
    {
        SetWeaponScaleLevel(_character.Score/300);
    }
    public void ResetScale()
    {
        _scaleUpLevel = 1;
        transform.localScale = _orginalScale * Mathf.Pow(1.1f, (float)_scaleUpLevel);
    }
    public void SetWeaponScaleLevel(int scaleUpStage)
    {
        transform.localScale = _orginalScale * Mathf.Pow(1.1f, (float)scaleUpStage);
    }
}
