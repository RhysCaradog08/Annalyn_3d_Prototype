using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible_Health : MonoBehaviour
{
    public int health;

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
