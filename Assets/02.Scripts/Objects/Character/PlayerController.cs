using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using InHwan.CircularQueue;


public class PlayerController : Character
{
    /********************************************
     *                Fields
     ********************************************/

    #region static Fields
    public static PlayerController instance = null;
    #endregion
    #region Delegate / Event Fields
    //�÷��̰� �׾��� ��, ��� ���� ����
    public delegate void PlayerDiehandler();           
    public static event PlayerDiehandler OnPlayerDie;

    public delegate void PlayerQuestSetting();             
    public static event PlayerQuestSetting OnSettingNPCQuest;
    #endregion

    #region Anim Hashing Fields
    private readonly int hashTakeDamage  = Animator.StringToHash("DoTakeDamage");
    private readonly int hashStrafe      = Animator.StringToHash("Strafe");
    private readonly int hashForward     = Animator.StringToHash("Forward");
    private readonly int hashRun         = Animator.StringToHash("Run");
    private readonly int hashMoveSpeed   = Animator.StringToHash("MoveSpeed");
    private readonly int hashAttackSpeed = Animator.StringToHash("AttackSpeed");
    private readonly int hashRolling     = Animator.StringToHash("IsRolling");
    private readonly int[] hashAttack = new int[] { Animator.StringToHash("DoAttack1"), Animator.StringToHash("DoAttack2"), Animator.StringToHash("DoAttack3")};
    #endregion

    #region private Fields
    #region ���� ����
    [SerializeField] private float m_fAttackDelay = 0.5f;
    private float m_fAttackRunTime = 0.0f;
    private float m_fAttackInitTime = 1.5f;
    private int m_nAttackCount = 0;
    #endregion
    #region GetComponent �ʱ�ȭ�� Fields
    private Rigidbody _rigid;
    private Weapon _weaponCtr;

    private Weapon _basicWeaponCtr;
    #endregion

    [SerializeField] private float moveSpeed = 5.0f; // �̵� �ӵ�

    private CameraController _cameraCtr;
    private PlayerLevelData[] _playerLevelData; // player Level�� ���� �����͵�
    private List<QuestData> _currentQuestList = new List<QuestData>(); //������ ����Ʈ ���
   
    private QuestSystem _questSystem;

    //�Ŵ��� ��ũ��Ʈ
    private GameManager _gameMgr;
    private PlayerStatManager _playerStatMgr;
    private ItemInventoryManager _itemInvenMgr;
    private MerChantInventoryManager _merChantInvenMgr;
    private QuickInvenManager _quickInvenMgr;

    private PlayerData _playerData;
    private PlayerStatusData _playerStatData;

    private string m_sCurVillageScene; //���� �÷��̾ �ִ� �� ����

    private int m_nRubyAmount; //���� �Ӵ� ��

    private float _xInput;
    private float _zInput;
    private float _runInput;
    private bool _rollingInput;
    private bool _attackInput;

    private bool _isRolling;
    private bool _isRollingCoolTime; // ������ ����� �ð�
    private bool _isPlayerDown;
    private bool _isUseInven;
    private bool _isUseSkillWindow;
    private bool _isUseStatusWindow;
    private bool _isPlayDialog;
    #endregion
    /********************************************
     *                Get, Set Methods
     ********************************************/

    #region Get Methods
    public int GetRubyAmount() => m_nRubyAmount;
    public bool GetUseInven() => _isUseInven;
    public CameraController GetCameraCtr() => _cameraCtr;
    public List<QuestData> GetPlayerQuestList() => _currentQuestList;
    public QuestSystem GetQuestSystem() => _questSystem;
    public string GetPlayerStayingVilliage() => m_sCurVillageScene;
    /// <summary> �÷��̾� �ൿ ���� ���� </summary>
    private bool GetFreezePlayer() => _isUseInven || _isUseSkillWindow || _isPlayDialog || m_bisAttack || _isPlayerDown || m_bisDead;
    #endregion
    #region Set Methods
    public void SetUseItemInven(bool value) { _isUseInven = value; }                                    // �κ��丮â�� ���� �ִ°�
    public void SetUseSKill_Inven(bool value) { _isUseSkillWindow = value; }                            // ��ųâ�� �����ִ°�
    public void SetUsePlayDialog(bool value) { _isPlayDialog = value; }                                 // ���̾�α� ������ΰ�  
    public void SetQuestSystem(QuestSystem questSystem) => _questSystem = questSystem;                  // ����Ʈ �ý���
    public void SetGameManager(GameManager gameMgr) => _gameMgr = gameMgr;                              // ���� �Ŵ���
    public void SetItemInvenManager(ItemInventoryManager itemInvenMgr) => _itemInvenMgr = itemInvenMgr; // ������ �κ��丮 �Ŵ���
    public void SetSkillMgr(Skill_InvenManager skillMgr) => _skillMgr = skillMgr;                             // ��ų �Ŵ���
    public void SetQuickInvenMgr(QuickInvenManager quickInvenMgr) => _quickInvenMgr = quickInvenMgr;
    public void SetPlayerUIMgr(PlayerStatManager playerUIMgr) => _playerStatMgr = playerUIMgr;              // �÷��̾� UI �Ŵ���
    public void SetMerChantMgr(MerChantInventoryManager merChantMgr) => _merChantInvenMgr = merChantMgr;
    public void SetCameraCtr(CameraController value) => _cameraCtr = value;                             // ī�޶�
    public void SetVillageScene(string name) => m_sCurVillageScene = name;
    #endregion


    /********************************************
     *                Unity Event
     ********************************************/

    #region Unity Event

    protected override void Start()
    {
        UpdateQuest(m_nLevel); //�÷��̾� ���� ���� ����Ʈ ���� ������Ʈ
        _currentQuestList = _questSystem.GetInProgressQuestList(); //�������� ����Ʈ �޾ƿ���
        CurrentQuestInit(_currentQuestList);

        UpdateNPCQuest();

        PlayerUI_Init();

        _itemInvenMgr.UpdateRubyAmount(m_nRubyAmount);

        base.Start();
    }

    private void FixedUpdate()
    {
        CharacterMovement();
    }

    /// <summary> BossAttack���� �浹 ó�� </summary>
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("BossAttack"))
        {
            BossMonster monsterCtr = coll.gameObject?.GetComponentInParent<ObjectBase>() as BossMonster;
            Debug.Assert(monsterCtr != null, "MonsterCtr is NULL");

            int monPower = monsterCtr.GetSkillDamage();
            int nDamage = monsterCtr.GetAttackPower(monPower);
            OnDamage(nDamage);
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("BossKnockAttack"))
        {
            BossMonster monsterCtr = coll.gameObject?.GetComponentInParent<ObjectBase>() as BossMonster;
            Debug.Assert(monsterCtr != null, "MonsterCtr is NULL");
            int monPower = monsterCtr.GetSkillDamage();
            int nDamage = monsterCtr.GetAttackPower(monPower);

            OnDamage(nDamage, true);
        }
    }

    NPC npcCtr;
    bool isComplete = false;  //�Ϸ� ����Ʈ ����
    bool isInprogress = false;  //���� ���� ����Ʈ ����
    bool isPossible = false;  //������ ����Ʈ ����
    /// <summary> NPC ��ȣ�ۿ� ó��</summary>
    private void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {

            //1. Npc ������ �� ���� �ʱ�ȭ ����
            if (npcCtr == null)
            {
                npcCtr = coll.gameObject?.GetComponent<NPC>();
                Debug.Assert(npcCtr != null, "npcCtr is NULL");

                Vector3 pos = this.transform.position + (Vector3.up * 1.5f) + (transform.right * 0.5f);

                isComplete = npcCtr.GetIsCompleteQuest();
                isInprogress = npcCtr.GetIsInProgressQuest();
                isPossible = npcCtr.GetIsPossibleQuest();


                //�̺�Ʈ ����Ʈ ���� ���� ��ȣ�ۿ�Ű Ȱ��ȭ
                if (isPossible || isInprogress || isComplete || npcCtr is MerChantNPC)
                    _playerStatMgr.ConversationKeyActiveOn(pos);
            }


            //2.NPC�� ��ȣ�ۿ��ϱ�
            if (Input.GetKeyDown(KeyCode.F) && !_isPlayDialog)
            {
                _playerStatMgr.ConversationKeyActiveOff();
                PlayerIdleState();

                #region ����Ʈ ��ȣ�ۿ�
                //1. �Ϸ� ����Ʈ�� ���� ��
                if (isComplete)
                {
                    QuestData quest = npcCtr.GetQuestData();
                    int index = npcCtr.GetCompleteQuestIndex();
                    StartCoroutine(_gameMgr.CompleteDialog(quest, index)); // �ش� ����Ʈ�� Dialog�� ����.
                }
                //2. ���� ���� ����Ʈ�� ���� ��
                else if(isInprogress)
                {
                    int unsolvedIndex = npcCtr.GetUnSolvedQuestID();
                    StartCoroutine(_gameMgr.UnsolvedQuestDialog(unsolvedIndex));
                }
                //3. ���� ������ ����Ʈ�� ���� ��
                else if (isPossible)
                {
                    QuestData quest = npcCtr.GetQuestData();
                    StartCoroutine(_gameMgr.PlayDialog(quest));//�ش� ����Ʈ�� Dialog�� ����.

                }
                #endregion

                
                if(npcCtr is MerChantNPC merCtr && !(isComplete || isPossible || isInprogress))
                {
                    merCtr.MerChantInvenOn();
                    _itemInvenMgr.SetWindowActive(true);
                    PlayerIdleState();
                }

            }
     
            if (!_isPlayDialog)
            {
                UpdateNPCQuest();
            }
        }

    }
    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            _playerStatMgr.ConversationKeyActiveOff(); //��ȣ�ۿ�Ű �񰡽�ȭ

            if (npcCtr is MerChantNPC merCtr)
            {
                merCtr.MerChantInvenOff();
                _itemInvenMgr.SetWindowActive(false);

            }

            isComplete = false;  //�Ϸ� ����Ʈ ����
            isInprogress = false;  //���� ���� ����Ʈ ����
            isPossible = false;  //������ ����Ʈ ����
            npcCtr = null;
        }
    }

    private void Update()
    {
        Input_init();

        if(m_fAttackInitTime > m_fAttackRunTime) m_fAttackRunTime += Time.deltaTime;

        StatusWindowActiveButton();
        UseQuickSlotButton();
        PlayerAttack();

    }

    private void OnDestroy()
    {
        ClearStaticField();
        SavePlayer();
    }
    #endregion


    /********************************************
     *                 Methods
     ********************************************/

    #region override Methods
    protected override void InitObj()
    {
        base.InitObj();
        PlayerController_Init();
    }
    /// <summary> �÷��̾��� ���� �ε� </summary>
    protected override void LoadData()
    {
        _playerData = SaveSys.LoadObject("PlayerData.Json");

        _playerLevelData = SaveSys.LoadAllData().PlayerLevelDB;

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

            m_nRubyAmount = _playerData.nRubyAmount;

            m_sCurVillageScene = _playerData.sVillageScene;

            return;
        }
        else
        {
            // ����� �����Ͱ� ���ٸ�, 1���� ���� ����
            PlayerLevelUP(0);
        }
    }

    /// <summary> Character LevelUp System </summary>
    public override void LevelUP()
    {
        PlayerLevelUP(m_nLevel);
        PlayerUI_Init();

        UpdateNPCQuest();
        base.LevelUP();
    }

    /// <summary> �÷��̾� ����� �߻� �̺�Ʈ </summary>
    public override void OnDamage(int _str, bool _isKnockback = false, Transform posTr = null)
    {
        //���� ���� - (���� + �߰� ����) = �� ���� ����
        int nDamage = _str - GetTotalDefence();
        if (nDamage < 0) nDamage = 0;

        this.m_nCurHP -= nDamage;

        //���� ���� Text ����ȭ
        ActiveDamageText(nDamage);

       

        if (_isKnockback == true)
        {
            if (posTr != null)
            {
                Vector3 pos = posTr.position;

                pos.y = transform.position.y;

                transform.LookAt(pos);
            }

            _rigid.velocity = Vector3.zero;
            _rigid.angularVelocity = Vector3.zero;

            _rigid.AddForce(-_rigid.transform.forward * 2000.0f);
            _rigid.AddForce(_rigid.transform.up * 2000.0f);

            _anim.SetTrigger(hashTakeDamage);

            StartCoroutine(CheckDownTime());
        }

        if (m_nCurHP <= 0)
        {
            m_nCurHP = 0;
            Die();
        }

        _playerStatMgr.UpdateHPbar(m_nCurHP, m_nMaxHP); //HP Bar UI ����
    }

    /// <summary> �÷��̾��� ��� ó�� </summary>
    protected override void Die()
    {
        StopAllCoroutines();
        _anim.SetTrigger(hashDead);
        m_bisDead = true;
        this.gameObject.layer = 3;
        OnPlayerDie(); //delegate event Play
    }

    /// <summary>
    /// 3ȸ �޺� �⺻ ����
    /// </summary>
    protected override IEnumerator Attack()
    {
        if (m_fAttackRunTime > m_fAttackDelay && !m_bisAttack)
        {
            m_bisAttack = true;
            _weaponCtr.Use(); //���� �ݶ��̴� Ȱ��ȭ / ��Ȱ��ȭ ��ƾ
                              //�⺻������ �ð��� �����ʱ�ȭ �ð����� ������ٸ� �޺� x
            if (m_fAttackRunTime > m_fAttackInitTime) m_nAttackCount = 0;
            _anim.SetTrigger(hashAttack[m_nAttackCount]);

            m_nAttackCount++;
            if (m_nAttackCount >= 3) m_nAttackCount = 0;

            m_fAttackRunTime = 0;
            //������ ���� ��, �� ��� ���
            if (m_nAttackCount == 0)
                yield return new WaitForSeconds(m_fAttackDelay + 1.0f);
            else
                yield return new WaitForSeconds(m_fAttackDelay);

            m_bisAttack = false;
        }
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
        //���� �������� ��ų ������ ũ�ٸ� ���� �ߴ�.
        if (m_nCurMP < curSkill.GetSkillManaAmount()) yield break;

        //��� ���� ���°� �ƴϸ�
        if (curSkill.GetInAvailable())
        {
            //������ �̹����� �����Ÿ�
            _skillMgr.SkillUsedWarring(skill_Idx);
            yield break;
        }


        m_bisAttack = true;

        _anim.SetTrigger(curSkill.GetAnimHash());

        _skillMgr.UseSkill(curSkill, this, ref m_nCurMP);
        _playerStatMgr.UpdateMPbar(m_nCurMP, m_nMaxMP);

        _skillMgr.StartSkillCoolTime(skill_Idx, curSkill);

        yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        m_bisAttack = false;
    }
    #endregion

    #region Init Moethods
    /// <summary> �÷��̾� �ʱ�ȭ </summary>
    private void PlayerController_Init()
    {

        #region SingTone
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        //DontDestroyOnLoad(this.gameObject);
        #endregion

        #region GetComponent
        _rigid = GetComponent<Rigidbody>();
        _basicWeaponCtr = GetComponentInChildren<Weapon>();
        _weaponCtr = _basicWeaponCtr;
        #endregion

        //���ݼӵ�
        _anim.SetFloat(hashAttackSpeed, m_fAttackDelay);
        _playerStatData = new PlayerStatusData(this);
        
        #region Manager��ũ��Ʈ�� GameManager���� �ʱ�ȭ ��Ŵ
        //_skillMgr = GetComponent<SkillManager>();
        //_inven = GetComponent<ItemInventoryManager>();
        //_inven.SetPlayerCtr(this);
        //_skillMgr.SetPlayerCtr(this);
        //_skillMgr.SetSkillPower(m_nCurSTR);
        //_skInvenUI = _skillMgr.GetSkInvenUI(); //skillMgr�� Awake() ���� skinvenUI�� �Ҵ����
        #endregion
    }
    /// <summary> Player Info UI Set </summary>
    private void PlayerUI_Init()
    {
        _playerStatMgr.UpdateHPbar(m_nCurHP, m_nMaxHP);
        _playerStatMgr.UpdateMPbar(m_nCurMP, m_nMaxMP);
        _playerStatMgr.UpdateEXPbar(m_nCurExp, m_nMaxExp);
        _playerStatMgr.UpdateLevelText(m_nLevel);
        _playerStatMgr.SetPlayerStatusData(_playerStatData);
    }
    /// <summary> Input ������ �Ҵ� </summary>
    private void Input_init()
    {
        _xInput = Input.GetAxis("Horizontal");
        _zInput = Input.GetAxis("Vertical");
        _runInput = Input.GetAxis("Run");
        _rollingInput = Input.GetButton("Rolling");
        _attackInput = Input.GetButton("Fire1");
    }
    public void CurrentQuestInit(List<QuestData> currentQuestList)
    {
        _gameMgr.UpdateQuestUI(currentQuestList);
        _gameMgr.SetPlayerQuestList(currentQuestList);
    }

    #endregion

    #region private Methods
    /// <summary> Character dir, move, speed, rot, anim
    /// <para/> Current Use FixedUpdate()
    /// </summary>
    private void CharacterMovement()
    {
        if (GetFreezePlayer()) return;

        Vector3 direction = new Vector3(_xInput, 0, _zInput);

        _anim.SetFloat(hashStrafe, direction.x);
        _anim.SetFloat(hashForward, direction.z);
        _anim.SetFloat(hashRun, _runInput);

        float movementSpeed = moveSpeed;
        if (_runInput < 0.5f) movementSpeed *= 0.5f;

        DoRolling(ref movementSpeed);


        Vector3 movement = direction.normalized * movementSpeed * Time.fixedDeltaTime;

        _anim.SetFloat(hashMoveSpeed, movementSpeed);


        if (movement != Vector3.zero)
        {
            Quaternion destRot = _cameraCtr.transform.localRotation;

            destRot.x = 0;
            destRot.z = 0;

            _rigid.transform.rotation = destRot;
        }

        _rigid.transform.Translate(movement);
    }
    #region Attack ����
    /// <summary> Player�� �⺻/��ų ���� ���� </summary>
    private void PlayerAttack()
    {
        if (GetFreezePlayer()) return;

        if (_attackInput) StartCoroutine(Attack());
        else Skill_Attack();
    }
    /// <summary> Player�� ��ų ���� </summary>
    private void Skill_Attack()
    {
        Commandkey.UseSkillAttack(ActionType.SKILLA, () => StartCoroutine(SkillAttack(1)));
        Commandkey.UseSkillAttack(ActionType.SKILLB, () => StartCoroutine(SkillAttack(2)));
        Commandkey.UseSkillAttack(ActionType.SKILLC, () => StartCoroutine(SkillAttack(3)));
        Commandkey.UseSkillAttack(ActionType.SKILLD, () => StartCoroutine(SkillAttack(4)));
        Commandkey.UseSkillAttack(ActionType.SKILLE, () => StartCoroutine(SkillAttack(5)));
        Commandkey.UseSkillAttack(ActionType.SKILLF, () => StartCoroutine(SkillAttack(6)));

        #region Input Alpha 1~6
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    StartCoroutine(SkillAttack(1));
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    StartCoroutine(SkillAttack(2));
        //}
        //else if(Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    StartCoroutine(SkillAttack(3));
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    StartCoroutine(SkillAttack(4));
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    StartCoroutine(SkillAttack(5));
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    StartCoroutine(SkillAttack(6));
        //}
        #endregion
    }
    #endregion
    /// <summary> shift + spacebar ���� �� ������ ���� </summary>
    private void DoRolling(ref float movementSpeed)
    {
        if (_rollingInput && _runInput > 0.5f && !_isRollingCoolTime)
        {
            StartCoroutine(CheckRolling());
            movementSpeed *= 2f;
            _anim.SetBool(hashRolling, true);
        }
        else
        {
            _anim.SetBool(hashRolling, false);
        }
    }
    /// <summary> �÷��̾� ������ ����ؼ� ����Ʈ ��ϵ� ���� </summary>
    private void UpdateQuest(int nLevel) => _gameMgr.UpdateQuestList(nLevel);
    /// <summary> Inven â�� ���� �ִ����� ���� bool �� ��ȯ </summary>
    ///     /// <summary> ������ ��� �Լ� </summary>
    private void UseQuickSlotButton()
    {
        Commandkey.UseQuickSlot(ActionType.QuickSlotA, () => _quickInvenMgr.Use(0));
        Commandkey.UseQuickSlot(ActionType.QuickSlotB, () => _quickInvenMgr.Use(1));
    }
    /// <summary> â�� �����ִ°�(�κ��丮 ��) </summary>
    private bool IsActiveWindowOnState()
    {
        return _isUseInven || _isUseSkillWindow || _isUseStatusWindow;
    }
    /// <summary> �÷��̾��� ������ �� ĳ���� ���� �ʱ�ȭ �Լ� </summary>
    /// <param name="value"> ���� ������ ������ ������ �� ������ �ҷ� �´�. </param>
    private void PlayerLevelUP(int value)
    {
        var playerData = _playerLevelData[value];

        m_nLevel = playerData.nLevel;
        m_nMaxHP = playerData.nMaxHP;
        m_nCurHP = playerData.nMaxHP;

        m_nMaxMP = playerData.nMaxMP;
        m_nCurMP = playerData.nMaxMP;

        m_nCurSTR = playerData.nSTR;
        m_nDefence = playerData.nDefence;

        m_nMaxExp = playerData.ntotalExp;
    }
    /// <summary> static Field�� ����ִ� �Լ� </summary>
    private void ClearStaticField()
    {
        instance = null;
        OnPlayerDie = null;
        OnSettingNPCQuest = null;
    }
    /// <summary> �÷��̾��� ���� �����ϱ� </summary>
    private void SavePlayer() => SaveSys.SavePlayer(this);
    #endregion

    #region public Methods
    /// <summary> �÷��̾� IDLEȭ </summary>
    public void PlayerIdleState()
    {
        _anim.SetFloat(hashStrafe, 0);
        _anim.SetFloat(hashForward, 0);
        _anim.SetFloat(hashRun, 0);
    }
    /// <summary> Invnetory Active</summary>
    public void StatusWindowActiveButton()
    {
        Commandkey.CheckInput(ActionType.Inventory, ref _isUseInven, _itemInvenMgr);
        Commandkey.CheckInput(ActionType.SkillWindow, ref _isUseSkillWindow, _skillMgr);
        Commandkey.CheckInput(ActionType.StatusWindow, ref _isUseStatusWindow, _playerStatMgr);

        #region ����
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    _isUseInven = !_isUseInven;

        //    _itemInvenMgr.SetWindowActive(_isUseInven);
        //    SetController();

        //    PlayerIdleState();
        //}
        //else if (Input.GetKeyDown(KeyCode.K))
        //{
        //    _isUseSkillWindow = !_isUseSkillWindow;

        //    _skillMgr.SetWindowActive(_isUseSkillWindow);
        //    SetController();

        //    PlayerIdleState();
        //}
        //else if (Input.GetKeyDown(KeyCode.S))
        //{

        //    _isUseStatusWindow = !_isUseStatusWindow;

        //    _playerUIMgr.SetWindowActive(_isUseStatusWindow);
        //    SetController();

        //    PlayerIdleState();
        //}
        #endregion
    }
    /// <summary> ��Ʈ�ѷ��� ��ܾ� �ϴ��� üũ �Ѵ�. </summary>
    public void CheckFreezController()
    {
        bool isWindowOn = IsActiveWindowOnState();

        GetCameraCtr().UseWindow(isWindowOn); //ī�޶��� ȸ�� ����

        if (isWindowOn) GameManager.instance.VisibleCursor();
        else
            GameManager.instance.InvisibleCursor();
    }
    /// <summary> �÷��̾ ������ ����Ʈ �߰� </summary>
    public void AddPlayerQuest(QuestData questData)
    {
        questData.bIsProgress = true;
        _currentQuestList.Add(questData);
    }
    /// <summary> ü�� ȸ�� �ۿ� </summary>
    public void RecoveryHP(int value)
    {
        m_nCurHP += value;

        if (m_nCurHP > m_nMaxHP) m_nCurHP = m_nMaxHP;
        _playerStatMgr.UpdateHPbar(m_nCurHP, m_nMaxHP);
    }
    /// <summary> ���� ȸ�� �ۿ� </summary>
    public void RecoveryMP(int value)
    {
        m_nCurMP += value;

        if (m_nCurMP > m_nMaxHP) m_nCurMP = m_nMaxMP;
        _playerStatMgr.UpdateMPbar(m_nCurMP, m_nMaxHP);
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
        _playerStatMgr.UpdateEXPbar(m_nCurExp, m_nMaxExp);
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
    /// <summary> �÷��̾� �Ӵ� �߰� </summary>
    public void AddMoney(int nMoney)
    {
        m_nRubyAmount += nMoney;
        _itemInvenMgr.UpdateRubyAmount(m_nRubyAmount);
    }
    /// <summary> ���� ���� </summary>
    public void PutOnWeapon(EquipmentItemData weaponData)
    {
        if(_equipWeaponGo != null)
        {
            TakeOffWeapon();
        }
        _basicWeaponCtr.gameObject.SetActive(false);

        GameObject weaponPrefabGo = weaponData.GetWeaponPrefab();
        Vector3 pos        = weaponData.GetPos();
        Quaternion rot     = weaponData.GetRot();

        _equipWeaponGo = Instantiate(weaponPrefabGo, _weaponTr);
        
        _equipWeaponGo.transform.localPosition = pos;
        _equipWeaponGo.transform.localRotation = rot;

        _weaponCtr = _equipWeaponGo.GetComponent<Weapon>();
    }
    /// <summary> ���� ���� ���� </summary>
    public void TakeOffWeapon()
    {
        _basicWeaponCtr.gameObject.SetActive(true);
        _weaponCtr = _basicWeaponCtr;
        DestroyImmediate(_equipWeaponGo);
    }
    /// <summary> NPC Quest�� ������Ʈ�ϴ� �̺�Ʈ�Լ� ���� </summary>
    public void UpdateNPCQuest()
    {
        if (OnSettingNPCQuest != null) OnSettingNPCQuest();
    }
    #endregion


    /********************************************
    *                 Coroutine
    ********************************************/
    #region Coroutine
    /// <summary>
    /// ������ �������� ���� ���ο� ������ ��Ÿ�� üũ �Լ�
    /// Layer�� Ignore�� �������� �ٲ��ش�.
    /// </summary>
    private IEnumerator CheckRolling()
    {
        _isRolling = true;
        _isRollingCoolTime = true;

        gameObject.layer = 2;
        yield return new WaitForSeconds(2.0f);

        gameObject.layer = 3;
        _isRolling = false;

        yield return new WaitForSeconds(4.0f);
        _isRollingCoolTime = false;
    }
    /// <summary> �ǰݴ��� ����, Down ���� �ð� üũ </summary>
    private IEnumerator CheckDownTime()
    {
        _isPlayerDown = true;
        yield return new WaitForSeconds(1.5f);
        _isPlayerDown = false;
    }
    #endregion

}
