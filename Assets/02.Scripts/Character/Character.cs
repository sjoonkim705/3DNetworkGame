using Cinemachine;
using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;
public enum CharacterStatus
{
    Playing,
    Dead,
}

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterCanvasAbility))]
[RequireComponent(typeof(CharacterShakeAbility))]
[RequireComponent(typeof(Animator))]


public class Character : MonoBehaviour, IPunObservable, IDamaged
{
    public Stat Stat;
    public bool IsDead;
    public PhotonView PhotonView { get; private set; }

    private Vector3 _recievedPosition;
    private Quaternion _recievedRotation;
    private Weapon _weapon;
    private CinemachineImpulseSource _impulseSource;
    private GameObject _damageScreen;
    private CharacterShakeAbility _modelMovement;
    private Animator _animator;
    private CharacterController _controller;


    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _damageScreen = UI_CharacterStat.Instance.DamageScreen.gameObject;
        _modelMovement = GetComponent<CharacterShakeAbility>();
        _animator = GetComponent<Animator>();
    }

    // 데이터 동기화를 위해 데이터 전송 및 수신 기능을 가진 약속

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // stream(통로)은 서버에서 주고받을 데이터가 담겨있는 변수
        if (stream.IsWriting) // 전송하는 상황
        {
            /*            stream.SendNext(transform.position);
                        stream.SendNext(transform.rotation);*/
            stream.SendNext(Stat.Health);
            stream.SendNext(Stat.Stamina);

        }
        else if (stream.IsReading) // 수신
        {

            /*            _recievedPosition = (Vector3)stream.ReceiveNext();
                        _recievedRotation = (Quaternion)stream.ReceiveNext();*/

            if (!PhotonView.IsMine)
            {
                Stat.Health = (int)stream.ReceiveNext();
                Stat.Stamina = (float)stream.ReceiveNext();
            }
        }
        // info는 송수신 성공/실패 여부에 대한 메시지가 담겨있다.
    }
    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
        if (PhotonView.IsMine)
        {
            UI_CharacterStat.Instance.MyCharacter = this;
            Stat.OnHealthChanged += UI_CharacterStat.Instance.RefreshHealthUI;
            Stat.OnStaminaChanged += UI_CharacterStat.Instance.RefreshStaminaUI;
            Stat.Init();
        }
        _weapon = GetComponentInChildren<Weapon>();

    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            //  ShowDamageEffectUI();
            Damaged(500, Vector3.zero);
        }

    }
    [PunRPC]
    public void Damaged(int amount, Vector3 attackOrigin)
    {
        Stat.Health -= amount;
        if (PhotonView.IsMine)
        {

            PhotonNetwork.Instantiate("DamageEffect", _weapon.transform.position, Quaternion.identity);
            ShowDamageEffectUI();
            StartCoroutine(_modelMovement.ShakeCharacter_Coroutine());
        }
        if (Stat.Health <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        IsDead = true;
        _animator.SetBool("Die", true);
        GetComponent<CharacterAttackAbility>().enabled = false;
        GetComponent<CharacterMoveAbility>().enabled = false;
        GetComponent<CharacterRotateAbility>().enabled = false;
        StartCoroutine(Respawn_Coroutine(5f));
    }

    [PunRPC]
    private IEnumerator Respawn_Coroutine(float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);
        IsDead = false;
        _animator.SetBool("Die", false);
        Stat.Init();
        GetComponent<CharacterAttackAbility>().enabled = true;
        GetComponent<CharacterMoveAbility>().enabled = true;
        GetComponent<CharacterRotateAbility>().enabled = true;
        TeleportCharacter(CharacterRespawnSpot.Instance.GetRandomRespawnSpot().position);
    }

    [PunRPC]
    public void TeleportCharacter(Vector3 respawnSpot)
    {
        _controller.enabled = false;
        transform.position = (respawnSpot);
        _controller.enabled = true;
        Debug.Log(transform.position);
        Debug.Log(respawnSpot);
    }
    public void ShowDamageEffectUI()
    {
        float strength = 0.2f;
        _impulseSource.GenerateImpulseWithVelocity(Random.insideUnitSphere.normalized * strength);
        UI_DamagedEffect.Instance.Show(0.4f);
    }
}



