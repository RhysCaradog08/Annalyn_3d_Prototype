using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactivate_Element_Activation : MonoBehaviour
{
    Animator interactAnim;

    public GameObject objectToActivate;

    public bool isActivated;

    // Start is called before the first frame update
    void Start()
    {
        interactAnim = GetComponent<Animator>();

        isActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            interactAnim.SetTrigger("Pressed");

            Animator objectAnim = objectToActivate.GetComponent<Animator>();
            objectAnim.SetBool("Activated", true);
        }
    }
}
