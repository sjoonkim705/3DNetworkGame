using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform Target;
    public float Ydistance = 20;
    private Vector3 _initialEulerAngles;

    private void Start()
    {
        _initialEulerAngles = transform.eulerAngles;
    }
    void LateUpdate()
    {
        if (Target != null)
        {
            if (UI_CharacterStat.Instance.MyCharacter.PhotonView.IsMine)
            {
                Vector3 targetPosition = Target.position;

                transform.position = targetPosition;
                targetPosition.y = Ydistance;
                transform.position = targetPosition;


                Vector3 targetEulerAngles = Target.eulerAngles;
                targetEulerAngles.x = _initialEulerAngles.x;
                targetEulerAngles.z = _initialEulerAngles.z;
                transform.eulerAngles = targetEulerAngles;
            }
        
        }
    }

}

