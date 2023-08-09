using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    PlayerCtr playerCtr;
    Transform playertr;

    [SerializeField] private float distance;
    [SerializeField] private float height;
    [SerializeField] private float damping;
   
    [SerializeField] private Vector3 yOffset;

    [SerializeField] private float yRotSpeed;

    private Vector3 offset;

    private float angle;

    Vector3 velocity;

    void Start()
    {
        playerCtr = PlayerCtr.instance;
        playertr = playerCtr.transform;

        StartCoroutine(CheckCam());
    }


    private void FixedUpdate()
    {
        float yRot = Input.GetAxis("Mouse X");

        angle += yRot * yRotSpeed * Time.fixedDeltaTime;
    }


    IEnumerator CheckCam()
    {
        while(true)
        {
            
            yield return new WaitForFixedUpdate();

            // 
            offset = playertr.position + (Quaternion.Inverse(playertr.rotation) * Quaternion.AngleAxis(angle, Vector3.up))
                      * ((-playertr.forward * distance) + (playertr.up * height));

            transform.position = Vector3.SmoothDamp(transform.position,
                                    offset,
                                    ref velocity,
                                    damping);

            transform.LookAt(playertr.position + yOffset);
        }
    }

    
}

