using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopController : MonoBehaviour
{
    private List<(Vector3 position, Vector3 velocity, Quaternion rotation)> loopList;
    private Rigidbody rb;

    private int loopTracker;
    private bool loopBackward;
    private bool objectIsLooping;

    private bool wasKinematic;

    public bool loopTestSwitch;

    public AudioSource LoopSFXSource;
    public AudioSource LoopForwardsSFXSource;
    private float startingVolume;
    public float volumeGain;

    public AudioClip loopStartSoundClip;

    // Start is called before the first frame update
    void Start()
    {
        loopList = new List<(Vector3 position, Vector3 velocity, Quaternion rotation)>();
        rb = GetComponent<Rigidbody>();
        if (LoopSFXSource)
        {
            startingVolume = LoopSFXSource.volume;
            LoopSFXSource.volume = 0f;
            LoopForwardsSFXSource.volume = 0f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(objectIsLooping)
        {
            // Break early for protection
            if (loopList.Count == 0)
                return;

            // Set Position and rotation
            (Vector3 position, Vector3 velocity, Quaternion rotation) loopItem = loopList[loopTracker];
            if (LoopSFXSource)
                LoopSFXSource.volume = Mathf.MoveTowards(LoopSFXSource.volume, loopBackward ? startingVolume : 0f, volumeGain / Time.deltaTime);
            if (LoopForwardsSFXSource)
                LoopForwardsSFXSource.volume = Mathf.MoveTowards(LoopForwardsSFXSource.volume, !loopBackward ? startingVolume : 0f, volumeGain / Time.deltaTime);
            rb.MovePosition(loopItem.position);
            rb.MoveRotation(loopItem.rotation);

            // Increment Loop direction
            if (loopBackward)
            {
                loopTracker--;
            }
            else
            {
                loopTracker++;
            }

            // Swap at boundaries
            if (loopTracker == -1 || loopTracker == loopList.Count)
            {
                loopBackward = !loopBackward;
                loopTracker = Mathf.Clamp(loopTracker, 0, loopList.Count - 1);
            }

        }
        else if(loopList.Count == 0 || Vector3.Distance(rb.position, loopList[loopList.Count - 1].position) > 0.01f || Quaternion.Angle(rb.rotation, loopList[loopList.Count - 1].rotation) > 1f)
        {
            // Add to loop List if object is moving at all and not looping
            loopList.Add((rb.position, rb.velocity, rb.rotation));
        }
        if (!objectIsLooping)
        {
            if (LoopSFXSource)
            {
                LoopSFXSource.volume = Mathf.MoveTowards(LoopSFXSource.volume, 0f, volumeGain / Time.deltaTime);
                LoopForwardsSFXSource.volume = Mathf.MoveTowards(LoopSFXSource.volume, 0f, volumeGain / Time.deltaTime);
            }

        }
        // For loop testing purposes
        if(loopTestSwitch)
        {
            if(objectIsLooping)
            {
                endLoop();
            }
            else
            {
                startLoop();
            }
            loopTestSwitch = false;
        }
    }

    // Loop will always start by going backwards...
    public void startLoop()
    {
        wasKinematic = rb.isKinematic;
        rb.isKinematic = true;
        GlobalLoopList.RBList.Add(rb);
        objectIsLooping = true;
        loopBackward = true;
        loopTracker = loopList.Count - 1;
        SoundManager.instance.PlaySoundClip(loopStartSoundClip, transform.position, 1f, 1.3f);
    }

    // Ends loop and removes all items from loop List above the current loop tracker
    public void endLoop()
    {
        objectIsLooping = false;
        rb.isKinematic = wasKinematic;
        rb.velocity = loopList[loopTracker].velocity;
        loopList.RemoveRange(loopTracker + 1, loopList.Count - loopTracker - 1);
        GlobalLoopList.RBList.Remove(rb);
        SoundManager.instance.PlaySoundClip(loopStartSoundClip, transform.position, 1f, 0.9f);
    }

    public bool ToggleLoop()
    {
        if (objectIsLooping)
        {
            endLoop();
        }
        else
        {
            startLoop();
        }
        return objectIsLooping;
    }
}
