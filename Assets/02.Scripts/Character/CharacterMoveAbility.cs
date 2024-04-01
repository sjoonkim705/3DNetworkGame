using JetBrains.Annotations;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]



public class CharacterMoveAbility : CharacterAbility
{
    //public float MoveSpeed = 5f;     // 일반 속도
    //public float RunSpeed = 10f;


    public int JumpMaxCount = 2;
    private int _jumpRemainCount;

    private bool _isJumping = false;
    private bool _isFalling = false;
    private bool _isSprintMode = false;
    private bool _isMoving = false;


    public float StaminaConsumeFactor = 10f;
    public float StaminaRecoveryFactor = 5f;

    private float _currentSpeed;
    private float _gravity = -30f;
    private float _yVelocity = 0f;
    private Vector3 _dir;

    private CharacterController _characterController;
    private Animator _animator;


    // 목표 : [W][A][S][D]및 방향키를 누르면 캐릭터를 그 방향으로 이동시키고 싶다.
    protected override void Awake()
    {
        base.Awake();

        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

    }
    private void Start()
    {
        _jumpRemainCount = JumpMaxCount;
    }

    private void Update()
    {
        if (!_owner.PhotonView.IsMine || _owner.State == State.Death)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        _dir = new Vector3(h, 0, v);             // 로컬 좌표꼐 (나만의 동서남북) 
        
        if (_dir.magnitude > 0)
        {
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }

        
        float unNormalizedDir = _dir.magnitude;

        _animator.SetFloat("MoveSpeed", _currentSpeed * unNormalizedDir);

        _dir.Normalize();
        _dir = Camera.main.transform.TransformDirection(_dir); // 글로벌 좌표계 (세상의 동서남북)
        if(!_characterController.isGrounded)
        {
            _yVelocity += _gravity * Time.deltaTime;
        }

        // 2. 플레이어에게 y축에 있어 중력을 적용한다.
        _dir.y = _yVelocity;
        _dir.x *= _currentSpeed;
        _dir.z *= _currentSpeed;

        _characterController.Move(_dir * Time.deltaTime);

        if (!_characterController.isGrounded && !_isJumping)
        {
            _isFalling = true;
        }
        else if (_characterController.isGrounded)
        {
            _jumpRemainCount = JumpMaxCount;
            _isJumping = false;
        }



        if (Input.GetKey(KeyCode.LeftShift) && _owner.Stat.Stamina > 0)
        {
            if (_isMoving)
            {
                _owner.Stat.Stamina -= Time.deltaTime * StaminaConsumeFactor;
                _currentSpeed = _owner.Stat.RunSpeed;
            }
        }
        else
        {
            _currentSpeed = _owner.Stat.MoveSpeed;
            _owner.Stat.Stamina += Time.deltaTime * StaminaRecoveryFactor;
        }
        bool haveJumpStamina = _owner.Stat.Stamina >= _owner.Stat.JumpConsumeStamina;

        if (Input.GetKeyDown(KeyCode.Space) && _jumpRemainCount > 0 && haveJumpStamina)
        {
            _isJumping = true;
            _jumpRemainCount--;
            if (_jumpRemainCount > 0)
            {
                _owner.Stat.Stamina -= _owner.Stat.JumpConsumeStamina;
                _animator.SetTrigger("Jump");
            }
            else
            {
                _animator.SetTrigger("DoubleJump");
            }
            _yVelocity = _owner.Stat.JumpPower;
        }

    }
    public bool IsJumping()
    {
        return _isJumping;
    }
}


