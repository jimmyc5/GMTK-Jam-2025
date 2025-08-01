using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerGrab : MonoBehaviour
{
    public Transform grabPosition;

    private Rigidbody grabbedObject;

    public float forceToThrow = 1f;

    public float grabdistance = 2f;

    private float grabForce = 0.3f;

    public TwoBoneIKConstraint leftHandConstraint;
    public TwoBoneIKConstraint rightHandConstraint;

    public float handSpeed;

    public LayerMask layersToGrabFrom;

    void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E))
        {
            if (!grabbedObject)
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var HitInfo, Vector3.Distance(Camera.main.transform.position, transform.position) + grabdistance, layersToGrabFrom))
                {
                    Rigidbody hitObject = HitInfo.collider.GetComponent<Rigidbody>();
                    if (hitObject && !hitObject.isKinematic)
                    {
                        if (!grabbedObject)
                        {
                            grabForce = 0f;
                            grabbedObject = hitObject;
                            hitObject.useGravity = false;
                            hitObject.gameObject.layer = 17;
                        }
                    }
                }
            }
            else
            {
                Vector3 direction = Camera.main.transform.forward;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var HitInfo, 100.0f))
                {
                    direction = (HitInfo.point - grabbedObject.position).normalized;
                }
                else
                {
                    direction = (Camera.main.transform.position + Camera.main.transform.forward * 100f - grabbedObject.position).normalized;
                }
                grabbedObject.gameObject.layer = 0;
                grabbedObject.useGravity = true;
                grabbedObject.AddForce(direction * forceToThrow, ForceMode.Impulse);
                grabbedObject = null;
            }
        }

        if (grabbedObject)
        {
            leftHandConstraint.weight = Mathf.Min(leftHandConstraint.weight + handSpeed * Time.deltaTime, 1f);
            rightHandConstraint.weight = Mathf.Min(rightHandConstraint.weight + handSpeed * Time.deltaTime, 1f);
        }
        else
        {
            leftHandConstraint.weight = Mathf.Max(leftHandConstraint.weight - handSpeed * Time.deltaTime, 0f);
            rightHandConstraint.weight = Mathf.Max(rightHandConstraint.weight - handSpeed * Time.deltaTime, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (grabbedObject)
        {
            grabbedObject.velocity = grabForce * (grabPosition.position - grabbedObject.position) / Time.deltaTime;
            grabForce = Mathf.Min(grabForce + 0.05f, 1f);
        }
    }

}
