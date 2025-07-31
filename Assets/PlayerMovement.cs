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

    public LayerMask groundMask;
    public float groundCheckDistance = 0.4f;

    public float smallGravityForce;
    public float largeGravityForce;

    private bool isGrounded = false;

    private Vector3 groundNormal = Vector3.up;

    public float maxSlopeAngle = 70f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveDir = new Vector3(0, 0, 0);
        col = GetComponent<CapsuleCollider>();
        cam = Camera.main.transform;
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
    private bool updateIsGrounded()
    {
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

    // function to get the final gravity to be applied to the player, including cancelling any sideways forces the gravity would create from being on a slope
    // (this involves making a raycast at the player's feet to see the angle of the slope they're standing on, and then doing some math to cancel out any sideways forces that would be created by that slope)
    private void addGravityButAccountForSlopes()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hit, groundCheckDistance + col.radius / 2f - 0.1f, groundMask, QueryTriggerInteraction.Ignore))
        {
            var collider = hit.collider;
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            Vector3 gravityForce = getGravity();
            float sidewaysForce = gravityForce.magnitude * Mathf.Sin(Mathf.Deg2Rad * angle);
            Vector3 directionOfSidewaysForce = new Vector3(hit.normal.x, 0, hit.normal.z);
            directionOfSidewaysForce.Normalize();
            // get the full force vector pushing the player off the slope and causing them to slide
            Vector3 sideForceVector = -directionOfSidewaysForce * sidewaysForce;
            Debug.Log(sideForceVector + gravityForce);
            rb.AddForce(sideForceVector + gravityForce);
        }
        else
        {
            rb.AddForce(getGravity());
        }
    }

    private void FixedUpdate()
    {
        updateIsGrounded();
        // move/rotate

        // part 1: get to goal velocity
        // this whole thing is a bit hacky
        // ideally increase your velocity in the direction of input by the acceleration parameter
        Vector3 desiredVelocity = projectOnGroundPlane(moveDir) * maxSpeed;
        //Vector3 moveVelocity = projectOnGroundPlane(moveDir * acceleration);
        //Vector3 currentHorizontalVelocity = new Vector3(rb.velocity.x, rb.v, rb.velocity.z);

        //// deccelerate in all directions (adding a bit of friction/responsiveness here)
        //if (currentHorizontalVelocity.magnitude > 1)
        //{
        //    // Cap deceleration in cases where velocity is really high (don't want to stop on a dime when moving really fast)
        //    // this will create a linear deceleration when moving fast (>1 m/s)
        //    moveVelocity += -currentHorizontalVelocity.normalized * deccelerationConstant;
        //}
        //else
        //{
        //    // if you're not moving fast, deccellerate exponentially (nice for responsiveness)
        //    moveVelocity += -currentHorizontalVelocity * deccelerationConstant;
        //}
        

        //if ((moveVelocity + currentHorizontalVelocity).magnitude <= currentHorizontalVelocity.magnitude)
        //{
        //    // if this change would make us slow down (ie we're going the opposite direction), then just do it
        //    rb.velocity = moveVelocity + currentHorizontalVelocity + new Vector3(0, rb.velocity.y, 0);
        //}
        //else
        //{
            // otherwise, accelerate in the direction of input to a max of the max speed parameter
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(desiredVelocity.x, rb.velocity.y, desiredVelocity.z), acceleration);
            //rb.velocity = Vector3.ClampMagnitude(moveVelocity + currentHorizontalVelocity, Mathf.Max(maxSpeed, currentHorizontalVelocity.magnitude)) + new Vector3(0, rb.velocity.y, 0);
        //}


        //rb.MoveRotation(Quaternion.Euler(moveRot));


        // if on the ground and trying to jump, then jump
        if (jumpInput && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
            jumpInput = false;

        }

        addGravityButAccountForSlopes();
    }

    Vector3 projectOnGroundPlane(Vector3 vector)
    {
        Debug.Log(groundNormal);
        return vector - groundNormal * Vector3.Dot(vector, groundNormal);
    }

    private void OnCollisionStay(Collision collision)
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
    }
}
