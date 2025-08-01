using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    public Transform grabPosition;

    private Rigidbody grabbedObject;

    public float forceToThrow = 10f;

    public float grabdistance = 2f;

    public void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E))
        {
            if (!grabbedObject)
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var HitInfo, Vector3.Distance(Camera.main.transform.position, transform.position) + grabdistance))
                {
                    Rigidbody hitObject = HitInfo.collider.GetComponent<Rigidbody>();
                    if (hitObject && !hitObject.isKinematic)
                    {
                        if (!grabbedObject)
                        {
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
    }

    private void FixedUpdate()
    {
        if(grabbedObject)
            grabbedObject.velocity = 0.3f *  (grabPosition.position - grabbedObject.position) / Time.deltaTime;
    }

}
