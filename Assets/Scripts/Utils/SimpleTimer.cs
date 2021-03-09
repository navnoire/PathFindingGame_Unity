using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTimer : MonoBehaviour
{
    bool timeReached;
    public float waitInterval = 10f;
    public bool isRunning;

    public void StartTimer()
    {
        isRunning = true;

        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        //print("Timer Starts");
        yield return new WaitForSeconds(waitInterval);
        timeReached = true;
        yield return null;
    }

    public bool CheckTimer()
    {
        //print("Checking Timer is reached = " + timeReached);
        return timeReached;
    }

    public void ResetTimer()
    {
        StopCoroutine(TimerRoutine());
        timeReached = false;
        isRunning = false;
    }

}

