using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using TMPro;

public class PlayerGrab : MonoBehaviour
{
    public Transform grabPosition;

    public Rigidbody grabbedObject;

    public float forceToThrow = 1f;

    public float grabdistance = 2f;

    private float grabForce = 0.3f;

    private bool goingToHands = false;

    public TwoBoneIKConstraint leftHandConstraint;
    public TwoBoneIKConstraint rightHandConstraint;

    public float handSpeed;

    public LayerMask layersToGrabFrom;
    public Vector3 grabPositionChange;

    private FixedJoint joint = null;
    public float breakGrabForce = 100f;
    public PhysicMaterial materialToApplyToHeldThings;
    public PhysicMaterial materialToApplyToDroppedThings;
    private Collider itemCollider = null;

    private bool forceGrabNextStep = false;

    public AudioClip grabSound;

    public TMP_Text tutorialText;
    private bool tutorialDone = false;

    void Update()
    {
        if (!goingToHands && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E)))
        {
            if (!grabbedObject)
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var HitInfo, Vector3.Distance(Camera.main.transform.position, transform.position) + grabdistance, layersToGrabFrom))
                {
                    Rigidbody hitObject = HitInfo.collider.attachedRigidbody;
                    bool canPickupLoopingObject = false;
                    if(hitObject && GlobalLoopList.RBList.Contains(hitObject))
                    {
                        LoopController lc = hitObject.gameObject.GetComponent<LoopController>();
                        if(!lc.getWasKinematic())
                        {
                            canPickupLoopingObject = true;
                            lc.endLoop();
                            hitObject.transform.parent.GetChild(1).GetComponent<ParticleSystem>().Stop();
                        }
                    }
                    if (hitObject && (!hitObject.isKinematic || canPickupLoopingObject))
                    {
                        itemCollider = HitInfo.collider;
                        grabbedObject = hitObject;
                        grabForce = 0.2f;
                        grabbedObject = hitObject;
                        hitObject.useGravity = false;
                        hitObject.gameObject.layer = 17;
                        goingToHands = true;
                        SoundManager.instance.PlaySoundClip(grabSound, transform.position, 0.7f, Random.Range(1.1f,1.2f));
                        if(tutorialText && tutorialText.isActiveAndEnabled)
                        {
                            tutorialText.text = "Press E again to drop boxes";
                        }
                    }
                }
            }
            else
            {
                breakJoint();
                if (tutorialText && tutorialText.isActiveAndEnabled)
                {
                    tutorialText.gameObject.SetActive(false);
                }
            }
        }




        if (grabbedObject || goingToHands)
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

    public void breakJoint()
    {
        Destroy(joint);
        joint = null;
        //movementScript.turnInDirectionOfMovement = true;
        itemCollider.material = materialToApplyToDroppedThings;
        grabbedObject = null;
        goingToHands = false;
        SoundManager.instance.PlaySoundClip(grabSound, transform.position, 0.7f, Random.Range(0.8f, 0.7f));
    }

    // will be called when an excessive outside force breaks the joint between the player and what they're holding
    private void OnJointBreak(float breakForce)
    {
        joint = null;
        //movementScript.turnInDirectionOfMovement = true;
        itemCollider.material = materialToApplyToDroppedThings;
        grabbedObject = null;
        goingToHands = false;
        itemCollider = null;
        SoundManager.instance.PlaySoundClip(grabSound, transform.position, 0.7f, Random.Range(0.8f, 0.7f));
    }

    private void FixedUpdate()
    {
        if (goingToHands && (Vector3.Distance(grabbedObject.position, grabPosition.position) < 0.01f || forceGrabNextStep))
        {
            joint = gameObject.AddComponent<FixedJoint>() as FixedJoint;
            joint.connectedBody = grabbedObject;
            joint.breakForce = breakGrabForce;
            if (materialToApplyToHeldThings && materialToApplyToHeldThings.dynamicFriction < itemCollider.material.dynamicFriction)
            {
                itemCollider.material = materialToApplyToHeldThings;
            }
            grabbedObject.gameObject.layer = 14;
            grabbedObject.useGravity = true;
            goingToHands = false;
            forceGrabNextStep = false;
        }

        if (goingToHands && grabbedObject)
        {
            grabbedObject.velocity = grabForce * (grabPosition.position - grabbedObject.position) / Time.deltaTime;
            
            if(grabForce >= 1f)
            {
                grabbedObject.MovePosition(grabPosition.position);
                forceGrabNextStep = true;
            }
            else
            {
                grabForce = Mathf.Min(grabForce + 0.1f, 1f);
            }
        }
    }

}
