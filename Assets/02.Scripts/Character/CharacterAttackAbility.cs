using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAttackAbility : CharacterAbility
{

    // SOLID 법칙 : 객체지향 5가지 법칙
    // 1. 단일 책임 원칙(가장 단순하지만 곡 지켜야 하는 원칙
    //     - 클래스는 단 한개의 책임을 가져야 한다.
    //     - 클래스를 변경하는 이유는 단 하나여야 한다.
    //     - 이를 지키지 않으면 한 책임 변경에 의해 다른 책임과 관련된 다른 코드도 영향을 미칠 수 있어서
    //      -> 유지보수가 어려움

    // 준수 전략
    // - 기존에 클래스로 해결할 수 없다면 새로운 클래스를 구현
    // 
    public float StaminaConsumeFactor = 20f;
    private float _attackTimer = 0;
    private Animator _animator;
    public Collider WeaponCollider;
    public TrailRenderer WeaponTrail;


    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!_owner.PhotonView.IsMine)
        {
            return;
        }
        if (_attackTimer < _owner.Stat.AttackCoolTime)
        {
            _attackTimer += Time.deltaTime;
        }
        bool haveStamina = (_owner.Stat.Stamina >= StaminaConsumeFactor);
        if (Input.GetMouseButtonDown(0) && _attackTimer > _owner.Stat.AttackCoolTime && haveStamina)
        {
            _owner.Stat.Stamina -= StaminaConsumeFactor;
            _owner.PhotonView.RPC(nameof(PlayAttackAnimation), RpcTarget.All, Random.Range(1, 4));
            // RpcTarget.All : 모두에게
            // RpcTarget.Others : 나자신 제외 모두에게
            // RpcTarget.Master : 방장에게만
            _attackTimer = 0;
        }
    }

    [PunRPC]
    public void PlayAttackAnimation(int index)
    {
        //int animationRandFactor = Random.Range(1, 4);
        _animator.SetTrigger($"Attack{index}");

    }
    public void OnTriggerEnter(Collider other)
    {
        // 0: 개방 폐쇄 원칙 + 인터페이스
        // 수정에는 닫혀있고ㅓ, 확장에는 열려있다.
        if (!_owner.PhotonView.IsMine || other.transform == transform)
        {
            return;
        }
        IDamaged damageableObject = other.GetComponent<IDamaged>();
        if (damageableObject != null)
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null)
            {
                photonView.RPC("Damaged", RpcTarget.All, _owner.Stat.Damage);

            }
            // damageableObject?.Damaged(_owner.Stat.Damage);
           
        }
    }
    public void ActiveCollider()
    {
        WeaponTrail.Clear();
        WeaponCollider.enabled = true;
        WeaponTrail.enabled = true;

    }
    public void InactiveCollider()
    {
        WeaponCollider.enabled = false;
        WeaponTrail.enabled = false;
    }
}