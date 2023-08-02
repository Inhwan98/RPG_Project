using UnityEngine;

public class CameraController : MonoBehaviour
{
    PlayerController playerCtr;
    Transform playertr;

    [SerializeField] private float distance;
    [SerializeField] private float height;
    [SerializeField] private float damping;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 yOffset;

    [SerializeField] private float yRotSpeed;

    float angle;

    Vector3 velocity;

    void Start()
    {
        playerCtr = PlayerController.instance;
        playertr = playerCtr.transform;
    }

    void Update()
    {
        


        

    }

    void LateUpdate()
    {

        float yRot = Input.GetAxis("Mouse X");

        angle += yRot * yRotSpeed * Time.deltaTime;
        
        Vector3 offset = playertr.position + Quaternion.Inverse(playertr.rotation) * Quaternion.AngleAxis(angle, Vector3.up) * ((-playertr.forward * distance) + (playertr.up * height));

        //Vector3 pos = (playertr.position +
        //              (-playertr.forward * distance) +
        //              (playertr.up * height));



        transform.position = Vector3.SmoothDamp(transform.position,
                                                offset,
                                                ref velocity,
                                                damping);
            //playertr.position + offset;
        //transform.LookAt(playertr.position);

        transform.LookAt(playertr.position + yOffset);
    }
}

