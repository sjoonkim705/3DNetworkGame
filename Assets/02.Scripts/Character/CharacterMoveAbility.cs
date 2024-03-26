using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]



public class CharacterMoveAbility : CharacterAbility
{
    //public float MoveSpeed = 5f;     // 일반 속도
    //public float RunSpeed = 10f;

    public float JumpPower = 3f;

    private bool _isJumping = false;
   // private int JumpMaxCount = 2;
   // private int JumpRemainCount;
    private bool _isFalling = false;

    public float StaminaConsumeFactor = 10f;
    public float StaminaRecoveryFactor = 5f;


    private float _currentSpeed;
    private float _gravity = -10;
    private float _yVelocity = 0f;
    private Vector3 _dir;

    private CharacterController _characterController;
    private Animator _animator;

    //private Character _owner;

    // 목표 : [W][A][S][D]및 방향키를 누르면 캐릭터를 그 방향으로 이동시키고 싶다.
    protected override void Awake()
    {
        base.Awake();

        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
 
    }

    private void Update()
    {
        // 순서
        // 1. 사용자의 키보드 입력을 받는다.
        // 2. 캐릭터가 바라보는 방향을 기준으로 방향을 설정한다.
        // 3. 이동 속도에 따라 그 방향으로 이동한다.
        if (!_owner.PhotonView.IsMine)
        {
            return;
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        _dir = new Vector3(h, 0, v);             // 로컬 좌표꼐 (나만의 동서남북) 
        float unNormalizedDir = _dir.magnitude;


        
        _animator.SetFloat("MoveSpeed", _currentSpeed * unNormalizedDir);

        _dir.Normalize();
        _dir = Camera.main.transform.TransformDirection(_dir); // 글로벌 좌표계 (세상의 동서남북)
        _yVelocity += _gravity * Time.deltaTime;

        // 2. 플레이어에게 y축에 있어 중력을 적용한다.
        _dir.y = _yVelocity;

        _characterController.Move(_dir * _currentSpeed * Time.deltaTime);

        if (!_characterController.isGrounded && !_isJumping)
        {
            _isFalling = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetTrigger("Jump");
            _yVelocity = JumpPower;
            //_isJumping = true;
            // JumpRemainCount--;
        }
        if (!_characterController.isGrounded && !_isJumping)
        {
            _isFalling = true;
        }
    }
    private void FixedUpdate()
    {
        _currentSpeed = _owner.Stat.MoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && _owner.Stat.Stamina > 0)
        {
            _currentSpeed = _owner.Stat.RunSpeed;
            _owner.Stat.Stamina -= Time.fixedDeltaTime * StaminaConsumeFactor;
        }
        else
        {
            _owner.Stat.Stamina += Time.fixedDeltaTime * StaminaRecoveryFactor;
        }

    }
}
