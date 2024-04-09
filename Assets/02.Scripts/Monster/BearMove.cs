using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;
using static UnityEngine.UI.GridLayoutGroup;
using System.Collections.Generic;
using JetBrains.Annotations;

public enum BearState
{
    Idle,
    Patrol,
    Trace,
    Attack,
    Comeback,
    IsHit,
    Die
}

public class BearMove : MonoBehaviour, IDamaged
{
    public Stat stat;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private BearState _currentState = BearState.Idle;
    private List<Character> _characterList = new List<Character>();
    public SphereCollider CharacterDetectCollider;


    [Header("Health")]
    public Slider HealthSliderUI;
    private Canvas _UI_Canvas;


    [Header("Movement")]
    public float TraceSpeed = 4f;
    public float PatrolSpeed = 2f;
    public Vector3 StartPosition;

    [Header("Attack")]
    public const float AttackDelay = 2.5f;
    private float _attackTimer = 0f;


    private Transform _target;

    [Header("AI")]
    public float TraceDetactRange = 10f;
    public float AttackDistance = 7f;
    public float MoveDistance = 40f;
    public const float TOLERANCE = 0.1f;       
    public float IdleDuration = 10f;
    private float _idleTimer;

    [Header("Patrol")]
    private Vector3 _destination;
    public float MovementRange = 30f;

    [Header("Knockback")]
    private Vector3 _knockbackStartPosition;
    private Vector3 _knockbackEndPosition;
    private const float KNOCKBACK_DURATION = 0.2f;
    private float _knockbackProgress = 0f;
    public float KnockbackPower = 1.2f;

    private void Awake()
    {
        CharacterDetectCollider.GetComponent<SphereCollider>();
    }
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();


        _destination = _navMeshAgent.transform.position;
        CharacterDetectCollider.radius = TraceDetactRange;

        StartPosition = transform.position;
        Init();
        _UI_Canvas = GetComponentInChildren<Canvas>();
    }
    public void Init()
    {
        _idleTimer = 0f;
        stat.Health = stat.MaxHealth;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Character character = other.GetComponent<Character>();
            if (!_characterList.Contains(character))
            {
                Debug.Log("새로운 인간을 찾았다");
                _characterList.Add(character);
            }
        }

    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // _UI_Canvas.transform.forward = Camera.main.transform.forward;

            HealthSliderUI.value = (float)stat.Health / (float)stat.MaxHealth;
            if (_currentState == BearState.Trace || _currentState == BearState.Attack)
            {
                LookAtPlayerSmoothly();
            }
            switch (_currentState)
            {
                case BearState.Idle:
                    Idle();
                    break;

                case BearState.Trace:
                    Trace();
                    break;

                case BearState.Patrol:
                    Patrol();
                    break;

                case BearState.Comeback:
                    Comeback();
                    break;

                case BearState.Attack:
                    Attack();
                    break;

                case BearState.IsHit:
                    IsHit();
                    break;

                case BearState.Die:
                    Die();
                    break;
            }
        }
    }

    private void Idle()
    {
        _idleTimer += Time.deltaTime;
        if ((_idleTimer >= IdleDuration * 0.1f && _idleTimer <= IdleDuration * 0.9f) || Vector3.Distance(_target.position, transform.position) <= TraceDetactRange / 4)
        {
            _animator.SetBool("Idle_Sitdown", true);
        }
        else
        {
            _animator.SetBool("Idle_Sitdown", false);
        }

        if (_idleTimer >= IdleDuration && _target == null)
        {
            _idleTimer = 0f;
            _animator.SetTrigger("IdleToPatrol");
            _currentState = BearState.Patrol;
        }

        if (_target != null)
        {
            if (Vector3.Distance(_target.position, transform.position) <= TraceDetactRange / 2)
            {
                _animator.SetTrigger("IdleToTrace");
                _currentState = BearState.Trace;
                _idleTimer = 0;
            }
        }
        else
        {
          //.//  _target = FindTarget(TraceDetactRange).transform;
        }
    }

    private void Trace()
    {
        if (_target == null)
        {
            _currentState = BearState.Comeback;
            return;
        }

        _navMeshAgent.speed = stat.RunSpeed;
        _navMeshAgent.destination = _target.position;

        // 플레이어와의 거리가 공격 범위 내에 있는지 확인
        if (Vector3.Distance(_target.position, transform.position) <= AttackDistance)
        {
            if (_currentState != BearState.Attack)   // 현재 상태가 Attack이 아닐 때만 전환
            {
                _animator.SetTrigger("TraceToAttack");
                _currentState = BearState.Attack;
            }
        }
        else if (Vector3.Distance(_target.position, transform.position) >= TraceDetactRange)
        {
            // 플레이어와의 거리가 찾기 범위를 벗어나면 Comeback 상태로 전환
            _animator.SetTrigger("TraceToComeback");
            _currentState = BearState.Comeback;
        }
    }

    private void Patrol()
    {
        _navMeshAgent.speed = stat.MoveSpeed;
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= TOLERANCE)
        {
            MoveToRandomPosition();
        }

        // 플레이어가 감지 범위 내에 있으면 상태를 Trace로 변경하여 플레이어를 추적
        if (Vector3.Distance(_target.position, transform.position) <= TraceDetactRange)
        {
            _animator.SetTrigger("PatrolToTrace");
            Debug.Log("PatrolToTrace");
            _currentState = BearState.Trace;
        }

        // 추가: Patrol 상태에서 일정 시간 대기 후 Comeback으로 전환
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= TOLERANCE)
        {
            StartCoroutine(WaitAndComeback());
        }
    }

    private IEnumerator WaitAndComeback()
    {
        yield return new WaitForSeconds(2f);  // 대기 시간 조절 필요
        _animator.SetTrigger("PatrolToComeback");
        Debug.Log("PatrolToComeback");
        _currentState = BearState.Comeback;
    }

    private void MoveToRandomPosition()
    {
        // 일정 범위 내에서 랜덤한 위치로 이동
        Vector3 randomDirection = Random.insideUnitSphere * MovementRange;
        randomDirection += _navMeshAgent.destination;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, MovementRange, NavMesh.AllAreas);
        Vector3 targetPosition = hit.position;
        _navMeshAgent.SetDestination(targetPosition);
        _destination = targetPosition;
    }

    private void Comeback()
    {
        Vector3 dir = StartPosition - this.transform.position;
        dir.y = 0;
        dir.Normalize();
        _target = null;

        _navMeshAgent.stoppingDistance = TOLERANCE;
        _navMeshAgent.destination = StartPosition;

        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= TOLERANCE)
        {
            _animator.SetTrigger("ComebackToIdle");
            Debug.Log("CombackToIdle");
            _currentState = BearState.Idle;
        }
    }

    private void Attack()
    {
        float distanceToTarget = Vector3.Distance(_target.position, transform.position);
        _attackTimer += Time.deltaTime;
        _navMeshAgent.isStopped = true;

        if (_attackTimer >= AttackDelay)
        {

            if (distanceToTarget <= AttackDistance)
            {
                _animator.SetTrigger("TraceToAttack");
                Debug.Log("TraceToAttack");
            }

            _attackTimer = 0f; 
        }
        if (distanceToTarget > AttackDistance || distanceToTarget > TraceDetactRange)
        {
            _attackTimer = 0f;
            _animator.SetTrigger("AttackToTrace");
            Debug.Log("AttackToTrace");
            _currentState = BearState.Trace;
        }
    }
    
    private void IsHit()
    {
        // 넉백
        if (_knockbackProgress == 0)
        {
            _navMeshAgent.isStopped = true;
            _knockbackStartPosition = transform.position;

            Vector3 dir = transform.position - _target.position;
            dir.y = 0;
            dir.Normalize();

            _knockbackEndPosition = transform.position + dir * KnockbackPower;
        }
        _knockbackProgress += Time.deltaTime / KNOCKBACK_DURATION;
        transform.position = Vector3.Lerp(_knockbackStartPosition, _knockbackEndPosition, _knockbackProgress);
        _animator.SetTrigger("IsHit");
        if (_knockbackProgress > 1)
        {
            _navMeshAgent.isStopped = false;
            _knockbackProgress = 0f;
            _currentState = BearState.Trace;
        }
    }

    [PunRPC]
    public void Damaged(int amount, int actorNumber)
    {
        _animator.SetTrigger("IsHit");
        _currentState = BearState.IsHit;
        stat.Health -= amount;
        if (stat.Health <= 0)
        {
            _currentState = BearState.Die;
            Die();
        }
    }

    private Coroutine _dieCoroutine;
    private void Die()
    {
        _animator.SetTrigger("Die");
        _navMeshAgent.isStopped = true;
        if (_dieCoroutine == null)
        {
            _dieCoroutine = StartCoroutine(Die_Coroutine());
        }
   
    }
    private IEnumerator Die_Coroutine()
    {
        yield return new WaitForSeconds(5f);
        _navMeshAgent.ResetPath();

        HealthSliderUI.gameObject.SetActive(false);
        Destroy(gameObject);
       // ItemObjectFactory.Instance.MakePercent(transform.position);

    }

    private List<Character> FindTarget(float distance)
    {
        _characterList.RemoveAll(c => c == null);
        List<Character> characters = new List<Character>();

        Vector3 myPosition = transform.position;
        foreach (Character character in _characterList)
        {
            if (Vector3.Distance(character.transform.position, myPosition) <= distance)
            {
                characters.Add(character);

            }
        }

        return characters;
    }
    public void PlayerAttack()
    {
        IDamaged playerHit = _target.GetComponent<IDamaged>();
        if (playerHit != null)
        {
            Debug.Log("Player Damaged");
            playerHit.Damaged(stat.Damage, -2);
        }
    }
    void LookAtPlayerSmoothly()
    {
        Vector3 directionToTarget = _target.position - transform.position;
        directionToTarget.y = 0; //수평 회전만
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5); // 회전 속도 조절
    }
    void ActiveMovement()
    {
        _navMeshAgent.isStopped = false;
    }
    void InactiveMovement()
    {
        _navMeshAgent.isStopped = true;
    }
}
