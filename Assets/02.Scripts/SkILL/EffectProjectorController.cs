using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectProjectorController : MonoBehaviour
{
    [SerializeField]
    private float rotSpeed = 10.0f;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, Time.deltaTime * rotSpeed, 0) * transform.rotation;
    }

}
