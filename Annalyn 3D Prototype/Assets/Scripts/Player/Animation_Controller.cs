using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Animation_Controller : MonoBehaviour
{
    Pickaxe_Controller pickCtrl;

    public Animator anim;
    [SerializeField] string currentState;

    public string idle = "Idle";
    public string SwingPick_L = "SwingPick_L";
    public string SwingPick_R = "SwingPick_R";
    public string PrepThrow_L = "PrepThrow_L";
    public string PrepThrow_R = "PrepThrow_R";


    private void Awake()
    {
        pickCtrl = FindObjectOfType<Pickaxe_Controller>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {

    }

    public void ChangeAnimationState(string newState)
    {
        //Stop the animation from interrupting itself.
        if (currentState == newState) return;

        //Play the animation.
        anim.Play(newState);

        //Reassign the current state.
        newState = currentState;
    }

    public void ResetPickSwing()
    {
        pickCtrl.isSwingingPick = false;
    }
}
