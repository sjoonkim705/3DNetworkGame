using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 아이템 공장의 역할: 아이템 오브젝트의 생성을 책임진다.
// 팩토리 패턴
// 객체 생성을 공장 클래스를 이용해 캡슐화 처리하여 대신 "생성"하게 하는 대신
// 객체 생성에 필요한 과정을 템플릿화 해놓고 외부에서 쉽게 사용한다.
// 장점:
// 1. 생성과 처리 로직을 분리하여 결합도를 낮출 수 있다.
// 2. 확장 및 유지보수가 편리하다.
// 3. 객체 생성 후 공통으로 할 일을 수행하도록 지정해 줄 수 있다.

// 단점:
// 1. 상대적으로 조금 복잡
// 2. 학습 필요

public class ItemObjectFactory : MonoBehaviourPun
{
    public static ItemObjectFactory Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void RequestCreate(ItemType type, Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Create(type, position);
        }
        else
        {
            photonView.RPC(nameof(Create), RpcTarget.MasterClient, type, position);
        }
    }

    [PunRPC]
    private void Create(ItemType type, Vector3 position)
    {
        Vector3 dropPos = position + new Vector3(0, 0.5f, 0f) + UnityEngine.Random.insideUnitSphere;
        PhotonNetwork.InstantiateRoomObject(type.ToString(), dropPos, Quaternion.identity);
    }

    public void RequestDelete(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Delete(viewID);
        }
        else
        {
            photonView.RPC(nameof(Delete), RpcTarget.MasterClient, viewID);
        }
    }

    [PunRPC]
    private void Delete(int viewID)
    {
        GameObject objectToDelete = PhotonView.Find(viewID).gameObject;
        if (objectToDelete != null)
        {
            PhotonNetwork.Destroy(objectToDelete);
        }
    }
}