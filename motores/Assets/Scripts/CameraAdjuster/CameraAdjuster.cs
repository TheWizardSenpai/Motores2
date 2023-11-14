using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
//[RequireComponent(typeof(Camera),typeof(Rigidbody))]
[RequireComponent(typeof(Camera))]
public class CameraAdjuster : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float levelSize = 10;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSize();
    }

    private void ChangeSize()
    {
        //Landscape Game
        float widthDiff = levelSize / Screen.width;

        float cameraSize = widthDiff * Screen.height * 0.5f;

        if (cam.orthographic) cam.orthographicSize = cameraSize;
        else cam.fieldOfView = cameraSize;

        //Portrait Game
        /*float heightDiff = levelSize / Screen.height;

        float cameraSize2 = heightDiff * Screen.width * 0.5f;

        if (cam.orthographic) cam.orthographicSize = cameraSize2;
        else cam.fieldOfView = cameraSize2;*/

    }
}
