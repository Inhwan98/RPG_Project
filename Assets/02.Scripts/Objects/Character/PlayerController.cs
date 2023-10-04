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

    [SerializeField] private GameManager _gameMgr;

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
    private Rigidbody _rigid;
    private Weapon _weaponCtr;
    #endregion

    private CameraController _cameraCtr;

    private PlayerLevelData[] playerLevelData;

    private float xInput;
    private float zInput;
    private float runInput;
    private bool attackInput;

    private bool _isUseInven;
    private bool _isUseSkillWindow;
    private bool _isPlayDialog;

    //���͸� �ڵ� ������ ���� ����Ʈ
    private List<KeyValuePair<int, Transform>> monsterObjs_list = new List<KeyValuePair<int, Transform>>();
    //�÷��̾� UI (HPbar) Manager

    [SerializeField]
    private PlayerUIManager _playerUICtr;

    private List<QuestData> _currentQuestList = new List<QuestData>();
   
    private QuestSystem _questSystem;

    private ItemInventoryManager _itemInvenMgr;

    private PlayerData _playerData;


    public bool GetUseInven() => _isUseInven;
    public CameraController GetCameraCtr() => _cameraCtr;
    public List<QuestData> GetPlayerQuestList() => _currentQuestList;

    public void SetUseItemInven(bool value) { _isUseInven = value; }                                    // �κ��丮â�� ���� �ִ°�
    public void SetUseSKill_Inven(bool value) { _isUseSkillWindow = value; }                            // ��ųâ�� �����ִ°�
    public void SetUsePlayDialog(bool value) { _isPlayDialog = value; }                                 // ���̾�α� ������ΰ�  
    public void SetQuestSystem(QuestSystem questSystem) => _questSystem = questSystem;                  // ����Ʈ �ý���
    public void SetGameManager(GameManager gameMgr) => _gameMgr = gameMgr;                              // ���� �Ŵ���
    public void SetItemInvenManager(ItemInventoryManager itemInvenMgr) => _itemInvenMgr = itemInvenMgr; // ������ �κ��丮 �Ŵ���
    public void SetSkillMgr(SkillManager skillMgr) => _skillMgr = skillMgr;                             // ��ų �Ŵ���
    public void SetCameraCtr(CameraController value) => _cameraCtr = value;                             // ī�޶�

    public void AddPlayerQuest(QuestData questData) => _currentQuestList.Add(questData);

    /// <summary> �÷��̾� ������ ����ؼ� ����Ʈ ��ϵ� ���� </summary>
    private void UpdateQuest() => _gameMgr.UpdateQuestList(m_nLevel);


    protected override void Awake()
    {
        #region SingTone
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        //DontDestroyOnLoad(this.gameObject);
        #endregion

        base.Awake();

        PlayerController_Init();
        
    }

    /// <summary> �÷��̾� �ʱ�ȭ </summary>
    private void PlayerController_Init()
    {
        #region GetComponent
        _rigid = GetComponent<Rigidbody>();
        _weaponCtr = GetComponentInChildren<Weapon>();
        #endregion

        _anim.SetFloat(hashAttackSpeed, m_fAttackDelay);

        //UI
        _playerUICtr = FindObjectOfType<PlayerUIManager>();
        _playerUICtr.SetPlayerCtrReference(this);

        //���ݼӵ�
        _anim.SetFloat(hashAttackSpeed, m_fAttackDelay);

        #region Manager��ũ��Ʈ�� GameManager���� �ʱ�ȭ ��Ŵ
        //_skillMgr = GetComponent<SkillManager>();
        //_inven = GetComponent<ItemInventoryManager>();
        //_inven.SetPlayerCtr(this);
        //_skillMgr.SetPlayerCtr(this);
        //_skillMgr.SetSkillPower(m_nCurSTR);
        //_skInvenUI = _skillMgr.GetSkInvenUI(); //skillMgr�� Awake() ���� skinvenUI�� �Ҵ����
        #endregion

    }

    protected override void Start()
    {
        //_cam           = Camera.main;

        UpdateQuest(); //����Ʈ ���� ������Ʈ
        CurrentQuestInit(_currentQuestList);

        PlayerUI_Init();
       
        base.Start(); //skill_List �� �θ𿡼� �ʱ�ȭ ��
    }


    /// <summary>
    /// ����Ʈ ������ ������Ʈ ���ش�.
    /// ���� ����, ������ �Ҷ����� ȣ�� ����� �Ѵ�.
    /// </summary>
  
    private void FixedUpdate()
    {
        CharacterMovement();

    }

    private void Update()
    {
        Input_init();

        if(m_fAttackInitTime > m_fAttackRunTime) m_fAttackRunTime += Time.deltaTime;

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
        if (_isUseInven || _isUseSkillWindow || _isPlayDialog || m_bisAttack) return;

        Vector3 direction = new Vector3(xInput, 0, zInput);

        _anim.SetFloat(hashStrafe, direction.x);
        _anim.SetFloat(hashForward, direction.z);
        _anim.SetFloat(hashRun, runInput);

        float movementSpeed = moveSpeed;
        if (runInput < 1.0f) movementSpeed *= 0.5f;

        Vector3 movement = direction.normalized * movementSpeed * Time.fixedDeltaTime;

        _anim.SetFloat(hashMoveSpeed, movementSpeed);


        if (movement != Vector3.zero)
        {
            Quaternion destRot = _cameraCtr.transform.localRotation;

            destRot.x = 0;
            destRot.z = 0;

            _rigid.transform.rotation = destRot;
        }
        //Quaternion destRot = _cameraCtr.transform.localRotation;
        //destRot.x = 0;
        //destRot.z = 0;

        //_rigid.transform.rotation = destRot;
        _rigid.transform.Translate(movement);
    }

    /// <summary> Player�� �⺻/��ų ���� ���� </summary>
    private void PlayerAttack()
    {
        if (m_bisAttack || _isUseInven || _isUseSkillWindow || _isPlayDialog) return;

        if (attackInput) StartCoroutine(Attack());
        else Skill_Attack();
    }

    /// <summary> Player�� ��ų ���� </summary>
    private void Skill_Attack()
    {
        #region Input Alpha 1~6
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
        #endregion
    }

    protected override IEnumerator Attack()
    {
        if (m_fAttackRunTime > m_fAttackDelay && !m_bisAttack)
        {
            _weaponCtr.Use();
            //�⺻������ �ð��� �����ʱ�ȭ �ð����� ������ٸ� �޺� x
            if (m_fAttackRunTime > m_fAttackInitTime) m_nAttackCount = 0;
            _anim.SetTrigger(hashAttack[m_nAttackCount]);

            m_nAttackCount++;
            if (m_nAttackCount >= 3) m_nAttackCount = 0;

            m_bisAttack = true;
            m_fAttackRunTime = 0;
            //������ ���� ��, �� ��� ���
            if(m_nAttackCount == 0)
                yield return new WaitForSeconds(m_fAttackDelay + 1.0f);
            else
                yield return new WaitForSeconds(m_fAttackDelay);

            m_bisAttack = false;
        }
    }

    public IEnumerator AttackTimer()
    {
        
        return null;
    }

    /// <summary> Player�� ��ų ���� 
    /// <para/> SKill Manager�� �÷��̾� ��ųIndex �� ����
    /// <para/> skill_Datas[] �迭 �̿�
    /// </summary>
    protected override IEnumerator SkillAttack(int skillNum)
    {
        //���� ����� ��ų. 0��° ���� ������.
        int skill_Idx = skillNum - 1;
        SkillData curSkill = _skill_Datas[skill_Idx];

        //ȹ������ ���� ���¸�
        if (curSkill == null) yield break;

        //��� ���� ���°� �ƴϸ�
        if (curSkill.GetInUse())
        {
            //������ �̹����� �����Ÿ�
            _skillMgr.SkillUsedWarring(skill_Idx);
            //StartCoroutine(_skInvenUI.SkillUsedWarring(skill_Idx));
            yield break;
        }

        //���� �������� ��ų ������ ũ�ٸ� ���� �ߴ�.
        //if (m_fCurMP < curSkill.GetSkillManaAmount()) yield break; ���� �ߴ� ����

        m_bisAttack = true;

        _anim.SetTrigger(curSkill.GetAnimHash());

        _skillMgr.UseSkill(curSkill, this, ref m_nCurMP);
        _playerUICtr.SetMPbar(m_nCurMP, m_nMaxMP);

        _skillMgr.StartSkillCoolTime(skill_Idx, curSkill);

        yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        m_bisAttack = false;
        //circualrQueue.DeQueue(); //��ų ���� ��� �ֱ�
    }


    /// <summary> Invnetory Active</summary>
    public void StatusWindowActiveButton()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            _isUseInven = !_isUseInven;

            _itemInvenMgr.SetInventoryActive(_isUseInven);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            _isUseSkillWindow = !_isUseSkillWindow;

            _skillMgr.SetSkillWindowActive(_isUseSkillWindow);
        }
    }

    #region Init
    /// <summary> Player Info UI Set </summary>
    private void PlayerUI_Init()
    {
        _playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP);
        _playerUICtr.SetMPbar(m_nCurMP, m_nMaxMP);
        _playerUICtr.SetEXPbar(m_nCurExp, m_nMaxExp);
        _playerUICtr.DisplayInfo(m_nLevel, m_nMaxHP, m_nMaxMP, m_nCurSTR);
    }

    private void Input_init()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");
        runInput = Input.GetAxis("Run");
        attackInput = Input.GetButton("Fire1");
    }


    public void CurrentQuestInit(List<QuestData> currentQuestList)
    {
        _gameMgr.UpdateQuestUI(currentQuestList);
        _gameMgr.SetPlayerQuestList(currentQuestList);
    }

    #endregion


    public override void Buff(int _str)
    {
        base.Buff(_str);
        _playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP); //HP Bar UI ����
    }

    /// <summary> �÷��̾� ����� �߻� �̺�Ʈ </summary>
    public override void OnDamage(int _str)
    {
        this.m_nCurHP -= _str;

        //if (m_fCurHP <= 0) this.objState = ObjectState.DEAD; //��� ����

        if (m_nCurHP <= 0) m_nCurHP = 0;
        _playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP); //HP Bar UI ����
    }

    /// <summary> �÷��̾��� ��� ó�� </summary>
    protected override void Die()
    {
        Debug.Log("Player Die");
        StopAllCoroutines();
        _anim.SetTrigger(hashDead);
        m_bisDead = true;

        OnPlayerDie(); //delegate event Play
        GameManager.instance.PlayerDie();
    }


    /// <summary> ü�� ȸ�� �ۿ� </summary>
    public void RecoveryHP(int value)
    {
        m_nCurHP += value;

        if (m_nCurHP > m_nMaxHP) m_nCurHP = m_nMaxHP;
        _playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP);
    }

    /// <summary> ���� ȸ�� �ۿ� </summary>
    public void RecoveryMP(int value)
    {
        m_nCurMP += value;

        if (m_nCurMP > m_nMaxHP) m_nCurMP = m_nMaxMP;
        _playerUICtr.SetMPbar(m_nCurMP, m_nMaxHP);
    }

    /// <summary> ����ġ ȹ�� ó�� �� ������ �Լ� ȣ�� (���, ����Ʈ ���)</summary>
    public void SetEXP(int _exp)
    {
        this.m_nCurExp += _exp;
        while(m_nCurExp >= m_nMaxExp)
        {
            //�ʰ��� ��ŭ ���ش�.
            m_nCurExp -= m_nMaxExp;
            LevelUP();
        }
        _playerUICtr.SetEXPbar(m_nCurExp, m_nMaxExp);
    }

    /// <summary> ������ �������� Inventory�� ���� </summary>
    public void AddInven(Dictionary<ItemData, int> _itemDic)
    {
        foreach(var itemDic in _itemDic)
        {
            _itemInvenMgr.AddItem(itemDic.Key, itemDic.Value);
        }
    }

    /// <summary> AddInven �����ε� </summary>
    public void AddInven(ItemData itemData, int amount)
    {
        _itemInvenMgr.AddItem(itemData, amount);
    }


    /// <summary> �÷��̾��� ���� �����ϱ� </summary>
    public void SavePlayer() => SaveSys.SavePlayer(this);

    /// <summary> �÷��̾��� ���� �ε� </summary>
    protected override void LoadData()
    {
        _playerData = SaveSys.LoadObject("PlayerData.Json");

        playerLevelData = SaveSys.LoadAllData().PlayerLevelDB;

        //����� �����Ͱ� �ִٸ� ĳ���� ������ �ҷ��´�.
        if (_playerData != null)
        {
            
            m_nLevel = _playerData.nLevel;
            m_nMaxHP = _playerData.nMaxHP;
            m_nCurHP = _playerData.nCurHP;

            m_nMaxMP = _playerData.nMaxMP;
            m_nCurMP = _playerData.nCurMP;

            m_nCurSTR = _playerData.nCurSTR;
            m_nCurExp = _playerData.nCurExp;
            m_nMaxExp = _playerData.ntotalExp;

            _currentQuestList = _playerData.currentQuestList;
            return;
        }
        else
        {
            // ����� �����Ͱ� ���ٸ�, 1���� ���� ����
            PlayerLevelUP(0);
        }

        #region preSetting
        //m_nLevel = objData.GetLevel();
        //m_nMaxHP = objData.GetMaxHP();
        //m_nCurHP = objData.GetCurHP();

        //m_nMaxMP = objData.GetMaxMP();
        //m_nCurMP = objData.GetCurMP();

        //m_nCurSTR = objData.GetCurSTR();
        //m_nCurExp = objData.GetCurExp();

        //m_nLevel = objData.GetLevel();
        //m_nMaxHP = objData.GetMaxHP();
        //m_nCurHP = objData.GetCurHP();

        //m_nMaxMP = objData.GetMaxMP();
        //m_nCurMP = objData.GetCurMP();
        #endregion

    }

    /// <summary> Character LevelUp System </summary>
    public override void LevelUP()
    {
        PlayerLevelUP(m_nLevel);
        PlayerUI_Init();
        base.LevelUP();
    }

    /// <summary> �÷��̾��� ������ �� ĳ���� ���� �ʱ�ȭ �Լ� </summary>
    /// <param name="value"> ���� ������ ������ ������ �� ������ �ҷ� �´�. </param>
    private void PlayerLevelUP(int value)
    {
        var playerData = playerLevelData[value];

        m_nLevel = playerData.nLevel;
        m_nMaxHP = playerData.nMaxHP;
        m_nCurHP = playerData.nMaxHP;

        m_nMaxMP = playerData.nMaxMP;
        m_nCurMP = playerData.nMaxMP;

        m_nCurSTR = playerData.nSTR;

        m_nMaxExp = playerData.ntotalExp;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            NPC npcCtr = coll.gameObject?.GetComponent<NPC>();
            Debug.Assert(npcCtr != null, "npcCtr is NULL");

            int questAmount = _currentQuestList.Count;

            for(int i = 0; i < questAmount; i++)
            {
                var curQuest = _currentQuestList[i];

                if (curQuest.nDestID == npcCtr.GetID())
                {
                    StartCoroutine(_gameMgr.CompleteDialog(curQuest, i)); // �ش� ����Ʈ�� Dialog�� ����.
                    return;
                }
            }
        
            //���簡���� ����Ʈ ��� ��
            var poQuestList = _questSystem.GetPossibleQuest();

            foreach (var quest in poQuestList)
            {
                //�ش� NPC�� ID�� ��ġ�ϴ°� �ִٸ�
                if (quest.nID == npcCtr.GetID())
                {
                    foreach(var curQuest in _currentQuestList)
                    {
                        if(curQuest.nQuestID == quest.nQuestID)
                        {
                            StartCoroutine(_gameMgr.UnsolvedQuestDialog(quest.nQuestID));
                            return;
                        }
                    }

                    StartCoroutine(_gameMgr.PlayDialog(quest));//�ش� ����Ʈ�� Dialog�� ����.
                    break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        SavePlayer();
    }
}
