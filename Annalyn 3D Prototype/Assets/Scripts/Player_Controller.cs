using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player_Controller : MonoBehaviour
{
    Pickaxe_Controller pickCtrl;

    [Header("Visual")]
    [SerializeField] GameObject annalyn_Sprite;
    public bool isFacingRight;

    [Header("Movement")]
    [SerializeField] CharacterController cc;
    [SerializeField] float speed, moveSpeed;
    [SerializeField] Vector3 moveDir;
    bool canMove;

    [Header("Jumping")]
    [SerializeField] float gravity;
    public Vector3 velocity;
    public float jumpHeight, jumpSpeed, timeToJumpApex, lowJumpMultiplier;
    [SerializeField] float canJumpTime;
    public bool hasJumped, canPressSpace;

    // Start is called before the first frame update
    void Start()
    {
        pickCtrl = FindObjectOfType<Pickaxe_Controller>();

        //Sprite
        isFacingRight = true;

        //Movement
        cc = GetComponent<CharacterController>();
        speed = moveSpeed;
        canMove = true;

        //Jumping
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpSpeed = Mathf.Abs(gravity) * timeToJumpApex;
        hasJumped = false;
        canPressSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        InputCheck();
        JumpCheck();
        ApplyGravity();

        Debug.Log("Is Grounded " + cc.isGrounded);        

        if (canMove)
        {
            if(moveDir.magnitude > Mathf.Epsilon)
            {
                cc.Move(moveDir * moveSpeed * Time.deltaTime);
            }

            if(moveDir.x > 0 && !isFacingRight)
            {
                Debug.Log("Moving Right");
                FlipSprite();
            }
            else if(moveDir.x < 0 && isFacingRight)
            {
                Debug.Log("Moving Left");
                FlipSprite();
            }
        }

        cc.Move(velocity * Time.deltaTime);

        speed = moveSpeed;
    }

    void InputCheck()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        moveDir = new Vector3(moveX, 0, 0).normalized;

        if (Input.GetKeyUp(KeyCode.Space) && !hasJumped) //Check to stop infinite jumping.
        {
            canPressSpace = true;
        }      
    }

    void JumpCheck()
    {
        if (cc.isGrounded)  //canJumpTime allows for "Coyote Time" jumps to be performed
        {
            canJumpTime = 0.5f;
        }
        else canJumpTime -= Time.deltaTime;

        if (canJumpTime <= 0 && !cc.isGrounded)
        {
            canJumpTime = 0;
        }

        if (canJumpTime > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) && canPressSpace)
            {
                //Debug.Log("Jump!");
                Jump();
            }

            if (hasJumped)  //Prevents player from additional jumps once the Jump action is performed.
            {
                SetJumpBoolsToFalse();
            }
        }
    }

    void Jump()
    {
        velocity.y = jumpSpeed;
        canJumpTime = 0;
        hasJumped = true;
    }

    void SetJumpBoolsToFalse()
    {
        canPressSpace = false;
        hasJumped = false;
    }

    void ApplyGravity()  //Applies relevant gravity modifier to player dependent on current situation
    {
        if (cc.isGrounded && velocity.y < 0)  //Keeps player grounded
        {
            velocity.y = -2;
        }
        else velocity.y += gravity * Time.deltaTime;

        if (velocity.y > 0 && !Input.GetKey(KeyCode.Space))  //Allows for a brief Jump action to be performed.
        {
            velocity.y += gravity * (lowJumpMultiplier + 1) * Time.deltaTime;
        }
    }

    void FlipSprite()  //Will flip player sprite to face appropriate direction of locomotion
    {
        Vector3 spriteScale = annalyn_Sprite.transform.localScale;
        spriteScale.x *= -1;
        annalyn_Sprite.transform.localScale = spriteScale;

        isFacingRight = !isFacingRight;

        if (!pickCtrl.hasThrownPick)
        {
            pickCtrl.SetPickPosition();
        }
    }
}
