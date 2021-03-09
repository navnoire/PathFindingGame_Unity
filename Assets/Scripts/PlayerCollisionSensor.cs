using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCollisionSensor : MonoBehaviour
{
    public static event Action CollidedWithTail;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Carriage")
        {
            CollidedWithTail?.Invoke();
        }
    }
}
