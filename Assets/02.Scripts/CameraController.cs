using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    PlayerController playerCtr;
    Transform playertr;

    [SerializeField] private float distance;
    [SerializeField] private float height;
    [SerializeField] private float damping;
   
    [SerializeField] private Vector3 yOffset;

    [SerializeField] private float yRotSpeed;

    private Vector3 offset;

    private float angle;

    private bool _isUseWindow;

    Vector3 velocity;

    void Start()
    {
        playerCtr = PlayerController.instance;
        playertr = playerCtr.transform;

        playerCtr.SetCameraCtr(this);

        StartCoroutine(CheckCam());
    }


    private void FixedUpdate()
    {
        RotateCam();
    }

    /// <summary> 플레이어의 위치,회전에 비례하여 카메라 위치 선정 </summary>
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

            //yield return new WaitUntil(() => !_isUseInven); //인벤토리가 사용중이면 회전 대기
        }
    }

    /// <summary> 카메라의 회전 </summary>
    private void RotateCam()
    {
        if (_isUseWindow) return;

        float yRot = Input.GetAxis("Mouse X");

        angle += yRot * yRotSpeed * Time.fixedDeltaTime;
    }

    /// <summary> 플레이어 Inven의 상태 반영 </summary>
    public void UseWindow(bool value)
    {
        _isUseWindow = value;
    }

    
}

