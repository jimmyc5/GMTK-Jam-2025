using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSounds : MonoBehaviour
{
    public Transform playerBase;
    public AudioClip footstepSound;
    public PlayerMovement movement;

    private bool isGrounded = true;

    // Update is called once per frame
    void Update()
    {
        if (movement.isGrounded)
        {
            if(transform.position.y - playerBase.position.y > 0.3f)
            {
                isGrounded = false;
            }
            else
            {
                if (!isGrounded)
                {
                    SoundManager.instance.PlaySoundClip(footstepSound, transform.position, 0.5f, Random.Range(0.8f,1.2f));
                }
                isGrounded = true;
            }
        }
    }
}
