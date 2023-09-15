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
    private Inventory _inven;
    #endregion

    private CameraController _cameraCtr;

    private float xInput;
    private float zInput;
    private float runInput;
    private bool attackInput;

    private bool _isUseInven;
    private bool _isUseSkillWindow;

    //���͸� �ڵ� ������ ���� ����Ʈ
    private List<KeyValuePair<int, Transform>> monsterObjs_list = new List<KeyValuePair<int, Transform>>();
    //�÷��̾� UI (HPbar) Manager
    private PlayerUIManager playerUICtr;

    protected override void Awake()
    {
        base.Awake();

        rigid       = GetComponent<Rigidbody>();
        weaponCtr   = GetComponentInChildren<Weapon>();
        _inven      = GetComponent<Inventory>();
        _skillMgr   = GetComponent<SkillManager>();

        _inven.SetPlayerCtr(this);
        _skillMgr.SetPlayerCtr(this);

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

        //_cam           = Camera.main;
        playerUICtr   = PlayerUIManager.instance;

        PlayerUI_Init();
        playerUICtr.DisplayInfo(m_nLevel, m_fMaxHP, m_fMaxMP, m_fCurSTR);

        base.Start(); //skill_List �� �θ𿡼� �ʱ�ȭ ��
        playerUICtr.UpdateSkill_Image(skill_Datas);
    }

    private void FixedUpdate()
    {
        CharacterMovement();

    }

    private void Update()
    {
        Input_init();

        if(m_fAttackDelay > m_fAttackRunTime) m_fAttackRunTime += Time.deltaTime;

        //if (attackInput && !m_bisAttack) StartCoroutine(Attack());
        //Skill_Attack();
        StatusWindowActiveButton();
        PlayerAttack();
    }

    /// <summary> Character dir, move, speed, rot, anim
    /// <para/> Current Use FixedUpdate()
    /// </summary>
    private void CharacterMovement()
    {
        if (_isUseInven || _isUseSkillWindow || m_bisAttack) return;

        Vector3 direction = new Vector3(xInput, 0, zInput);

        anim.SetFloat(hashStrafe, direction.x);
        anim.SetFloat(hashForward, direction.z);
        anim.SetFloat(hashRun, runInput);

        float movementSpeed = moveSpeed;
        if (runInput < 1.0f) movementSpeed *= 0.5f;

        Vector3 movement = direction.normalized * movementSpeed * Time.fixedDeltaTime;

        anim.SetFloat(hashMoveSpeed, movementSpeed);

        Quaternion destRot = _cameraCtr.transform.localRotation;
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

    /// <summary> Player�� �⺻/��ų ���� ���� </summary>
    private void PlayerAttack()
    {
        if (m_bisAttack || _isUseInven || _isUseSkillWindow) return;

        if (attackInput) StartCoroutine(Attack());
        else Skill_Attack();
    }

    /// <summary> Player�� ��ų ���� </summary>
    private void Skill_Attack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(SkillAttack(1));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(SkillAttack(2));
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(SkillAttack(3));
        }
    }

    /// <summary> Invnetory Active</summary>
    public void StatusWindowActiveButton()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            _isUseInven = !_isUseInven;

            _inven.InventoryActive(_isUseInven);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            _isUseSkillWindow = !_isUseSkillWindow;

            _skillMgr.SkillWindowActive(_isUseSkillWindow);
        }
    }


    public CameraController GetCameraCtr() => _cameraCtr;

    /// <summary> Player Info UI Set </summary>
    private void PlayerUI_Init()
    {
        playerUICtr.SetHPbar(m_fCurHP, m_fMaxHP);
        playerUICtr.SetMPbar(m_fCurMP, m_fMaxMP);
        playerUICtr.SetEXPbar(m_nCurExp, m_nMaxExp);
    }

    private void Input_init()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");
        runInput = Input.GetAxis("Run");
        attackInput = Input.GetButton("Fire1");
    }

    /// <summary> Character LevelUp System </summary>
    public override void LevelUP()
    {
        base.LevelUP();
        PlayerUI_Init();

        playerUICtr.DisplayInfo(m_nLevel, m_fMaxHP, m_fMaxMP, m_fCurSTR);
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
        //���� ����� ��ų. 0��° ���� ������.
        int skill_Idx = skillNum - 1;
        SkillData curSkill = skill_Datas[skill_Idx];
        
        //��� ���� ���°� �ƴϸ�
        if (curSkill.GetInUse())
        {
            //������ �̹����� �����Ÿ�
            StartCoroutine(playerUICtr.SkillUsedWarring(skill_Idx));
            yield break;
        }

        //���� �������� ��ų ������ ũ�ٸ� ���� �ߴ�.
        //if (m_fCurMP < curSkill.GetSkillManaAmount()) yield break; ���� �ߴ� ����

        m_bisAttack = true;

        anim.SetTrigger(curSkill.GetAnimHash());
        
        _skillMgr.UseSkill(curSkill, this, ref m_fCurMP);
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

    public bool  GetUseInven() { return _isUseInven; }
    public void  SetUseInven(bool value) { _isUseInven = value; }

    /// <summary> ü�� ȸ�� �ۿ� </summary>
    public void RecoveryHP(float value)
    {
        m_fCurHP += value;

        if (m_fCurHP > m_fMaxHP) m_fCurHP = m_fMaxHP;
        playerUICtr.SetHPbar(m_fCurHP, m_fMaxHP);
    }

    /// <summary> ���� ȸ�� �ۿ� </summary>
    public void RecoveryMP(float value)
    {
        m_fCurMP += value;

        if (m_fCurMP > m_fMaxHP) m_fCurMP = m_fMaxMP;
        playerUICtr.SetMPbar(m_fCurHP, m_fMaxHP);
    }

    public void SetEXP(int _exp)
    {
        this.m_nCurExp += _exp;
        if(m_nCurExp >= m_nMaxExp)
        {
            LevelUP();
        }
        playerUICtr.SetEXPbar(m_nCurExp, m_nMaxExp);
    }

    public void AddInven(Dictionary<ItemData, int> _itemDic)
    {
        foreach(var itemDic in _itemDic)
        {
            _inven.Add(itemDic.Key, itemDic.Value);
        }

    }

    public void SetCameraCtr(CameraController value) => _cameraCtr = value;

    [ContextMenu("SavePlayer")]
    public void SavePlayer()
    {
        SaveSys.SavePlayer(this);
    }

    [ContextMenu("SaveSkillSet")]
    public void SaveSkillSet()
    {
        SaveSys.SavePlayerSkillSet(skill_Datas);
    }

    [ContextMenu("LoadPlaeyr")]
    protected override void LoadData()
    {
        objData = SaveSys.LoadObject("PlayerData.Json");

        if(objData == null)
        {
            Debug.LogError("Player Data is NULL!!");
            return;
        }

        m_nLevel = objData.GetLevel();
        m_fMaxHP = objData.GetMaxHP();
        m_fCurHP = objData.GetCurHP();

        m_fMaxMP = objData.GetMaxMP();
        m_fCurMP = objData.GetCurMP();

        m_fCurSTR = objData.GetCurSTR();
        m_nCurExp = objData.GetCurExp();
    }
}
