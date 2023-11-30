using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Pickaxe_Controller : MonoBehaviour
{
    Animation_Controller animCtrl;
    Player_Controller playerCtrl;
    Rigidbody rb;

    public Transform holdPosR, holdPosL;
    Collider pickaxeCollider;
    public bool canDealDamage;

    [Header("Button Held Check")]
    const float minButtonHold = 0.25f;
    float buttonHeldTime;
    public bool buttonHeld;

    [Header("Swing")]
    public bool isSwingingPickaxe;

    [Header("Throw")]
    [SerializeField] float throwDistance, throwForce, spinSpeed;
    Vector3 throwDirection;
    [SerializeField] bool canThrowPickaxe;
    public bool hasThrownPick;

    [Header("Arc Throw")]
    [SerializeField] float arcHeight, arcDistance;
    float gravity = -50;
    [SerializeField] Vector3 arcApex, arcEnd;
    [SerializeField] bool hasThrownInArc;

    [Header("Recall")]
    [SerializeField] float recallDist;
    [SerializeField] bool isRecallingPickaxe;

    [Header("Stick to Wall")]
    [SerializeField] GameObject objectStuckTo;
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

        canDealDamage = false;

        //Swing
        isSwingingPickaxe = false;

        //Throw
        canThrowPickaxe = false;
        hasThrownPick = false;

        //Arc Throw
        rb.useGravity = false;
        hasThrownInArc = false;

        //Recall
        isRecallingPickaxe = false;

        //Stick to Wall
        objectStuckTo = null;
        isStuckToWall = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one;
        //Debug.DrawLine(transform.position, transform.position + pc.transform.right * 10, Color.red);

        ButtonHeldCheck();

        if (!canThrowPickaxe && !isSwingingPickaxe)
        {
            animCtrl.ChangeAnimationState(animCtrl.idle);
        }

        if (!hasThrownPick && !isSwingingPickaxe)
        {
            if (!isStuckToWall)
            {
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (!canThrowPickaxe)
                    {
                        SwingPickaxe();
                    }
                    else ThrowPickaxe();
                }

                if(Input.GetKey(KeyCode.Mouse1))
                {
                    canThrowPickaxe = true;
                }

                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    ArcThrow();
                }
            }
        } 

        if(isSwingingPickaxe || hasThrownPick)
        {
            pickaxeCollider.enabled = true;
        }
        else pickaxeCollider.enabled = false;

        if (canThrowPickaxe)
        {
            PreparePickaxeThrow();
        }

        if (hasThrownPick)
        {

            if (!rb.isKinematic)
            {
                if (playerCtrl.isFacingRight) //Sets rotation of Pickaxe when thrown
                {
                    spinSpeed = -7;
                }
                else if (!playerCtrl.isFacingRight)
                {
                    spinSpeed = 7;
                }
                transform.Rotate(transform.forward * spinSpeed);

                throwDistance = Vector3.Distance(playerCtrl.transform.position, transform.position);

                if (throwDistance >= 20 && !hasThrownInArc)  //Starts recalling Pickaxe like a boomerang if it hasn't collided with anything over a set distance
                {
                    isRecallingPickaxe = true;
                }
            }

            if(Input.GetKeyDown(KeyCode.Mouse0) ||  Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (rb.isKinematic && !isRecallingPickaxe)
                {
                    isRecallingPickaxe = true;
                }
            }
        }

        if (isRecallingPickaxe)
        {
            RecallPickaxe();

            recallDist = Vector3.Distance(transform.position, playerCtrl.transform.position);

            if (recallDist < 2) //Resets Pickaxe velocity and position when close enough to the player
            {
                if(hasThrownInArc)
                {
                    hasThrownInArc = false;
                }

                if (canDealDamage)
                {
                    canDealDamage = false;
                }

                rb.velocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;

                isRecallingPickaxe = false;
                hasThrownPick = false;

                SetPickPosition();

                recallDist = 0;
                throwDistance = 0;
            }

        }

        if (isStuckToWall)
        {
            StickToWall();

            if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1) || !objectStuckTo)
            {
                isStuckToWall = false;
                playerCtrl.gravity = playerCtrl.currentGravity;
                objectStuckTo = null;
            }
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
                canThrowPickaxe = true;
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
        isSwingingPickaxe = true;
        canDealDamage = true;

        if(playerCtrl.isFacingRight)  //Sets animation to swing Pickaxe
        {
            animCtrl.ChangeAnimationState(animCtrl.SwingPick_R);
        }
        else if (!playerCtrl.isFacingRight)
        {
            animCtrl.ChangeAnimationState(animCtrl.SwingPick_L);
        }
    } 

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
        hasThrownPick = true;
        canThrowPickaxe = false;
        canDealDamage = true;

        rb.isKinematic = false;
        transform.parent = null;
        
        if (playerCtrl.isFacingRight) //Will throw Pickaxe in direction that player is facing
        {
            rb.AddForce(playerCtrl.transform.right * throwForce, ForceMode.Impulse);
        }
        else if (!playerCtrl.isFacingRight)
        {
            rb.AddForce(-playerCtrl.transform.right * throwForce, ForceMode.Impulse);
        }

    } 

    Vector3 CalculateArcVelocity()
    {
        Vector3 playerPosition = playerCtrl.transform.position;

        if (playerCtrl.isFacingRight)
        {
            arcApex = new Vector3(playerPosition.x + arcDistance / 2, playerPosition.y + arcHeight, 0);
            arcEnd = new Vector3(playerPosition.x + arcDistance, 0, 0);
        }
        else if (!playerCtrl.isFacingRight)
        {
            arcApex = new Vector3(playerPosition.x - arcDistance / 2, playerPosition.y + arcHeight, 0);
            arcEnd = new Vector3(playerPosition.x - arcDistance, 0, 0);
        }

        float displacementY = (arcEnd.y - rb.position.y) + (arcApex.y);
        Vector3 displacementX = new Vector3(arcEnd.x - rb.position.x, 0, 0);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * arcHeight);
        Vector3 velocityX = displacementX / (Mathf.Sqrt(-2 * arcHeight/ gravity) + (Mathf.Sqrt(2 * displacementY - arcHeight) / gravity));

        return velocityX + velocityY;
    }

    void ArcThrow() //Throws Pickaxe in an Arc to reach and clear higher targets and obstacles
    {
        Physics.gravity = Vector3.up * gravity;
        rb.useGravity = true;
        rb.isKinematic = false;
        transform.parent = null;    
        rb.velocity = CalculateArcVelocity();

        hasThrownInArc = true;
        hasThrownPick = true;
        canThrowPickaxe = false;
        canDealDamage = true;
    }

    void RecallPickaxe()  //Recalls Pickaxe to Players current position
    {
        //Debug.Log("Recall Pickaxe");

        if (rb.isKinematic)
        {
            rb.isKinematic = false;
        }

        Vector3 recallDir = (playerCtrl.transform.position - transform.position).normalized * (throwForce * 1.5f);

        rb.velocity = recallDir;
    } 

    void StickToWall()
    {
        //Debug.Log("Stick to Wall");
        playerCtrl.gravity = 0;
        playerCtrl.velocity = Vector3.zero;

        Vector3 currentPlayerPosition = playerCtrl.transform.position;
        playerCtrl.transform.position = currentPlayerPosition;

        if (playerCtrl.isFacingRight)
        {
            transform.rotation = Quaternion.Euler(0, 0, -70);
        }
        else transform.rotation = Quaternion.Euler(0, 0, 70);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);

        if (hasThrownPick)
        {
            if (!other.gameObject.CompareTag("Player") && !other.GetComponent<Destructible_Health>()) //Pick will stick in object collided with
            {
                if (!rb.isKinematic)
                {
                    rb.isKinematic = true;

                    Vector3 dir = (other.transform.position - transform.position).normalized;
                    float directionX = Vector3.Dot(dir, playerCtrl.transform.right);
                    float directionY = Vector3.Dot(dir, playerCtrl.transform.up);

                    if (directionX < 0) //Uses dot product to make Pickaxe stick out of contacted object
                    {
                        //Debug.Log("Object is Left");
                        transform.rotation = Quaternion.Euler(0, 0, 70);
                    }
                    else if (directionX > 0)
                    {
                        //Debug.Log("Object is Right");
                        transform.rotation = Quaternion.Euler(0, 0, -70);
                    }
                }

                if (hasThrownInArc)
                {
                    isRecallingPickaxe = true;
                }
            }
        }

        if (isSwingingPickaxe && !playerCtrl.charCtrl.isGrounded)
        {
            if (!other.CompareTag("Player") || !other.CompareTag("Switch") && other.gameObject.activeInHierarchy)
            {
                objectStuckTo = other.gameObject;
                playerCtrl.currentGravity = playerCtrl.gravity;
                isStuckToWall = true;
            }
        }

        if (other.GetComponent<Destructible_Health>() != null) //Damages destructible objects with Destructible_Health script attached
        {
            Debug.Log("Damage Object");

            Destructible_Health dest_Health = other.GetComponent<Destructible_Health>();

            if (dest_Health != null)  //Will recall Pickaxe after delaing damage to Destructible_Health object
            {
                if (canDealDamage)
                {
                    dest_Health.health--;
                    
                    if(hasThrownInArc)
                    {
                        canDealDamage = false;
                    }
                }

                if (hasThrownPick)
                {
                    isRecallingPickaxe = true;
                }
            }
        }

        if (other.CompareTag("Switch"))
        {
            Debug.Log(other.name);

            Interactivate_Element_Activation interact = other.gameObject.GetComponent<Interactivate_Element_Activation>();

            if (interact != null)
            {
                interact.isActivated = true;
            }
        }
    }
}
