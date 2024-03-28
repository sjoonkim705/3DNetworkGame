using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShakeAbility : CharacterAbility
{
    private Transform _characterModel;

    private void Start()
    {
        _characterModel = transform.GetChild(0);
    }

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCharacter_Coroutine());
    }
    public IEnumerator ShakeCharacter_Coroutine()
    {
        float elapsedTime = 0f;
        float shakeDuration = 0.15f;
        float shakeAmount = 0.1f;
        Vector3 _originalPos = _characterModel.transform.localPosition;
        Vector3 _originalRot = _characterModel.transform.localEulerAngles;


        while (elapsedTime < shakeDuration)
        {
            _characterModel.transform.localPosition = _originalPos + Random.insideUnitSphere.normalized * shakeAmount;

            Vector3 eulerRotation = _originalRot;

            eulerRotation.x += Random.Range(-shakeAmount, shakeAmount) * 0.2f;
            eulerRotation.y += Random.Range(-shakeAmount, shakeAmount) * 0.2f;
            eulerRotation.z += Random.Range(-shakeAmount, shakeAmount) * 0.2f;

            _characterModel.transform.localRotation = Quaternion.Euler(eulerRotation);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _characterModel.transform.localPosition = _originalPos;
        _characterModel.transform.localRotation = Quaternion.Euler(_originalRot);
    }
}
