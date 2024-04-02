using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviourPun
{
    public List<ItemObject> GeneratedObjects;
    public int GenerateSize = 10;

    private void Awake()
    {
        GeneratedObjects = new List<ItemObject>();
    }
    private void Start()
    {
    }
    private void InitPool()
    {

    }
}
