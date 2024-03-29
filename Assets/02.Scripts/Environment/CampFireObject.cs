using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFireObject : MonoBehaviour
{
    private float _elapsedTime = 0;
    public float DamageInterval = 1f;
    public bool IsColliding = false;
    public int Damage = 20;

    private IDamaged _damagedObject = null;

    private void Start()
    {
        _elapsedTime = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _damagedObject = other.GetComponent<IDamaged>();
        }
        PhotonView photonView = other.GetComponent<PhotonView>();
        if (photonView != null || !photonView.IsMine)
        {
            return;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > DamageInterval)
        {
            _elapsedTime = 0;
            _damagedObject?.Damaged(Damage, -1);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        _elapsedTime = 0;
    }
}
