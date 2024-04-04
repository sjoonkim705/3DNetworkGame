using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviourPun
{
    private float _currentTime;
    private float _createTime;
    public float MinCreateTime = 10;
    public float MaxCreateTime = 50;

    private int _createCount;
    public int MinCreateCount = 10;
    public int MaxCreateCount = 20;

    private List<ItemObject> _itemList = new List<ItemObject>();


    private void Start()
    {
        _createTime = Random.Range(MinCreateTime, MaxCreateTime);
 
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        _currentTime += Time.deltaTime;

        if (_currentTime >= _createTime)
        {
            _itemList.RemoveAll(i => i == null || i.isActiveAndEnabled == false);

            if (_itemList.Count > MaxCreateCount)
            {
                return;
            }
            _createCount = Random.Range(MinCreateCount, MaxCreateCount);
            Vector3 randomPosition = transform.position + new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
            int randomGemFactor = Random.Range(0, 100);

            ItemObject itemObject = null;

            if (randomGemFactor <= 85)
            {
                itemObject = ItemObjectFactory.Instance.MasterCreate(ItemType.ScoreGem10, randomPosition);
            }
            else if (randomGemFactor <= 95)
            {

                itemObject = ItemObjectFactory.Instance.MasterCreate(ItemType.ScoreGem30, randomPosition);
            }
            else
            {

                itemObject = ItemObjectFactory.Instance.MasterCreate(ItemType.ScoreGem50, randomPosition);
            }

            if(itemObject != null)
            {
                _itemList.Add(itemObject);
                itemObject.transform.SetParent(transform);
            }

            _currentTime = 0f;

        }
        _createTime = Random.Range(MinCreateTime, MaxCreateTime);

    }

}
