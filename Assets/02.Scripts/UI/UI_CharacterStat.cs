using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class UI_CharacterStat : MonoBehaviour
{
    public static UI_CharacterStat Instance { get;private set; }

    public Character MyCharacter;

    public Slider HealthSlider;
    public Slider StaminaSlider;
    public Image DamageScreen;

    private void Awake()
    {
        Instance = this;
    }

    public void RefreshHealthUI()
    {
        if (MyCharacter != null)
        {
                HealthSlider.value = (float)MyCharacter.Stat.Health / MyCharacter.Stat.MaxHealth;
        }
    }
    public void RefreshStaminaUI()
    {
        if (MyCharacter != null)
        {
                StaminaSlider.value = MyCharacter.Stat.Stamina / MyCharacter.Stat.MaxStamina;
        }
    }
}
