using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using InHwan.CircularQueue;

public class PlayerController : Character
{
    public static PlayerController instance = null;
    //플레이가 죽었을 때, 모든 몬스터 제어
    public delegate void PlayerDiehandler();             //델리게이트 선언
    public static event PlayerDiehandler OnPlayerDie;    //이벤트 선언

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

    #region GetComponent 초기화 항목
    private Rigidbody rigid;
    private Weapon weaponCtr;
    private ItemInventory _inven;
    #endregion

    private CameraController _cameraCtr;

    private float xInput;
    private float zInput;
    private float runInput;
    private bool attackInput;

    private bool _isUseInven;
    private bool _isUseSkillWindow;

    //몬스터를 자동 추적할 몬스터 리스트
    private List<KeyValuePair<int, Transform>> monsterObjs_list = new List<KeyValuePair<int, Transform>>();
    //플레이어 UI (HPbar) Manager
    [SerializeField]
    private PlayerUIManager playerUICtr;

    protected override void Awake()
    {
        base.Awake();

        rigid       = GetComponent<Rigidbody>();
        weaponCtr   = GetComponentInChildren<Weapon>();
        _inven      = GetComponent<ItemInventory>();
        _skillMgr   = GetComponent<SkillManager>();
        _skInvenUI  = _skillMgr.GetSkInvenUI(); //skillMgr은 Awake() 전에 skinvenUI를 할당받음

        _inven.SetPlayerCtr(this);
        _skillMgr.SetPlayerCtr(this);
        _skillMgr.SetSkillPower(m_nCurSTR);

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

        PlayerUI_Init();
        playerUICtr.DisplayInfo(m_nLevel, m_nMaxHP, m_nMaxMP, m_nCurSTR);

        base.Start(); //skill_List 가 부모에서 초기화 됌

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

    /// <summary> Player의 기본/스킬 공격 구성 </summary>
    private void PlayerAttack()
    {
        if (m_bisAttack || _isUseInven || _isUseSkillWindow) return;

        if (attackInput) StartCoroutine(Attack());
        else Skill_Attack();
    }

    /// <summary> Player의 스킬 공격 </summary>
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
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(SkillAttack(4));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(SkillAttack(5));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartCoroutine(SkillAttack(6));
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
        playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP);
        playerUICtr.SetMPbar(m_nCurMP, m_nMaxMP);
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

        playerUICtr.DisplayInfo(m_nLevel, m_nMaxHP, m_nMaxMP, m_nCurSTR);
    }

    protected override IEnumerator Attack()
    {
        if(m_fAttackRunTime > m_fAttackDelay && !m_bisAttack)
        {
            weaponCtr.Use();
            //기본공격의 시간이 공격초기화 시간보다 길어진다면 콤보 x
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

    /// <summary> Player의 스킬 공격 
    /// <para/> SKill Manager의 플레이어 스킬Index 를 지닌
    /// <para/> skill_Datas[] 배열 이용
    /// </summary>
    protected override IEnumerator SkillAttack(int skillNum)
    {
        //현재 사용할 스킬. 0번째 부터 시작함.
        int skill_Idx = skillNum - 1;
        SkillData curSkill = skill_Datas[skill_Idx];

        //획득하지 않은 상태면
        if (curSkill == null) yield break;

        //사용 가능 상태가 아니면
        if (curSkill.GetInUse())
        {
            //빨간색 이미지가 깜빡거림
            StartCoroutine(_skInvenUI.SkillUsedWarring(skill_Idx));
            yield break;
        }

        //보유 마나보다 스킬 마나가 크다면 공격 중단.
        //if (m_fCurMP < curSkill.GetSkillManaAmount()) yield break; 공격 중단 없음

        m_bisAttack = true;

        anim.SetTrigger(curSkill.GetAnimHash());
        
        _skillMgr.UseSkill(curSkill, this, ref m_nCurMP);
        playerUICtr.SetMPbar(m_nCurMP, m_nMaxMP);

        StartCoroutine(_skInvenUI.StartSkillCoolTime(skill_Idx, curSkill));
        

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        m_bisAttack = false;
        //circualrQueue.DeQueue(); //스킬 순서 당겨 주기
    }

    public override void Buff(int _str)
    {
        base.Buff(_str);
        playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP); //HP Bar UI 수정
    }

    public override void OnDamage(int _str)
    {
        this.m_nCurHP -= _str;
        //Debug.Log($"{this.name} 가 {_str} 대미지를 입었다. 현재 체력 {m_nCurHP}");

        //if (m_fCurHP <= 0) this.objState = ObjectState.DEAD; //사망 없음

        if (m_nCurHP <= 0) m_nCurHP = 0;
        playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP); //HP Bar UI 수정
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

    /// <summary> 체력 회복 작용 </summary>
    public void RecoveryHP(int value)
    {
        m_nCurHP += value;

        if (m_nCurHP > m_nMaxHP) m_nCurHP = m_nMaxHP;
        playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP);
    }

    /// <summary> 마나 회복 작용 </summary>
    public void RecoveryMP(int value)
    {
        m_nCurMP += value;

        if (m_nCurMP > m_nMaxHP) m_nCurMP = m_nMaxMP;
        playerUICtr.SetMPbar(m_nCurMP, m_nMaxHP);
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

    /// <summary> 몬스터의 아이템을 Inventory로 전달 </summary>
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
        m_nMaxHP = objData.GetMaxHP();
        m_nCurHP = objData.GetCurHP();

        m_nMaxMP = objData.GetMaxMP();
        m_nCurMP = objData.GetCurMP();

        m_nCurSTR = objData.GetCurSTR();
        m_nCurExp = objData.GetCurExp();
    }

    /// <summary> 모든 스킬 덮어 씌우기 </summary>
    public void SetPlayerSkill(SkillData[] skill_datas)
    {
        skill_Datas = skill_datas;
    }

    /// <summary> 플레이어 스킬 데이터에 할당 </summary>
    public void SetPlayerSkill(int idx, SkillData skillData)
    {
        skill_Datas[idx] = skillData;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            NPC npcCtr = coll.gameObject?.GetComponent<NPC>();
            Debug.Assert(npcCtr != null, "npcCtr is NULL");
            int id = npcCtr.GetID();

            StartCoroutine(PlayDialog(id));
        }
    }

    private IEnumerator PlayDialog(int id)
    {
        var dialogSys = GameManager.instance.GetDialogSystem();
        dialogSys.SetDialogData(id);

        yield return new WaitUntil(() => dialogSys.UpdateDialog());
    }

}
