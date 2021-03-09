using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{

    public float referenceRatio = 18f / 9f;
    float screenRatio;

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        screenRatio = cam.aspect;
        print("Screen Ration = " + screenRatio);

        if (Mathf.Abs(2 - screenRatio) > .1f)
        {
            cam.orthographicSize = referenceRatio / screenRatio * 3;
        }


    }
}
