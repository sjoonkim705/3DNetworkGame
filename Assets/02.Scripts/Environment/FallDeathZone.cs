using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDeathZone : MonoBehaviour
{
    private Collider _dieZoneCollider;
    private void Awake()
    {
        _dieZoneCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("DeathZone");
        other.GetComponent<Character>().Die();
    }
}
