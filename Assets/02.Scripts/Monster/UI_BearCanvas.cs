using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BearCanvas : MonoBehaviour
{
    public Bear Owner;
    private Canvas _bearCanvas;
    public Slider HpSlider;

    // Start is called before the first frame update
    void Start()
    {
        Owner = GetComponent<Bear>();
        _bearCanvas = GetComponentInChildren<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        _bearCanvas.transform.forward = Camera.main.transform.forward;

    }
    public void RefreshHealthUI()
    {
        HpSlider.value = (float)Owner.Stat.Health / Owner.Stat.MaxHealth;

    }
}
