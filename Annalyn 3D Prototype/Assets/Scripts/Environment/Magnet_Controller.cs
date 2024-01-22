using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet_Controller : MonoBehaviour
{
    [SerializeField] Rigidbody lodestoneRB;
    [SerializeField] float magnetStrength, distance;
    public bool isOn, hasPlayer;

    private void Start()
    {
        hasPlayer = false;
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOn && other.CompareTag("Lodestone"))
        {
            if (other.GetComponent<Rigidbody>() != null)
            {
                lodestoneRB = other.GetComponent<Rigidbody>();
            }

            if (lodestoneRB.isKinematic)
            {
                lodestoneRB.isKinematic = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isOn && lodestoneRB !=  null)
        {
            Debug.Log("Lodestone in Magnet");           

            distance = Vector3.Distance(other.transform.position, transform.position);

            Vector3 magnetDirection = transform.position - lodestoneRB.transform.position;

            if (distance > 0)
            {
                if (!lodestoneRB.transform.parent)
                {
                    lodestoneRB.AddForce(magnetDirection * magnetStrength, ForceMode.Force);
                }
            }
        }

        if (!isOn && lodestoneRB != null)
        {
            lodestoneRB = null;
        }

        if (isOn && !lodestoneRB)
        {
            if (other.CompareTag("Lodestone"))
            {
                lodestoneRB = other.GetComponent<Rigidbody>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(lodestoneRB != null)
        {
            lodestoneRB = null;
        }
    }
}
