using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pickaxe_Controller : MonoBehaviour
{
    Player_Controller pc;
    Rigidbody rb;

    public Transform holdPosR, holdPosL;

    [Header("Swing")]
    bool isSwingingPick;

    [Header("Throw")]
    [SerializeField] float throwDistance, throwForce;
    Vector3 throwDirection;
    float lerpTime = 1f;
    [SerializeField] bool hasThrownPick;

    // Start is called before the first frame update
    void Start()
    {
        pc = FindObjectOfType<Player_Controller>();
        rb = GetComponent<Rigidbody>();

        //transform.position = holdPosR.position;
        //transform.parent = holdPosR;

        //Swing
        isSwingingPick = false;

        //Throw
        hasThrownPick = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasThrownPick && !isSwingingPick)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SwingPickaxe();
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StartCoroutine(ThrowPickaxe());
            }
        }

        if(hasThrownPick)
        {
            if(pc.facingRight)
            {
                throwDirection = pc.transform.position + pc.transform.right * throwDistance;
            }
            else if (!pc.facingRight)
            {
                throwDirection = pc.transform.position + -pc.transform.right * throwDistance;
            }
                
            transform.position = Vector3.MoveTowards(transform.position, throwDirection, throwForce * Time.deltaTime);
            transform.Rotate(0,0, Time.deltaTime * 500);
        }
        else if(!hasThrownPick && transform.position != holdPosL.position || transform.position != holdPosR.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, pc.transform.position, throwForce * Time.deltaTime);
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
    }

    IEnumerator ThrowPickaxe()
    {
        hasThrownPick = true;
        yield return new WaitForSeconds(1);
        hasThrownPick = false;
    }
}
