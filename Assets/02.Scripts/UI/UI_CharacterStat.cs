using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UI_CharacterStat : MonoBehaviour
{
    public static UI_CharacterStat Instance { get;private set; }

    public Character MyCharacter;
    public Slider HealthSlider;
    public Slider StaminaSlider;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

        HealthSlider = transform.GetChild(0).GetComponent<Slider>();
        StaminaSlider = transform.GetChild(1).GetComponent<Slider>();

    }
    public void RefreshHealthUI()
    {
        HealthSlider.value = MyCharacter.Stat.Health / MyCharacter.Stat.MaxHealth;
    }
    public void RefreshStaminaUI()
    {
        StaminaSlider.value = MyCharacter.Stat.Stamina / MyCharacter.Stat.MaxStamina;
    }
}
