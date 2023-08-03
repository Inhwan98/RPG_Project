using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }


    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
