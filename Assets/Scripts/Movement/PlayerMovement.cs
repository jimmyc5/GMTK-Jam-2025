using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CapsuleCollider col;
    private Transform cam;
    private Vector3 moveDir;
    private Vector3 moveRot;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public float maxSpeed = 6f;
    public float acceleration = 2f;
    public float deccelerationConstant = 0.1f;
    public float jumpForce = 15f;
    private bool jumpInput = false;
    private bool jumpHeld = false;

    private Rigidbody rb;
    private Animator anim;
    private float curXAngle;
    private float curZAngle;

    private Transform characterTF;
    private Transform rotationTF;

    public LayerMask groundMask;
    public float groundCheckDistance = 0.4f;

    public float smallGravityForce;
    public float largeGravityForce;

    private bool isGrounded = false;
    private bool isAlmostGrounded = false;

    private Vector3 groundNormal = Vector3.up;

    public float maxSlopeAngle = 70f;

    private bool isJumping = false;
    private float jumpCutMultiplier = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveDir = new Vector3(0, 0, 0);
        col = GetComponent<CapsuleCollider>();
        cam = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterTF = transform.GetChild(0);
        rotationTF = transform.GetChild(1);
        anim = characterTF.GetComponent<Animator>();
        curXAngle = 0;
        curZAngle = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // convert those inputs into a direction to move the player relative to the camera
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            moveRot = new Vector3(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (moveDir.magnitude > 1f)
            {
                moveDir.Normalize();
            }
        }
        else
        {
            moveDir = new Vector3(0, 0, 0);
        }

        // check for jump inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpInput = true;
            jumpHeld = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpHeld = false;
        }
    }


    // method to check if the player is currently grounded via a sphereCast at the players feet
    private bool updateGroundedInfo()
    {
        isAlmostGrounded = Physics.SphereCast(transform.position + new Vector3(0, col.radius / 3f), col.radius / 3f - 0.03f, Vector3.down, out var almostHit, groundCheckDistance + 1.5f, groundMask, QueryTriggerInteraction.Ignore);
        isGrounded = Physics.SphereCast(transform.position + new Vector3(0, col.radius / 3f), col.radius / 3f - 0.03f, Vector3.down, out var hit, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
        groundNormal = hit.normal;
        return isGrounded;
    }

    private Vector3 getGravity()
    {
        if (rb.velocity.y < 0 || isGrounded)
        {
            return new Vector3(0, -largeGravityForce, 0);
        }
        else
        {
            return new Vector3(0, -smallGravityForce, 0);
        }
    }

    private void FixedUpdate()
    {
        updateGroundedInfo();
        // move/rotate

        // part 1: get to goal velocity
        Vector3 desiredVelocity = projectOnGroundPlane(moveDir) * maxSpeed;
        rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(desiredVelocity.x, rb.velocity.y, desiredVelocity.z), acceleration);

        // Check the direction of the velocity relative to the orientation of the camera, update the animator...
        float desiredXAngle, desiredZAngle;
        if (desiredVelocity != Vector3.zero)
        {
            characterTF.rotation = Quaternion.RotateTowards(characterTF.rotation, rotationTF.rotation, Time.deltaTime * acceleration * 100);
            float angleFromForward = (Vector3.Angle(desiredVelocity, Vector3.forward) * Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(desiredVelocity, Vector3.forward))) - 180f) * -1;
            float orientationAngle = Mathf.Deg2Rad * ((angleFromForward - characterTF.rotation.eulerAngles.y + 360) % 360);
            desiredXAngle = -Mathf.Sin(orientationAngle);
            desiredZAngle = -Mathf.Cos(orientationAngle);
        }
        else
        {
            desiredXAngle = 0;
            desiredZAngle = 0;
        }
        curXAngle = Mathf.MoveTowards(curXAngle, desiredXAngle, Time.deltaTime * acceleration * 2);
        curZAngle = Mathf.MoveTowards(curZAngle, desiredZAngle, Time.deltaTime * acceleration * 2);
        anim.SetFloat("VelocityX", curXAngle);
        anim.SetFloat("VelocityZ", curZAngle);

        // if on the ground and trying to jump, then jump
        if (jumpInput && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
            jumpInput = false;
            isJumping = true;
            anim.SetTrigger("jump");
            anim.SetBool("isJumping", true);
            anim.SetBool("isFalling", false);
        }

        if(rb.velocity.y < 0 && !isGrounded)
        {
            isJumping = false;
            anim.SetBool("isJumping", false);
            if(isAlmostGrounded)
            {
                anim.SetBool("isFalling", false);
            }
            else
            {
                anim.SetBool("isFalling", true);
            }
        }

        if (isJumping && !jumpHeld)
        {
            isJumping = false;
            anim.SetBool("isFalling", true);
            anim.SetBool("isJumping", false);
            if (rb.velocity.y > 0)
            {
                // do a jump cut
                rb.AddForce(Vector2.down * rb.velocity.y * (1 - jumpCutMultiplier), ForceMode.Impulse);
            }
        }

        rb.AddForce(getGravity());
    }

    Vector3 projectOnGroundPlane(Vector3 vector)
    {
        return vector - groundNormal * Vector3.Dot(vector, groundNormal);
    }

    private void OnCollisionStay(Collision collision)
    {
        correctSlopeForces(collision);
    }
        

    public void OnCollisionEnter(Collision collision)
    {
        Vector3 newPos = new Vector3(0,0);
        foreach (ContactPoint contact in collision.contacts)
        {
            newPos += contact.impulse;
        }
        rb.MovePosition(rb.position + newPos * Time.deltaTime *0.001f);
        correctSlopeForces(collision);
    }

    public void correctSlopeForces(Collision collision)
    {
        Vector3 totalForceNormal = new Vector3();
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint point = collision.GetContact(i);
            totalForceNormal += point.impulse;
        }
        float contactAngle = Vector2.Angle(totalForceNormal, Vector2.up);
        // if hitting a walkable sloped surface, cancel out most of the horizontal momentum impacted by landing on it
        if (contactAngle > 0.01f && contactAngle < maxSlopeAngle)
        {
            rb.AddForce(-new Vector3(totalForceNormal.x, 0f, totalForceNormal.z), ForceMode.Impulse);
        }

        // if hitting any kind of floor, re-run the grounded check right away to update early
        if (contactAngle > 0.01f && contactAngle < maxSlopeAngle || totalForceNormal.normalized.y > 0.5f)
        {
            updateGroundedInfo();
        }
    }
}
