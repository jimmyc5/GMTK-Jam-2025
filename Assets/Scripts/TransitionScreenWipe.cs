using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScreenWipe : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spr;

    public void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("WipeIn");
    }

    public void WipeOut()
    {
        anim.SetTrigger("WipeOut");
    }
}
