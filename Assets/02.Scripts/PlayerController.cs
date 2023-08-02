#pragma warning disable IDE0051 //�Լ��� ������ �����ϰ� �ٸ� �ڵ忡�� ������� �ʾ��� �� Visual Studio �Ǵ� VSCode IDE���� ��� ǥ���Ѵ�.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //Input System ����ϱ� ����

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;

    private Rigidbody rigid;
    private Animator anim;

    private readonly int hashStrafe = Animator.StringToHash("Strafe");
    private readonly int hashForward = Animator.StringToHash("Forward");
    private readonly int hashRun = Animator.StringToHash("Run");
    private readonly int hashAttack = Animator.StringToHash("DoAttack");
    private readonly int hashMoveSpeed= Animator.StringToHash("MoveSpeed");

    [SerializeField] private float moveSpeed = 5.0f;

    Camera cam;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame

    private void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");
        float runInput = Input.GetAxis("Run");
        bool attackInput = Input.GetButton("Fire1");

        Vector3 direction = new Vector3(xInput, 0, zInput);

        if (attackInput) anim.SetTrigger(hashAttack);

        anim.SetFloat(hashStrafe, direction.x);
        anim.SetFloat(hashForward, direction.z);
        anim.SetFloat(hashRun, runInput);

        float movementSpeed = moveSpeed;
        if (runInput < 1.0f) movementSpeed *= 0.5f;
        Vector3 movement = direction.normalized * movementSpeed * Time.deltaTime;

        anim.SetFloat(hashMoveSpeed, movementSpeed);

        Quaternion destRot = cam.transform.localRotation;
        destRot.x = 0;
        destRot.z = 0;

        rigid.transform.rotation = destRot;
        //if (zInput >= 0.1)
        //{
        //    Quaternion destRot = cam.transform.localRotation;
        //    destRot.x = 0;
        //    destRot.z = 0;

        //    rigid.rotation = destRot;
        //}

        rigid.transform.Translate(movement);

    }

  


}
