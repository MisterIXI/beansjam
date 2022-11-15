using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    Animator _animator;
    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    private void OnEnable() {
        
        if(_animator.name =="LoseImage (1)") 
        {
            _animator.Play("LoseBlendInAnimation");
        }
        else
        {
            _animator.Play("BlendInAnimation");
        }
    }
}
