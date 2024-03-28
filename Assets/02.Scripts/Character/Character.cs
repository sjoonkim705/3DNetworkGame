using Cinemachine;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterCanvasAbility))]
[RequireComponent(typeof(CharacterShakeAbility))]

public class Character : MonoBehaviour, IPunObservable, IDamaged
{
    public Stat Stat;
    public PhotonView PhotonView { get; private set; }

    private Vector3 _recievedPosition;
    private Quaternion _recievedRotation;
    private Weapon _weapon;
    private CinemachineImpulseSource _impulseSource;
    private GameObject _damageScreen;
    private CharacterShakeAbility _modelMovement;


    private void Start()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _damageScreen = UI_CharacterStat.Instance.DamageScreen.gameObject;
        _modelMovement = GetComponent<CharacterShakeAbility>();
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
        /*        if (!PhotonView.IsMine)
                {
                    transform.position = Vector3.Lerp(transform.position, _recievedPosition, Time.deltaTime * 20f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, _recievedRotation, Time.deltaTime * 20f);
                }*/
        if (Input.GetKeyDown(KeyCode.P))
        {
            //  ShowDamageEffectUI();
            _modelMovement.Shake();

        }
    }
    [PunRPC]
    public void Damaged(int amount, Vector3 attackOrigin)
    {
        if (PhotonView.IsMine)
        {
            Stat.Health -= amount;
            PhotonNetwork.Instantiate("DamageEffect", _weapon.transform.position, Quaternion.identity);
            ShowDamageEffectUI();
            StartCoroutine(_modelMovement.ShakeCharacter_Coroutine());
        }
    }
    public void ShowDamageEffectUI()
    {
        float strength = 0.2f;
        _impulseSource.GenerateImpulseWithVelocity(Random.insideUnitSphere.normalized * strength);
        UI_DamagedEffect.Instance.Show(0.4f);
    }
}



