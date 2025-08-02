using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHandler : MonoBehaviour, EventHandler
{
    [SerializeField] private float movementDuration;
    [SerializeField] private Vector3 destination;
    private Vector3 originalPosition;
    private Rigidbody rb;
    private bool moving;
    private float maxDistanceDelta;
    [SerializeField] private bool destinationIsRelative;    // controls whether the supplied destination is relative to that starting position

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = rb.position;

        if (destinationIsRelative)
        {
            destination = originalPosition + destination;
        }

        float distance = Vector3.Distance(originalPosition, destination);
        maxDistanceDelta = distance / movementDuration / 50;
    }

    void FixedUpdate()
    {
        if (GlobalLoopList.RBList.Contains(rb))
            return;

        if(moving)
        {
            if(Vector3.Distance(rb.position, destination) > 0.05f)
            {
                rb.MovePosition(Vector3.MoveTowards(rb.position, destination, maxDistanceDelta));
            }
        }
        else
        {
            if (Vector3.Distance(rb.position, originalPosition) > 0.05f)
            {
                rb.MovePosition(Vector3.MoveTowards(rb.position, originalPosition, maxDistanceDelta));
            }
        }
    }

    public void activate()
    {
        moving = true;
    }

    public void deactivate()
    {
        moving = false;
    }
}
