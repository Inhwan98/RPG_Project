#pragma warning disable IDE0051 //함수나 변수를 정의하고 다른 코드에서 사용하지 않았을 때 Visual Studio 또는 VSCode IDE에서 경고를 표시한다.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //Input System 사용하기 위함

public class PlayerCtr : MonoBehaviour
{
    public static PlayerCtr instance = null;

    private Rigidbody rigid;
    private Animator anim;

    private readonly int hashStrafe = Animator.StringToHash("Strafe");
    private readonly int hashForward = Animator.StringToHash("Forward");
    private readonly int hashRun = Animator.StringToHash("Run");
    private readonly int hashAttack = Animator.StringToHash("DoAttack");
    private readonly int hashMoveSpeed= Animator.StringToHash("MoveSpeed");

    [SerializeField] private float moveSpeed = 5.0f;

    Camera cam;
    float xInput;
    float zInput;
    float runInput;
    bool attackInput;

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
    private void FixedUpdate()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");
        runInput = Input.GetAxis("Run");
        attackInput = Input.GetButtonDown("Fire1");


        Vector3 direction = new Vector3(xInput, 0, zInput);

        if (attackInput) anim.SetTrigger(hashAttack);

        anim.SetFloat(hashStrafe, direction.x);
        anim.SetFloat(hashForward, direction.z);
        anim.SetFloat(hashRun, runInput);

        float movementSpeed = moveSpeed;
        if (runInput < 1.0f) movementSpeed *= 0.5f;
        Vector3 movement = direction.normalized * movementSpeed * Time.fixedDeltaTime;

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

    private void Update()
    {
        

    }

    

  


}
