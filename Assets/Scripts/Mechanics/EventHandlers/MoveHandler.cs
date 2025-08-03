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

    private AudioSource movingSound;
    private float startingVolume;
    public float volumeGain;

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
        movingSound = GetComponent<AudioSource>();
        if (movingSound)
        {
            startingVolume = movingSound.volume;
            movingSound.volume = 0f;
        }

    }

    void FixedUpdate()
    {
        if (GlobalLoopList.RBList.Contains(rb))
            return;

        Vector3 difference = new Vector3(0, 0, 0);
        if(moving)
        {
            if (Vector3.Distance(rb.position, destination) > 0.05f)
            {
                Vector3 newPosition = Vector3.MoveTowards(rb.position, destination, maxDistanceDelta);
                difference = newPosition - rb.position;
                rb.MovePosition(newPosition);
            }
        }
        else
        {
            if (Vector3.Distance(rb.position, originalPosition) > 0.05f)
            {
                Vector3 newPosition = Vector3.MoveTowards(rb.position, originalPosition, maxDistanceDelta);
                difference = newPosition - rb.position;
                rb.MovePosition(newPosition);
            }
        }
        if (movingSound)
            movingSound.volume =  Mathf.MoveTowards(movingSound.volume, startingVolume * (difference.magnitude / Time.deltaTime), volumeGain / Time.deltaTime);
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
