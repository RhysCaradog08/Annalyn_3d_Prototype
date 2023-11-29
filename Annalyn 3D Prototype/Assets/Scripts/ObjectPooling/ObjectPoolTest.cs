using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Basics.ObjectPool;

public class ObjectPoolTest : MonoBehaviour
{
    GameObject one;
    GameObject two;
    GameObject three;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            one = ObjectPoolManager.instance.CallObject("Number_1", null, Vector3.zero, Quaternion.Euler(0, 180, 0), 1);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            two = ObjectPoolManager.instance.CallObject("Number_2", null, Vector3.zero, Quaternion.Euler(0, 180, 0), 1);
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            three = ObjectPoolManager.instance.CallObject("Number_3", null, Vector3.zero, Quaternion.Euler(0, 180, 0), 1);
        }
    }
}
