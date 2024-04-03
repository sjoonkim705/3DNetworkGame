using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;


[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ItemObject : MonoBehaviourPun
{
    [Header("아이템 타입")]
    public ItemType ItemType;
    public float Value = 100;
    private Rigidbody Rigidbody;

    private Character _character;


    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
    public void ApplyRandomDirectionalForce()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);
        Vector3 randomDir = new Vector3(randomX, 0, randomZ);
        Vector3 forceDir = randomDir + Vector3.up;
        Rigidbody.AddForce(forceDir, ForceMode.Impulse);
    }
    private void Start()
    {
        if (photonView.IsMine)
        {
            ApplyRandomDirectionalForce();
        }
    }

        private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _character = other.GetComponent<Character>();
            if (!_character.PhotonView.IsMine || _character.State == State.Death || _character == null)
            {
                return;
            }

            switch (ItemType)
            {
                case ItemType.HealthPotion:
                {
                    _character.Stat.Health += (int)Value;
                    _character.GetComponent<CharacterEffectAbility>().RequestPlay((int)ItemType.HealthPotion);
                    if (_character.Stat.Health >= _character.Stat.MaxHealth)
                    {
                        _character.Stat.Health = _character.Stat.MaxHealth;
                    }
                    break;
                }

                case ItemType.StaminaPotion:
                {
                    _character.Stat.Stamina += Value; 
                    _character.GetComponent<CharacterEffectAbility>().RequestPlay((int)ItemType.StaminaPotion);

                    if (_character.Stat.Stamina > _character.Stat.MaxStamina)
                    {
                        _character.Stat.Stamina = _character.Stat.MaxStamina;
                    }
                    break;
                }
                case ItemType.ScoreGem:
                    _character.AddScore((int)Value);
                    _character.GetComponent<CharacterEffectAbility>().RequestPlay((int)ItemType.ScoreGem);

                    break;
            }

            gameObject.SetActive(false);
            ItemObjectFactory.Instance.RequestDelete(photonView.ViewID);
        }
    }

}

