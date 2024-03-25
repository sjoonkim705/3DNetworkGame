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
    private int JumpMaxCount = 2;
    private int JumpRemainCount;
    private bool _isFalling = false;

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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        _dir = new Vector3(h, 0, v);             // 로컬 좌표꼐 (나만의 동서남북) 
        float unNormalizedDir = _dir.magnitude;
        /*        if (unNormalizedDir >= 0.1f)
                {
                    _animator.SetBool("Move", true);

                }
                else
                {
                    _animator.SetBool("Move", false);
                }*/
        float speed = _owner.Stat.MoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // - Shift 누른 동안에는 스태미나가 서서히 소모된다. (3초)

            speed = _owner.Stat.RunSpeed;

        }
        
        _animator.SetFloat("MoveSpeed", speed* unNormalizedDir);

        _dir.Normalize();
        _dir = Camera.main.transform.TransformDirection(_dir); // 글로벌 좌표계 (세상의 동서남북)
        _yVelocity += _gravity * Time.deltaTime;

        // 2. 플레이어에게 y축에 있어 중력을 적용한다.
        _dir.y = _yVelocity;

        _characterController.Move(_dir * speed * Time.deltaTime);

        if (!_characterController.isGrounded && !_isJumping)
        {
            _isFalling = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _yVelocity = JumpPower;
            //_isJumping = true;
            // JumpRemainCount--;
        }
    }
}
