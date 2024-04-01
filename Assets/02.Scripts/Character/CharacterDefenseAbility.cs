using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class CharacterDefenseAbility : MonoBehaviour
{
    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            _animator.SetBool("Guard", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            _animator.SetBool("Guard", false);
        }    
    }
}
