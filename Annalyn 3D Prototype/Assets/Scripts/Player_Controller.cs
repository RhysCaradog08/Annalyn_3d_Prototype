using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    Pickaxe_Controller pickCtrl;

    [Header("Visual")]
    [SerializeField] GameObject annalyn_Sprite;
    public bool facingRight;

    [Header("Movement")]
    [SerializeField] CharacterController cc;
    [SerializeField] float speed, moveSpeed;
    Vector3 moveX, moveDir;
    bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        pickCtrl = FindObjectOfType<Pickaxe_Controller>();

        //Sprite
        facingRight = true;

        //Movement
        cc = GetComponent<CharacterController>();
        speed = moveSpeed;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        InputCheck();

        if(canMove)
        {
            if(moveDir.magnitude > Mathf.Epsilon)
            {
                cc.Move(moveDir * moveSpeed * Time.deltaTime);
            }

            if(moveDir.x > 0 && !facingRight)
            {
                Debug.Log("Moving Right");
                FlipSprite();
            }
            else if(moveDir.x < 0 && facingRight)
            {
                Debug.Log("Moving Left");
                FlipSprite();
            }
        }
    }

    void InputCheck()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        moveDir = new Vector3(moveX, 0, 0).normalized;
    }

    void FlipSprite()
    {
        Vector3 spriteScale = annalyn_Sprite.transform.localScale;
        spriteScale.x *= -1;
        annalyn_Sprite.transform.localScale = spriteScale;

        facingRight = !facingRight;

        //pickCtrl.SetPickPosition();
    }
}
