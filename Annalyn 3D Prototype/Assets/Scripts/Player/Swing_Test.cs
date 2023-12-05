using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing_Test : MonoBehaviour
{
    public GameObject cube;
    public Transform swingObject, swingPivot;
    [SerializeField] Vector3 swingOffset;
    [SerializeField] float swingSpeed, swingAngle;
    [SerializeField] bool isSwingingOnObject;

    // Start is called before the first frame update
    void Start()
    {
        isSwingingOnObject = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSwingingOnObject = !isSwingingOnObject;
        }

        if (isSwingingOnObject)
        {
            swingPivot.position = swingObject.position;
            cube.transform.parent = swingPivot;
            cube.transform.localPosition = swingOffset;

            float angle = swingAngle * Mathf.Sin(Time.time * swingSpeed);
            swingPivot.localRotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            swingPivot.position = Vector3.zero;
            swingPivot.localRotation = Quaternion.Euler(Vector3.zero);
            cube.transform.localPosition = Vector3.zero;
            //cube.transform.localRotation  = Quaternion.Euler(Vector3.zero);
            cube.transform.parent = null;
        }
    }
}
