using Cinemachine;
using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.SocialPlatforms.Impl;

[RequireComponent(typeof(CharacterMoveAbility))]
[RequireComponent(typeof(CharacterRotateAbility))]
[RequireComponent(typeof(CharacterAttackAbility))]
[RequireComponent(typeof(CharacterCanvasAbility))]
[RequireComponent(typeof(CharacterShakeAbility))]
[RequireComponent(typeof(Animator))]


public class Character : MonoBehaviour, IPunObservable, IDamaged
{
    public Stat Stat;


    public State State { get; private set; } = State.Alive;
    public PhotonView PhotonView { get; private set; }
    public float RespawnTime = 5f;

    // private Vector3 _recievedPosition;
    // private Quaternion _recievedRotation;
    private Weapon _weapon;
    private CinemachineImpulseSource _impulseSource;
    private GameObject _damageScreen;
    private CharacterShakeAbility _modelMovement;
    private Animator _animator;
    private CharacterController _controller;
    private int _score = 0;
    private int _diedPlayerScore = 0;


    public int Score
    {
        get
        {
            return _score;

        }
        set
        {
            _score = value;
            //UI_Score.Instance.RefreshScoreUI();
            if (value < 0 )
            {
                _score = 0;

            }
           
        }
    }



    // 데이터 동기화를 위해 데이터 전송 및 수신 기능을 가진 약속
    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();

    }


    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _damageScreen = UI_CharacterStat.Instance.DamageScreen.gameObject;
        _modelMovement = GetComponent<CharacterShakeAbility>();
        _animator = GetComponent<Animator>();
        Score = 0;

        if (PhotonView.IsMine)    // let all UI classes link to this Character class
        {
            UI_CharacterStat.Instance.MyCharacter = this;
            UI_Score.Instance.Character = this;

            Stat.OnHealthChanged += UI_CharacterStat.Instance.RefreshHealthUI;
            Stat.OnStaminaChanged += UI_CharacterStat.Instance.RefreshStaminaUI;
            Stat.Init();
        }
        _weapon = GetComponentInChildren<Weapon>();

        if (!PhotonView.IsMine)
        {
            return;
        }
        SetRandomPointAndRotation();

        // 캐릭터.CustomProperty = Hashtable<string, object>

        Hashtable hashtable = new Hashtable();
        hashtable.Add("Score", 0);
        Score = (int)hashtable["Score"];
        hashtable.Add("KillCount", 0);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

    }
    [PunRPC]
    public void AddPropertyIntValue(string key, int value)
    {
        Hashtable myHashTable = PhotonNetwork.LocalPlayer.CustomProperties;
        myHashTable[key] = (int)myHashTable[key] + value;
        if (key == "Score")
        {
            Score = (int)myHashTable["Score"];
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(myHashTable);
    }
    public void SetPropertyIntValue(string key, int value)
    {
        Hashtable myHashTable = PhotonNetwork.LocalPlayer.CustomProperties;
        myHashTable[key] = value;
        if (key == "Score")
        {
            Score = (int)myHashTable["Score"];
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(myHashTable);
    }
    public int GetPropertyIntValue(string key)
    {
        Hashtable myHashTable = PhotonNetwork.LocalPlayer.CustomProperties;
        return (int)myHashTable[key];
    }



    [PunRPC]
    public void AddLog(string logMessage)
    {
        UI_Roominfo.instance.AddLog(logMessage);
    }

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

    [PunRPC]
    public void Damaged(int amount, int actorNumber)
    {
        if (State == State.Death)
        {
            return;
        }
    
        Stat.Health -= amount;
        StartCoroutine(_modelMovement.ShakeCharacter_Coroutine());
        if (PhotonView.IsMine)
        {
            if (actorNumber > 0)
            {
                PhotonNetwork.Instantiate("DamageEffect", _weapon.transform.position, Quaternion.identity);
            }
            ShowDamageEffectUI();
        }
        if (Stat.Health <= 0)
        {
           // State = State.Death;
            if (PhotonView.IsMine)
            {
                OnDeath(actorNumber);
            }
            PhotonView.RPC(nameof(Die), RpcTarget.All);
        }
    }

    [PunRPC]
    public void Die()
    {
        if (State == State.Death)
        {
            return;
        }

        State = State.Death;
        _animator.SetTrigger("Death");
        GetComponent<CharacterAttackAbility>().InactiveCollider();
        if (PhotonView.IsMine)
        {
            _controller.enabled = false;
            StartCoroutine(Respawn_Coroutine(RespawnTime));
            SetPropertyIntValue("Score", 0);
        }
    }

    private void OnDeath(int actorNumber)
    {
        if (actorNumber >= 0)
        {
            string nickname = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).NickName;
            Hashtable opponentHashTable = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).CustomProperties;

            Hashtable myHashtable = PhotonNetwork.LocalPlayer.CustomProperties;
            _diedPlayerScore = (int)myHashtable["Score"];

            opponentHashTable["Score"] = (int)opponentHashTable["Score"] + _diedPlayerScore / 2;
            opponentHashTable["KillCount"] = (int)opponentHashTable["KillCount"] + 1;
            PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).SetCustomProperties(opponentHashTable);

            string logMessage = $"{nickname}님이 {PhotonView.Owner.NickName}을 처치하였습니다.\n";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
        }
        else
        {
            string logMessage = $"{PhotonView.Owner.NickName}이 운명을 다했습니다.\n";
            PhotonView.RPC(nameof(AddLog), RpcTarget.All, logMessage);
        }
    }

    private IEnumerator Respawn_Coroutine(float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime / 2f);
        ItemObjectFactory.Instance.RequestCreatebyRandomProbablity(transform.position);
        for (int i = 0; i < _diedPlayerScore / 20 ; i++)
        {
            ItemObjectFactory.Instance.RequestCreate(ItemType.ScoreGem10, transform.position);
        }
        _diedPlayerScore = 0;
        yield return new WaitForSeconds(respawnTime / 2f);
        SetRandomPointAndRotation();

        PhotonView.RPC(nameof(Resurrect), RpcTarget.All);
        _controller.enabled = true;
    }
    [PunRPC]
    private void Resurrect()
    {
        State = State.Alive;
        Stat.Init();
        _animator.SetTrigger("Resurrection");
    }

    private void SetRandomPointAndRotation()
    {
        BattleScene.Instance.TeleportCharacter(gameObject, BattleScene.Instance.GetRandomRespawnSpot().position);

        Vector3 randomRotation = new Vector3(0, Random.Range(0, 360), 0);
        GetComponent<CharacterRotateAbility>().SetRotation(randomRotation);
    }
    public void ShowDamageEffectUI()
    {
        float strength = 0.2f;
        _impulseSource.GenerateImpulseWithVelocity(Random.insideUnitSphere.normalized * strength);
        UI_DamagedEffect.Instance.Show(0.4f);
    }
}



