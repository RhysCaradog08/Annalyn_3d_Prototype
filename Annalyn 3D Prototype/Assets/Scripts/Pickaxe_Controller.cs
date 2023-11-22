using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField] float throwDistance, throwForce;
    Vector3 throwDirection;
    float lerpTime = 1f;
    [SerializeField] bool canThrowPick, hasThrownPick;

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
    }

    // Update is called once per frame
    void Update()
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
            RecallPickaxe();
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
            rb.AddForce(pc.transform.right * throwForce, ForceMode.Impulse);
        }
        else if (!pc.facingRight)
        {
            rb.AddForce(-pc.transform.right * throwForce, ForceMode.Impulse);
        }
        hasThrownPick = true;
    }

    void RecallPickaxe()
    {
        Debug.Log("Recall Pickaxe");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasThrownPick && !rb.isKinematic)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                rb.isKinematic = true;
            }
        }
    }
}
