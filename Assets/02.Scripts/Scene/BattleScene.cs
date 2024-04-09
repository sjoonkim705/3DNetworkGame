using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleScene : MonoBehaviourPunCallbacks
{
    public static BattleScene Instance;
    private int _spotCount;
    public GameObject RandomSpots;
    private List<Transform> _respawnSpotList;
    private CharacterController _charController;
    private bool _init = false;


    public override void OnJoinedRoom()
    {
        if (!_init)
        {
            Init();
        }
    }
    public void Init()
    {
        _init = true;
        GameObject[] points = GameObject.FindGameObjectsWithTag("BearSpawnPoint");
        foreach (GameObject point in points)
        {
            PhotonNetwork.InstantiateRoomObject("Bear", point.transform.position, Quaternion.identity);
        }
        if (PhotonManager.instance.HasMaleCharacterSelected)
        {
            PhotonNetwork.Instantiate("MaleCharacter", Vector3.zero, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate("FemaleCharacter", Vector3.zero, Quaternion.identity);
        }
    }
    private void Awake()
    {
        Instance = this;
        _respawnSpotList = new List<Transform>();

        _spotCount = RandomSpots.transform.childCount;
        for (int i = 0; i < _spotCount; i++)
        {
            _respawnSpotList.Add(RandomSpots.transform.GetChild(i));
        }
    }
    private void Start()
    {
        if (!_init)
        {
            Init();
        }
    }
    public Transform GetRandomRespawnSpot()
    {
        Transform selectedSpot = null;
        int randomFactor = Random.Range(0, _spotCount);
        selectedSpot = _respawnSpotList[randomFactor];
        //Debug.Log($"{selectedSpot} / {_respawnSpotList.Count}" );
        return selectedSpot;
    }
    public void TeleportCharacter(GameObject objectToMove, Vector3 respawnSpot)
    {
        _charController = objectToMove.GetComponent<CharacterController>();
        _charController.enabled = false;
        objectToMove.transform.position = (respawnSpot);
        PhotonNetwork.Instantiate("RespawnEffect", respawnSpot, Quaternion.identity);
        _charController.enabled = true;

    }

}
