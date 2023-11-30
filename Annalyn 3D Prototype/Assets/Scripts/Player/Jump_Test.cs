using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_Test : MonoBehaviour
{
    [SerializeField] CharacterController cc;

    [Header("Jumping")]
    [SerializeField] float gravity;
    public Vector3 velocity;
    public float jumpHeight, jumpSpeed, timeToJumpApex, lowJumpMultiplier;
    public bool hasJumped, isJumping, canPressSpace;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();   

        //Jumping
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpSpeed = Mathf.Abs(gravity) * timeToJumpApex;
        hasJumped = false;
        isJumping = false;
        canPressSpace = true;
        isJumping = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckJump();

        if (cc.isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }
        else velocity.y += gravity * Time.deltaTime;
        Debug.Log("Is Grounded " + cc.isGrounded);

        if (velocity.y > 0)
        {
            isJumping = true;
        }
        else if (velocity.y < 0)
        {
            isJumping = false;
        }

        if (velocity.y > 0 && !Input.GetKey(KeyCode.Space))  //Allows for a brief Jump action to be performed.
        {
            velocity.y += gravity * (lowJumpMultiplier + 1) * Time.deltaTime;
        }

        cc.Move(velocity * Time.deltaTime);
    }

    void CheckJump()
    {
        if (Input.GetKeyUp(KeyCode.Space) && !hasJumped) //Check to stop infinite jumping.
        {
            canPressSpace = true;
        }

        if (cc.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) && canPressSpace)
            {
                Debug.Log("Jump!");
                Jump();
            }

            if (hasJumped)  //Sets Jump animation and prevents player from additional jumps once the Jump action is performed.
            {
                SetJumpBoolsToFalse();
            }
        }
    }

    void Jump()
    {
        velocity.y = jumpSpeed;
        hasJumped = true;
    }

    void SetJumpBoolsToFalse()
    {
        canPressSpace = false;
        hasJumped = false;
    }
}
