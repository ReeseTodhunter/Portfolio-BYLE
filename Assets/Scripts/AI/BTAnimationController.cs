using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAnimationController : MonoBehaviour
{
    // ABANDONDED CODE // TOM
    Animator animator;
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartAnimation("Slam");
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            StartAnimation("Strike");
        }

    }
    public void StartAnimation(string _animName)
    {
        animator.Play(_animName);
    }
}
