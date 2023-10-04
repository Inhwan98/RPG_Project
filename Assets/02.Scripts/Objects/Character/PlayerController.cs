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

    #region GetComponent 초기화 항목
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

    //몬스터를 자동 추적할 몬스터 리스트
    private List<KeyValuePair<int, Transform>> monsterObjs_list = new List<KeyValuePair<int, Transform>>();
    //플레이어 UI (HPbar) Manager

    [SerializeField]
    private PlayerUIManager _playerUICtr;

    private List<QuestData> _currentQuestList = new List<QuestData>();
   
    private QuestSystem _questSystem;

    private ItemInventoryManager _itemInvenMgr;

    private PlayerData _playerData;


    public bool GetUseInven() => _isUseInven;
    public CameraController GetCameraCtr() => _cameraCtr;
    public List<QuestData> GetPlayerQuestList() => _currentQuestList;

    public void SetUseItemInven(bool value) { _isUseInven = value; }                                    // 인벤토리창이 켜져 있는가
    public void SetUseSKill_Inven(bool value) { _isUseSkillWindow = value; }                            // 스킬창이 켜져있는가
    public void SetUsePlayDialog(bool value) { _isPlayDialog = value; }                                 // 다이얼로그 사용중인가  
    public void SetQuestSystem(QuestSystem questSystem) => _questSystem = questSystem;                  // 퀘스트 시스템
    public void SetGameManager(GameManager gameMgr) => _gameMgr = gameMgr;                              // 게임 매니져
    public void SetItemInvenManager(ItemInventoryManager itemInvenMgr) => _itemInvenMgr = itemInvenMgr; // 아이템 인벤토리 매니져
    public void SetSkillMgr(SkillManager skillMgr) => _skillMgr = skillMgr;                             // 스킬 매니져
    public void SetCameraCtr(CameraController value) => _cameraCtr = value;                             // 카메라

    public void AddPlayerQuest(QuestData questData) => _currentQuestList.Add(questData);

    /// <summary> 플레이어 레벨에 비례해서 퀘스트 목록들 갱신 </summary>
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

    /// <summary> 플레이어 초기화 </summary>
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

        //공격속도
        _anim.SetFloat(hashAttackSpeed, m_fAttackDelay);

        #region Manager스크립트는 GameManager에서 초기화 시킴
        //_skillMgr = GetComponent<SkillManager>();
        //_inven = GetComponent<ItemInventoryManager>();
        //_inven.SetPlayerCtr(this);
        //_skillMgr.SetPlayerCtr(this);
        //_skillMgr.SetSkillPower(m_nCurSTR);
        //_skInvenUI = _skillMgr.GetSkInvenUI(); //skillMgr은 Awake() 전에 skinvenUI를 할당받음
        #endregion

    }

    protected override void Start()
    {
        //_cam           = Camera.main;

        UpdateQuest(); //퀘스트 정보 업데이트
        CurrentQuestInit(_currentQuestList);

        PlayerUI_Init();
       
        base.Start(); //skill_List 가 부모에서 초기화 됌
    }


    /// <summary>
    /// 퀘스트 정보를 업데이트 해준다.
    /// 게임 시작, 레벨업 할때마다 호출 해줘야 한다.
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

    /// <summary> Player의 기본/스킬 공격 구성 </summary>
    private void PlayerAttack()
    {
        if (m_bisAttack || _isUseInven || _isUseSkillWindow || _isPlayDialog) return;

        if (attackInput) StartCoroutine(Attack());
        else Skill_Attack();
    }

    /// <summary> Player의 스킬 공격 </summary>
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
            //기본공격의 시간이 공격초기화 시간보다 길어진다면 콤보 x
            if (m_fAttackRunTime > m_fAttackInitTime) m_nAttackCount = 0;
            _anim.SetTrigger(hashAttack[m_nAttackCount]);

            m_nAttackCount++;
            if (m_nAttackCount >= 3) m_nAttackCount = 0;

            m_bisAttack = true;
            m_fAttackRunTime = 0;
            //마지막 공격 후, 더 길게 대기
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

    /// <summary> Player의 스킬 공격 
    /// <para/> SKill Manager의 플레이어 스킬Index 를 지닌
    /// <para/> skill_Datas[] 배열 이용
    /// </summary>
    protected override IEnumerator SkillAttack(int skillNum)
    {
        //현재 사용할 스킬. 0번째 부터 시작함.
        int skill_Idx = skillNum - 1;
        SkillData curSkill = _skill_Datas[skill_Idx];

        //획득하지 않은 상태면
        if (curSkill == null) yield break;

        //사용 가능 상태가 아니면
        if (curSkill.GetInUse())
        {
            //빨간색 이미지가 깜빡거림
            _skillMgr.SkillUsedWarring(skill_Idx);
            //StartCoroutine(_skInvenUI.SkillUsedWarring(skill_Idx));
            yield break;
        }

        //보유 마나보다 스킬 마나가 크다면 공격 중단.
        //if (m_fCurMP < curSkill.GetSkillManaAmount()) yield break; 공격 중단 없음

        m_bisAttack = true;

        _anim.SetTrigger(curSkill.GetAnimHash());

        _skillMgr.UseSkill(curSkill, this, ref m_nCurMP);
        _playerUICtr.SetMPbar(m_nCurMP, m_nMaxMP);

        _skillMgr.StartSkillCoolTime(skill_Idx, curSkill);

        yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        m_bisAttack = false;
        //circualrQueue.DeQueue(); //스킬 순서 당겨 주기
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
        _playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP); //HP Bar UI 수정
    }

    /// <summary> 플레이어 대미지 발생 이벤트 </summary>
    public override void OnDamage(int _str)
    {
        this.m_nCurHP -= _str;

        //if (m_fCurHP <= 0) this.objState = ObjectState.DEAD; //사망 없음

        if (m_nCurHP <= 0) m_nCurHP = 0;
        _playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP); //HP Bar UI 수정
    }

    /// <summary> 플레이어의 사망 처리 </summary>
    protected override void Die()
    {
        Debug.Log("Player Die");
        StopAllCoroutines();
        _anim.SetTrigger(hashDead);
        m_bisDead = true;

        OnPlayerDie(); //delegate event Play
        GameManager.instance.PlayerDie();
    }


    /// <summary> 체력 회복 작용 </summary>
    public void RecoveryHP(int value)
    {
        m_nCurHP += value;

        if (m_nCurHP > m_nMaxHP) m_nCurHP = m_nMaxHP;
        _playerUICtr.SetHPbar(m_nCurHP, m_nMaxHP);
    }

    /// <summary> 마나 회복 작용 </summary>
    public void RecoveryMP(int value)
    {
        m_nCurMP += value;

        if (m_nCurMP > m_nMaxHP) m_nCurMP = m_nMaxMP;
        _playerUICtr.SetMPbar(m_nCurMP, m_nMaxHP);
    }

    /// <summary> 경험치 획득 처리 및 레벨업 함수 호출 (사냥, 퀘스트 등등)</summary>
    public void SetEXP(int _exp)
    {
        this.m_nCurExp += _exp;
        while(m_nCurExp >= m_nMaxExp)
        {
            //초과분 만큼 빼준다.
            m_nCurExp -= m_nMaxExp;
            LevelUP();
        }
        _playerUICtr.SetEXPbar(m_nCurExp, m_nMaxExp);
    }

    /// <summary> 몬스터의 아이템을 Inventory로 전달 </summary>
    public void AddInven(Dictionary<ItemData, int> _itemDic)
    {
        foreach(var itemDic in _itemDic)
        {
            _itemInvenMgr.AddItem(itemDic.Key, itemDic.Value);
        }
    }

    /// <summary> AddInven 오버로딩 </summary>
    public void AddInven(ItemData itemData, int amount)
    {
        _itemInvenMgr.AddItem(itemData, amount);
    }


    /// <summary> 플레이어의 정보 저장하기 </summary>
    public void SavePlayer() => SaveSys.SavePlayer(this);

    /// <summary> 플레이어의 정보 로드 </summary>
    protected override void LoadData()
    {
        _playerData = SaveSys.LoadObject("PlayerData.Json");

        playerLevelData = SaveSys.LoadAllData().PlayerLevelDB;

        //저장된 데이터가 있다면 캐릭터 정보를 불러온다.
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
            // 저장된 데이터가 없다면, 1레벨 부터 시작
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

    /// <summary> 플레이어의 레벨업 및 캐릭터 정보 초기화 함수 </summary>
    /// <param name="value"> 현재 레벨을 넣으면 레벨업 후 정보를 불러 온다. </param>
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
                    StartCoroutine(_gameMgr.CompleteDialog(curQuest, i)); // 해당 퀘스트의 Dialog를 띄운다.
                    return;
                }
            }
        
            //현재가능한 퀘스트 목록 중
            var poQuestList = _questSystem.GetPossibleQuest();

            foreach (var quest in poQuestList)
            {
                //해당 NPC의 ID와 일치하는게 있다면
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

                    StartCoroutine(_gameMgr.PlayDialog(quest));//해당 퀘스트의 Dialog를 띄운다.
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
