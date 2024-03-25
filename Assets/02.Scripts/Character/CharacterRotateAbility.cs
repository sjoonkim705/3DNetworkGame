using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CharacterRotateAbility : CharacterAbility
{
    //public float RotationSpeed = 300; // 초당 200도까지 회전 가능한 속도
    // 누적할 x각도
    private float _mx = 0;
    private float _my = 0;
    private float _mouseX;
    private float _mouseY;
    public Transform CameraRoot;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");


        Vector3 rotationDir = new Vector3(_mouseX, _mouseY, 0);
        _mx += rotationDir.x * _owner.Stat.RotationSpeed * Time.deltaTime;
        _my += rotationDir.y * _owner.Stat.RotationSpeed * Time.deltaTime;

        _my = Mathf.Clamp(_my, -45f, 15f);
        transform.eulerAngles = new Vector3(0f, _mx, 0);
        //Debug.Log(_mx);
        CameraRoot.localEulerAngles = new Vector3(-_my, 0, 0);

    }

}
