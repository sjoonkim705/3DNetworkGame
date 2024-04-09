using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearCharacterDetection : MonoBehaviour
{
    public Transform Target;
    public float raycastDistance = 20f;
    public float raycastAngle = 30f; // 부채꼴 모양의 각도

    private Transform _target;

    void Update()
    {
        DetectPlayerInCone();  
    }

    private void DetectPlayerInCone()
    {
        Vector3 forward = transform.forward;
        float halfAngle = raycastAngle / 2f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, transform.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, transform.up);

        // 왼쪽과 오른쪽 방향의 회전을 계산합니다.
        Vector3 leftDirection = leftRayRotation * forward;
        Vector3 rightDirection = rightRayRotation * forward;

        // Raycast를 쏘아 목표를 찾습니다.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, leftDirection, out hit, raycastDistance) ||
            Physics.Raycast(transform.position, forward, out hit, raycastDistance) ||
            Physics.Raycast(transform.position, rightDirection, out hit, raycastDistance))
        {
            Debug.DrawRay(transform.position, leftDirection * raycastDistance, Color.red);
            Debug.DrawRay(transform.position, forward * raycastDistance, Color.red);
            Debug.DrawRay(transform.position, rightDirection * raycastDistance, Color.red);
        }
        else
        {
            // Ray가 어떤 대상에도 맞지 않으면 _target을 null로 초기화합니다.
            _target = null;

            Debug.DrawRay(transform.position, leftDirection* raycastDistance, Color.clear);
            Debug.DrawRay(transform.position, forward* raycastDistance, Color.clear);
            Debug.DrawRay(transform.position, rightDirection* raycastDistance, Color.clear);

            
        }
    }

}
