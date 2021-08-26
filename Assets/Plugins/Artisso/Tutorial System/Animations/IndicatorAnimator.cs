using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorAnimator : MonoBehaviour
{
    Animator anim;
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetBool("Displayed", isActiveAndEnabled);
    }
}
