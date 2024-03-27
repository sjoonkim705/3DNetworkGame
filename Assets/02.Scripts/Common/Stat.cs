using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat 
{
    [Header("Health Stats")]
    public int MaxHealth;
    private int _health;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            OnHealthChanged?.Invoke();
            if (_health > MaxHealth)
            {
                _health = MaxHealth;
            }
            else if (_health < 0)
            {
                _health = 0;
            }
        }
    }
    public event Action OnHealthChanged;

    [Header("Stamina Stats")]
    public float MaxStamina;
    private float _stamina;
    public float Stamina
    {
        get
        {
            return _stamina;
        }
        set
        {
            _stamina = value;
            OnStaminaChanged?.Invoke();
            if (_stamina > MaxStamina)
            {
                _stamina = MaxStamina;
            }
            else if (_stamina < 0)
            {
                _stamina = 0;
            }
        }
    }
    public event Action OnStaminaChanged;

    [Header("MoveAbility Stats")]
    public float MoveSpeed;
    public float RunSpeed;

    [Header("Rotate Stats")]
    public float RotationSpeed;

    [Header("Attack Stats")]
    public int Damage;
    public float AttackCoolTime;

    public void Init()
    {
        Health = MaxHealth;
        Stamina = MaxStamina;

    }
}
