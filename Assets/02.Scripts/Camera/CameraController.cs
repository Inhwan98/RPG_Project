using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /**************************************************
     *                      Fields
     **************************************************/

    #region static Fields
    public static CameraController instance = null;
    #endregion

    #region option Fields
    [SerializeField] private float distance;
    [SerializeField] private float height;
    [SerializeField] private float damping;
   
    [SerializeField] private Vector3 yOffset;
    [SerializeField] private float yRotSpeed;

    [SerializeField] PlayerController playerCtr;
    #endregion

    #region private Fields
    private Transform playertr;
    private Vector3 offset;
    private Vector3 velocity;
    private float xAngle;
    private bool _isUseWindow;
    #endregion

    /**************************************************
    *                  Unity Event
    **************************************************/
    #region Unity Event
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
    }

    void Start()
    {
        playerCtr = PlayerController.instance;
        playerCtr.SetCameraCtr(this);
        playertr = playerCtr.transform;
        StartCoroutine(CheckCam());
    }

    #endregion

    /**************************************************
    *                     Methods
    **************************************************/
    #region private Methods
    /// <summary> ī�޶��� ȸ�� </summary>
    private void RotateCam()
    {
        if (_isUseWindow) return;

        float yRot = Input.GetAxis("Mouse X");
        float xRot = Input.GetAxis("Mouse Y");

        xAngle += yRot * yRotSpeed * Time.fixedDeltaTime;
    }
    #endregion

    #region public Methods
    /// <summary> �÷��̾� Inven�� ���� �ݿ� </summary>
    public void UseWindow(bool value) => _isUseWindow = value;
    #endregion

    /**************************************************
    *                  Coroutine
    **************************************************/
    #region Coroutine
    /// <summary> �÷��̾��� ��ġ,ȸ���� ����Ͽ� ī�޶� ��ġ ���� </summary>
    private IEnumerator CheckCam()
    {
        
        while(true)
        {
            yield return new WaitForFixedUpdate();
            RotateCam();

            // 
            offset = playertr.position + (Quaternion.Inverse(playertr.rotation) * Quaternion.AngleAxis(xAngle, Vector3.up))
                      * ((-playertr.forward * distance) + (playertr.up * height));

            transform.position = Vector3.SmoothDamp(transform.position,
                                    offset,
                                    ref velocity,
                                    damping);

            transform.LookAt(playertr.position + yOffset);

            //yield return new WaitUntil(() => !_isUseInven); //�κ��丮�� ������̸� ȸ�� ���
        }
    }
    #endregion


}

