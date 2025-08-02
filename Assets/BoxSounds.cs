using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSounds : MonoBehaviour
{
    public AudioClip collisionSound;

    private float lastSFXTime = 0f;
    private float SFXCooldown = 0.1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collisionSound)
        {
            float relativeVelocity = collision.impulse.magnitude;

            if ((collision.collider.gameObject.tag != "Player" && relativeVelocity > 0.1f) || relativeVelocity > 5f)
            {

                if (SoundManager.instance && Time.time > lastSFXTime + SFXCooldown)
                {
                    SoundManager.instance.PlaySoundClip(collisionSound, transform.position, Mathf.Min(collision.relativeVelocity.magnitude * 0.06f, 0.8f), 0.8f);
                    lastSFXTime = Time.time;
                }
            }
        }
    }
}
