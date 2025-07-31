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

    public bool loopTestSwitch;

    // Start is called before the first frame update
    void Start()
    {
        loopList = new List<(Vector3 position, Vector3 velocity, Quaternion rotation)>();
        rb = GetComponent<Rigidbody>();
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
            rb.MovePosition(loopItem.position);
            rb.velocity = loopItem.velocity;
            rb.rotation = loopItem.rotation;

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
        else if(rb.velocity != Vector3.zero)
        {
            // Add to loop List if object is moving at all and not looping
            loopList.Add((rb.position, rb.velocity, rb.rotation));
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
        GlobalLoopList.RBList.Add(rb);
        objectIsLooping = true;
        loopBackward = true;
        loopTracker = loopList.Count - 1;
    }

    // Ends loop and removes all items from loop List above the current loop tracker
    public void endLoop()
    {
        objectIsLooping = false;
        loopList.RemoveRange(loopTracker + 1, loopList.Count - loopTracker - 1);
        GlobalLoopList.RBList.Remove(rb);
    }
}
