using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvasAbility : CharacterAbility
{
    //protected Character _owner { get; private set; }
    public Canvas Canvas;
    public Text NicknameTextUI;
    public Slider HealthSlider;
    public Slider StaminaSlider;

    void Start()
    {
        NicknameTextUI.text = _owner.PhotonView.Controller.NickName;
        RefreshHealthUI();
        RefreshStaminaUI();
        _owner.Stat.OnHealthChanged += RefreshHealthUI;
        _owner.Stat.OnStaminaChanged += RefreshStaminaUI;
    }

    void Update()
    {
        // 빌보드 코드 구현
        if (!_owner.PhotonView.IsMine)
        {
            Canvas.transform.forward = Camera.main.transform.forward;
        }
    }
    public void RefreshHealthUI()
    {
        HealthSlider.value = (float)_owner.Stat.Health / _owner.Stat.MaxHealth;
    }
    public void RefreshStaminaUI()
    {
        StaminaSlider.value = _owner.Stat.Stamina / _owner.Stat.MaxStamina;
    }

}
