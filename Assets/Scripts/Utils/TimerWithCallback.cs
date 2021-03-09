using System;
using System.Collections;
using UnityEngine;

public class TimerWithCallback
{


    public static IEnumerator Start(float duration, Action callback)
    {
        return Start(duration, false, callback);
    }

    public static IEnumerator Start(float duration, int repCount, Action callback)
    {
        do
        {
            yield return new WaitForSeconds(duration);

            if (callback != null)
            {
                callback();
                repCount--;
            }


        } while (repCount > 0);
    }

    public static IEnumerator Start(float duration, bool repeat, Action callback)
    {
        do
        {
            yield return new WaitForSeconds(duration);

            if (callback != null)
                callback();

        } while (repeat);
    }

    public static IEnumerator StartRealtime(float time, System.Action callback)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }

        if (callback != null) callback();
    }

    public static IEnumerator NextFrame(Action callback)
    {
        yield return new WaitForEndOfFrame();

        if (callback != null)
            callback();
    }
}