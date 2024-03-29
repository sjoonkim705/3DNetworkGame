using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDieZone : MonoBehaviour
{
    private Collider _dieZoneCollider;
    private void Awake()
    {
        _dieZoneCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("DieZone");
        other.GetComponent<Character>().Die();
    }
}
