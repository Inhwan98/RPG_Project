using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using InHwan.CircularQueue;

public class PlayerController : Character
{
    public static PlayerController instance = null;
    //�÷��̰� �׾��� ��, ��� ���� ����
    public delegate void PlayerDiehandler();             //��������Ʈ ����
    public static event PlayerDiehandler OnPlayerDie;    //�̺�Ʈ ����

    #region Anim Hashing Init
    private readonly int hashStrafe      = Animator.StringToHash("Strafe");
    private readonly int hashForward     = Animator.StringToHash("Forward");
    private readonly int hashRun         = Animator.StringToHash("Run");
    private readonly int hashMoveSpeed   = Animator.StringToHash("MoveSpeed");
    private readonly int hashAttackSpeed = Animator.StringToHash("AttackSpeed");
    private readonly int[] hashAttack = new int[] { Animator.StringToHash("DoAttack1"), Animator.StringToHash("DoAttack2"), Animator.StringToHash("DoAttack3")};
    #endregion

    [SerializeField] private float m_fAttackDelay = 0.5f;
    private float m_fAttackRunTime = 0.0f;
    private float m_fAttackInitTime = 1.5f;
    private int m_nAttackCount = 0;


    [SerializeField] private float moveSpeed = 5.0f;

    #region GetComponent �ʱ�ȭ �׸�
    private Rigidbody rigid;
    private Weapon weaponCtr;
    #endregion

    private Camera cam;
    private float xInput;
    private float zInput;
    private float runInput;
    private bool attackInput;

    //���͸� �ڵ� ������ ���� ����Ʈ
    private List<KeyValuePair<int, Transform>> monsterObjs_list = new List<KeyValuePair<int, Transform>>();
    //�÷��̾� UI (HPbar) Manager
    private PlayerUIManager playerUICtr;

    protected override void Awake()
    {
        base.Awake();

        rigid       = GetComponent<Rigidbody>();
        weaponCtr   = GetComponentInChildren<Weapon>();
        #region SingTone
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        #endregion
    }

    protected override void Start()
    {
        anim.SetFloat(hashAttackSpeed, m_fAttackDelay);

        cam           = Camera.main;
        playerUICtr   = PlayerUIManager.instance;

        PlayerUI_Init();
        playerUICtr.DisplayInfo(m_nLevel, m_fMaxHP, m_fMaxMP, m_fCurSTR, m_fAttackRange);

        base.Start(); //skill_List �� �θ𿡼� �ʱ�ȭ ��
        playerUICtr.UpdateSkill_Image(skill_List);
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(xInput, 0, zInput);

        anim.SetFloat(hashStrafe, direction.x);
        anim.SetFloat(hashForward, direction.z);
        anim.SetFloat(hashRun, runInput);

        float movementSpeed = moveSpeed;
        if (runInput < 1.0f) movementSpeed *= 0.5f;
        if (m_bisAttack) movementSpeed = 0.0f;

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
        Input_init();

        if(m_fAttackDelay > m_fAttackRunTime) m_fAttackRunTime += Time.deltaTime;

        //if (attackInput && !m_bisAttack) StartCoroutine(Attack());
        //Skill_Attack();

        PlayerAttack();
    }

    private void PlayerAttack()
    {
        if (m_bisAttack) return;

        if (attackInput) StartCoroutine(Attack());
        else Skill_Attack();
    }


    private void Skill_Attack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1");
            StartCoroutine(SkillAttack(1));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2");
            StartCoroutine(SkillAttack(2));
        }
    }

    private void PlayerUI_Init()
    {
        playerUICtr.SetHPbar(m_fCurHP, m_fMaxHP);
        playerUICtr.SetMPbar(m_fCurMP, m_fMaxMP);
        playerUICtr.SetEXPbar(m_nCurExp, m_nMaxExp);
    }

    // Use => FixedUpdate()
    private void Input_init()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");
        runInput = Input.GetAxis("Run");
        attackInput = Input.GetButton("Fire1");
    }

    public override void LevelUP()
    {
        base.LevelUP();
        PlayerUI_Init();

        m_fAttackRange = statusSetting.GetAttackRange();

        playerUICtr.DisplayInfo(m_nLevel, m_fMaxHP, m_fMaxMP, m_fCurSTR, m_fAttackRange);
    }

    protected override IEnumerator Attack()
    {
        if(m_fAttackRunTime > m_fAttackDelay && !m_bisAttack)
        {
            weaponCtr.Use();
            //�⺻������ �ð��� �����ʱ�ȭ �ð����� ������ٸ� �޺� x
            if (m_fAttackRunTime > m_fAttackInitTime) m_nAttackCount = 0;
            anim.SetTrigger(hashAttack[m_nAttackCount]);

            m_nAttackCount++;
            if (m_nAttackCount >= 3) m_nAttackCount = 0;

            m_bisAttack = true;
            m_fAttackRunTime = 0;
            yield return new WaitForSeconds(m_fAttackDelay);

            m_bisAttack = false;
        }
    }

    protected override IEnumerator SkillAttack(int skillNum)
    {
        //���� �����. ��ų 0��° ���� ������.
        int skill_Idx = skillNum - 1;
        SkillStatus curSkill = skill_List[skill_Idx];
        
        if (curSkill.GetInUse())
        {
            StartCoroutine(playerUICtr.SkillUsedWarring(skill_Idx));
            yield break;
        }

        //���� �������� ��ų ������ ũ�ٸ� ���� �ߴ�.
        //if (m_fCurMP < curSkill.GetSkillManaAmount()) yield break; ���� �ߴ� ����

        //Debug.Assert(destTr, "destTr is NULL !!!");

        m_bisAttack = true;

        anim.SetTrigger(curSkill.GetAnimHash());
        
        skillMgr.UseSkill(curSkill, this, ref m_fCurMP);
        playerUICtr.SetMPbar(m_fCurMP, m_fMaxMP);

        StartCoroutine(playerUICtr.StartSkillCoolTime(skill_Idx, curSkill.GetCoolDown(), curSkill));
        

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        m_bisAttack = false;
        //circualrQueue.DeQueue(); //��ų ���� ��� �ֱ�
    }

    public override void Buff(float _str)
    {
        base.Buff(_str);
        playerUICtr.SetHPbar(m_fCurHP, m_fMaxHP); //HP Bar UI ����
    }

    public override void OnDamage(float _str)
    {
        this.m_fCurHP -= _str;
        //Debug.Log($"{this.name} �� {_str} ������� �Ծ���. ���� ü�� {m_nCurHP}");

        //if (m_fCurHP <= 0) this.objState = ObjectState.DEAD; //��� ����

        if (m_fCurHP <= 0) m_fCurHP = 0;
        playerUICtr.SetHPbar(m_fCurHP, m_fMaxHP); //HP Bar UI ����
    }

    protected override void Die()
    {
        Debug.Log("Player Die");
        StopAllCoroutines();
        anim.SetTrigger(hashDead);
        m_bisDead = true;

        OnPlayerDie(); //delegate event Play
        GameManager.instance.PlayerDie();
    }

    public float GetSTR() { return m_fCurSTR;}
    public float GetHP()  { return m_fCurHP; }

    public void SetEXP(int _exp)
    {
        this.m_nCurExp += _exp;
        if(m_nCurExp >= m_nMaxExp)
        {
            LevelUP();
        }
        playerUICtr.SetEXPbar(m_nCurExp, m_nMaxExp);
    }
    
}
