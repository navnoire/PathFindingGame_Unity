using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScaleTexture : MonoBehaviour
{
    public RenderTexture sourceTex;
    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        sourceTex.height = (int)(Screen.height * .8);
        sourceTex.width = (int)(Screen.width * .8);

    }
}
