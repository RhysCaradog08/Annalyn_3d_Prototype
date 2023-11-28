using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Pickaxe_Controller : MonoBehaviour
{
    Animation_Controller animCtrl;
    Player_Controller playerCtrl;
    Rigidbody rb;

    public Transform holdPosR, holdPosL;
    Collider pickaxeCollider;

    [Header("Button Held Check")]
    const float minButtonHold = 0.25f;
    float buttonHeldTime;
    public bool buttonHeld;

    [Header("Swing")]
    public bool isSwingingPick;

    [Header("Throw")]
    [SerializeField] float throwDistance, throwForce, spinSpeed;
    Vector3 throwDirection;
    [SerializeField] bool canThrowPick;
    public bool hasThrownPick;

    [Header("Recall")]
    [SerializeField] float recallDist;
    [SerializeField] bool isRecallingPick;

    [Header("Stick to Wall")]
    public float currentGravity;
    public bool isStuckToWall;

    // Start is called before the first frame update
    void Start()
    {
        animCtrl = FindObjectOfType<Animation_Controller>();
        playerCtrl = FindObjectOfType<Player_Controller>();
        rb = GetComponent<Rigidbody>();

        transform.position = holdPosR.position;
        transform.parent = holdPosR;
        pickaxeCollider = GetComponent<Collider>();
        pickaxeCollider.enabled = false;

        //Swing
        isSwingingPick = false;

        //Throw
        canThrowPick = false;
        hasThrownPick = false;

        //Recall
        isRecallingPick = false;

        //Stick to Wall
        isStuckToWall = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one;
        //Debug.DrawLine(transform.position, transform.position + pc.transform.right * 10, Color.red);

        ButtonHeldCheck();

        if (!hasThrownPick && !isSwingingPick)
        {
            if (!canThrowPick)
            {
                animCtrl.ChangeAnimationState(animCtrl.idle);
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && !canThrowPick)
            {
                SwingPickaxe();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && canThrowPick)
            {
                ThrowPickaxe();
            }
        } 

        if(isSwingingPick || hasThrownPick)
        {
            pickaxeCollider.enabled = true;
        }
        else pickaxeCollider.enabled = false;

        if (canThrowPick)
        {
            PreparePickaxeThrow();
        }

        if (hasThrownPick)
        {
            if(!rb.isKinematic)
            {
                transform.Rotate(transform.forward * spinSpeed);

                throwDistance = Vector3.Distance(playerCtrl.transform.position, transform.position);

                if (throwDistance >= 20)  //Starts recalling Pickaxe like a boomerang if it hasn't collided with anything over a set distance
                {
                    isRecallingPick = true;
                }
            }
        }
        
        if (hasThrownPick && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (rb.isKinematic && !isRecallingPick)
            {
                isRecallingPick = true;
            }
        }

        if (isRecallingPick)
        {
            RecallPickaxe();

            recallDist = Vector3.Distance(transform.position, playerCtrl.transform.position);

            if (recallDist < 2) //Resets Pickaxe velocity and position when close enough to the player
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;

                isRecallingPick = false;
                hasThrownPick = false;

                SetPickPosition();

                recallDist = 0;
                throwDistance = 0;
            }

        }

        if (isStuckToWall)
        {
            StickToWall();
        }
    }

    void ButtonHeldCheck()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) //&& cc.isGrounded)//Button is pressed down. Need to check to see if it is "held".
        {
            buttonHeldTime = Time.timeSinceLevelLoad;
            buttonHeld = false;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0)) // && cc.isGrounded)
        {
            buttonHeld = false;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Time.timeSinceLevelLoad - buttonHeldTime > minButtonHold)//Button is considered "held" if it is actually held down.
            {
                buttonHeld = true;
                canThrowPick = true;
            }
        }
    } //Checks if button is being held down or pressed for distinct actions

    public void SetPickPosition() //Sets Pickaxe to be in the players front hand
    {

        if (playerCtrl.isFacingRight)
        {
            transform.position = holdPosR.position;
            transform.parent = holdPosR;
        }
        else if (!playerCtrl.isFacingRight)
        {
            transform.position = holdPosL.position;
            transform.parent = holdPosL;
        }

        transform.localRotation = Quaternion.Euler(0,0,0);
    }

    void SwingPickaxe()
    {
        //Debug.Log("Swing Pickaxe");
        isSwingingPick = true;

        if(playerCtrl.isFacingRight)
        {
            animCtrl.ChangeAnimationState(animCtrl.SwingPick_R);
        }
        else if (!playerCtrl.isFacingRight)
        {
            animCtrl.ChangeAnimationState(animCtrl.SwingPick_L);
        }
    } //Sets animation to swing Pickaxe

    void PreparePickaxeThrow() //Sets animation to show Pickaxe is ready to be thrown
    {
        if (playerCtrl.isFacingRight)
        {
            animCtrl.ChangeAnimationState(animCtrl.PrepThrow_R);
        }
        else if (!playerCtrl.isFacingRight)
        {
            animCtrl.ChangeAnimationState(animCtrl.PrepThrow_L);
        }
    } 

    void ThrowPickaxe()
    {
        rb.isKinematic = false;
        transform.parent = null;
        
        if (playerCtrl.isFacingRight)
        {
            spinSpeed = -7;
            rb.AddForce(playerCtrl.transform.right * throwForce, ForceMode.Impulse);
        }
        else if (!playerCtrl.isFacingRight)
        {
            spinSpeed = 7;
            rb.AddForce(-playerCtrl.transform.right * throwForce, ForceMode.Impulse);
        }

        hasThrownPick = true;
        canThrowPick = false;
    } //Will throw Pickaxe in direction that player is facing

    void RecallPickaxe()
    {
        Debug.Log("Recall Pickaxe");

        if (rb.isKinematic)
        {
            rb.isKinematic = false;
        }

        Vector3 recallDir = (playerCtrl.transform.position - transform.position).normalized * throwForce;

        rb.velocity = recallDir;
    } //Recalls Pickaxe to Players current position

    void StickToWall()
    {
        Debug.Log("Stick to Wall");
        playerCtrl.gravity = 0;
        playerCtrl.velocity = Vector3.zero;

        Vector3 currentPlayerPosition = playerCtrl.transform.position;
        playerCtrl.transform.position = currentPlayerPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);

        if (other.GetComponent<Destructible_Health>() != null) //Damages destructible objects with Destructible_Health script attached
        {
            Destructible_Health dest_Health = other.GetComponent<Destructible_Health>();

            if (dest_Health != null)  //Will recall Pickaxe after delaing damage to Destructible_Health object
            {
                if (isSwingingPick || hasThrownPick)
                {
                    dest_Health.health--;                   
                }

                if(hasThrownPick)
                {
                    isRecallingPick = true;
                }
            }
        }

        if (hasThrownPick)
        {
            if (!other.gameObject.CompareTag("Player")) //Pick will stick in object collided with
            {
                if (!rb.isKinematic)
                {
                    rb.isKinematic = true;

                    Vector3 dir = (other.transform.position - transform.position).normalized;
                    float direction = Vector3.Dot(dir, playerCtrl.transform.right);

                    if (direction < 0) //Uses dot product to make Pickaxe stick out of contacted object
                    {
                        //Debug.Log("Object is Left");
                        transform.rotation = Quaternion.Euler(0, 0, 70);
                    }
                    else
                    {
                        //Debug.Log("Object is Right");
                        transform.rotation = Quaternion.Euler(0, 0, -70);
                    }
                }
            }
        }

        if(isSwingingPick && !playerCtrl.charCtrl.isGrounded)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                playerCtrl.currentGravity = playerCtrl.gravity;
                isStuckToWall = true;

                Vector3 dir = (other.transform.position - transform.position).normalized;
                float direction = Vector3.Dot(dir, playerCtrl.transform.right);

                if (direction < 0) //Uses dot product to make Pickaxe stick out of contacted object
                {
                    //Debug.Log("Object is Left");
                    transform.rotation = Quaternion.Euler(0, 0, 70);
                }
                else
                {
                    //Debug.Log("Object is Right");
                    transform.rotation = Quaternion.Euler(0, 0, -70);
                }
            }
        }
    }
}
