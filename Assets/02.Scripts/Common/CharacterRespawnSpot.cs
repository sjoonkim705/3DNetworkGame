using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRespawnSpot : MonoBehaviour
{
    public static CharacterRespawnSpot Instance;
    private int _spotCount;
    private List<Transform> _respawnSpotList;

    private void Awake()
    {
        Instance = this;
        _respawnSpotList = new List<Transform>();

        _spotCount = transform.childCount;
        for (int i = 0; i < _spotCount; i++)
        {
            _respawnSpotList.Add(transform.GetChild(i));
        }
    }
    public Transform GetRandomRespawnSpot()
    {
        Transform selectedSpot = null;
        int randomFactor = Random.Range(0, _spotCount);
        selectedSpot = _respawnSpotList[randomFactor];
        Debug.Log($"{selectedSpot} / {_respawnSpotList.Count}" );
        return selectedSpot;
    }

}
