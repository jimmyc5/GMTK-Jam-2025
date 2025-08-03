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
    public float startingVolume;
    public float volumeGain;

    public AudioClip loopStartSoundClip;

    // Start is called before the first frame update
    void Awake()
    {
        loopList = new List<(Vector3 position, Vector3 velocity, Quaternion rotation)>();
        rb = GetComponent<Rigidbody>();
        if (LoopSFXSource)
        {
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
        else if(checkIfShouldLog(rb.position, rb.velocity,rb.rotation))
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

    // check if you should add to list (if same as last 30 entries, stop adding)
    private bool checkIfShouldLog(Vector3 position, Vector3 velocity, Quaternion rotation)
    {
        if(loopList.Count < 30)
        {
            return true;
        }
        for(int i=loopList.Count - 1; i > 0 && i > loopList.Count - 30; i--)
        {
            if(Vector3.Distance(loopList[i].position, position) > 0.01f || Vector3.Distance(loopList[i].velocity, velocity) > 0.01f || Quaternion.Angle(loopList[i].rotation, rotation) > 1f)
            {
                return true;
            }
        }
        return false;
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
