using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Pickaxe_Controller : MonoBehaviour
{
    Animation_Controller animCtrl;
    Player_Controller pc;
    Rigidbody rb;

    public Transform holdPosR, holdPosL;

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

    // Start is called before the first frame update
    void Start()
    {
        animCtrl = FindObjectOfType<Animation_Controller>();
        pc = FindObjectOfType<Player_Controller>();
        rb = GetComponent<Rigidbody>();

        transform.position = holdPosR.position;
        transform.parent = holdPosR;

        //Swing
        isSwingingPick = false;

        //Throw
        canThrowPick = false;
        hasThrownPick = false;

        //Recall
        isRecallingPick = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawLine(transform.position, transform.position + pc.transform.right * 10, Color.red);

        ButtonHeldCheck();

        if (!hasThrownPick && !isSwingingPick)
        {
            animCtrl.ChangeAnimationState(animCtrl.idle);

            if (Input.GetKeyUp(KeyCode.Mouse0) && !canThrowPick)
            {
                SwingPickaxe();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0) && canThrowPick)
            {
                ThrowPickaxe();
            }
        } 

        if (hasThrownPick)
        {
            if(!rb.isKinematic)
            {
                transform.Rotate(transform.forward * spinSpeed); 
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

            recallDist = Vector3.Distance(transform.position, pc.transform.position);

            if (recallDist < 2)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;

                isRecallingPick = false;
                hasThrownPick = false;

                SetPickPosition();
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
                canThrowPick = true;
            }
        }
    }

    public void SetPickPosition()
    {
        if (pc.facingRight)
        {
            transform.position = holdPosR.position;
            transform.parent = holdPosR;
        }
        else if (!pc.facingRight)
        {
            transform.position = holdPosL.position;
            transform.parent = holdPosL;
        }

        transform.rotation = Quaternion.Euler(0,0,0);
    }

    void SwingPickaxe()
    {
        Debug.Log("Swing Pickaxe");
        isSwingingPick = true;

        if(pc.facingRight)
        {
            animCtrl.ChangeAnimationState(animCtrl.SwingPick_R);
        }
        else if (!pc.facingRight)
        {
            animCtrl.ChangeAnimationState(animCtrl.SwingPick_L);
        }
    }

    void ThrowPickaxe()
    {
        rb.isKinematic = false;
        transform.parent = null;
        
        if (pc.facingRight)
        {
            spinSpeed = -7;
            rb.AddForce(pc.transform.right * throwForce, ForceMode.Impulse);
        }
        else if (!pc.facingRight)
        {
            spinSpeed = 7;
            rb.AddForce(-pc.transform.right * throwForce, ForceMode.Impulse);
        }

        hasThrownPick = true;
        canThrowPick = false;
    }

    void RecallPickaxe()
    {
        Debug.Log("Recall Pickaxe");

        if (rb.isKinematic)
        {
            rb.isKinematic = false;
        }

        Vector3 recallDir = (pc.transform.position - transform.position).normalized * throwForce;

        rb.velocity = recallDir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasThrownPick && !rb.isKinematic)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                rb.isKinematic = true;

                Vector3 dir = (other.transform.position - transform.position).normalized;
                float direction = Vector3.Dot(dir, pc.transform.right);

                if (direction < 0)
                {
                    Debug.Log("Object is Left");
                    transform.rotation = Quaternion.Euler(0, 0, 70);
                }
                else
                {
                    Debug.Log("Object is Right");
                    transform.rotation = Quaternion.Euler(0, 0, -70);
                }
            }
        }

        if (isSwingingPick)
        {
            if (other.GetComponent<Destructible_Health>() != null)
            {
                Destructible_Health dest_Health = other.GetComponent<Destructible_Health>();
                dest_Health.health --;
            }
        }
    }
}
