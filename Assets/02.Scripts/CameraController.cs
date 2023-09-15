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

    /// <summary> �÷��̾��� ��ġ,ȸ���� ����Ͽ� ī�޶� ��ġ ���� </summary>
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

            //yield return new WaitUntil(() => !_isUseInven); //�κ��丮�� ������̸� ȸ�� ���
        }
    }

    /// <summary> ī�޶��� ȸ�� </summary>
    private void RotateCam()
    {
        if (_isUseWindow) return;

        float yRot = Input.GetAxis("Mouse X");

        angle += yRot * yRotSpeed * Time.fixedDeltaTime;
    }

    /// <summary> �÷��̾� Inven�� ���� �ݿ� </summary>
    public void UseWindow(bool value)
    {
        _isUseWindow = value;
    }

    
}

