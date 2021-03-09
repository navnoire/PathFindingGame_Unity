using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Transgressor : MonoBehaviour
{
    public int GoToRoomIndex;
    public float targetPlayerDirection;
    public Vector3 transgressTarget;


    public delegate void ReloadHandler(int index);
    public static event ReloadHandler ReloadEvent;

    public static event Action<Vector3, float> TransgressAction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TransgressAction != null)
            {
                print("Transgressing to " + transgressTarget);
                TransgressAction(transgressTarget, targetPlayerDirection);
            }

            if (ReloadEvent != null)
            {
                ReloadEvent(GoToRoomIndex);
            }





        }
    }
}
