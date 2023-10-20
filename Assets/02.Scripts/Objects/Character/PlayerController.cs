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

    public delegate void PlayerQuestSetting();             //델리게이트 선언
    public static event PlayerQuestSetting OnSettingNPCQuest;    //이벤트 선언

    [SerializeField] private float moveSpeed = 5.0f; // 이동 속도
    #region Anim Hashing Init
    private readonly int hashTakeDamage  = Animator.StringToHash("DoTakeDamage");
    private readonly int hashStrafe      = Animator.StringToHash("Strafe");
    private readonly int hashForward     = Animator.StringToHash("Forward");
    private readonly int hashRun         = Animator.StringToHash("Run");
    private readonly int hashMoveSpeed   = Animator.StringToHash("MoveSpeed");
    private readonly int hashAttackSpeed = Animator.StringToHash("AttackSpeed");
    private readonly int hashRolling     = Animator.StringToHash("IsRolling");
    private readonly int[] hashAttack = new int[] { Animator.StringToHash("DoAttack1"), Animator.StringToHash("DoAttack2"), Animator.StringToHash("DoAttack3")};
    #endregion

    #region Attack 관련
    [SerializeField] private float m_fAttackDelay = 0.5f;
    private float m_fAttackRunTime = 0.0f;
    private float m_fAttackInitTime = 1.5f;
    private int m_nAttackCount = 0;
    #endregion

    #region GetComponent 초기화 항목
    private Rigidbody _rigid;
    private Weapon _weaponCtr;
    #endregion

    private CameraController _cameraCtr;

    private PlayerLevelData[] _playerLevelData;

    private string m_sCurVillageScene; //현재 플레이어가 있는 씬 정보

    private int m_nRubyAmount; //게임 머니 양

    private float _xInput;
    private float _zInput;
    private float _runInput;
    private bool _rollingInput;
    private bool _attackInput;

    private bool _isRolling;
    private bool _isRollingCoolTime; // 구르기 재시전 시간
    private bool _isPlayerDown;
    private bool _isUseInven;
    private bool _isUseSkillWindow;
    private bool _isUseStatusWindow;
    private bool _isPlayDialog;

    private List<QuestData> _currentQuestList = new List<QuestData>(); //수락한 퀘스트 목록
   
    private QuestSystem _questSystem;

    //매니져 스크립트
    private GameManager _gameMgr;
    private PlayerUIManager _playerUIMgr;
    private ItemInventoryManager _itemInvenMgr;
    private MerChantInventoryManager _merChantInvenMgr;


    private PlayerData _playerData;

    #region Get, Set Method
    public int GetRubyAmount() => m_nRubyAmount;
    public bool GetUseInven() => _isUseInven;
    public CameraController GetCameraCtr() => _cameraCtr;
    public List<QuestData> GetPlayerQuestList() => _currentQuestList;
    public QuestSystem GetQuestSystem() => _questSystem;
    public string GetPlayerStayingVilliage() => m_sCurVillageScene;
    /// <summary> 플레이어 행동 중지 조건 </summary>
    private bool GetFreezePlayer() => _isUseInven || _isUseSkillWindow || _isPlayDialog || m_bisAttack || _isPlayerDown;

    public void SetUseItemInven(bool value) { _isUseInven = value; }                                    // 인벤토리창이 켜져 있는가
    public void SetUseSKill_Inven(bool value) { _isUseSkillWindow = value; }                            // 스킬창이 켜져있는가
    public void SetUsePlayDialog(bool value) { _isPlayDialog = value; }                                 // 다이얼로그 사용중인가  
    public void SetQuestSystem(QuestSystem questSystem) => _questSystem = questSystem;                  // 퀘스트 시스템
    public void SetGameManager(GameManager gameMgr) => _gameMgr = gameMgr;                              // 게임 매니져
    public void SetItemInvenManager(ItemInventoryManager itemInvenMgr) => _itemInvenMgr = itemInvenMgr; // 아이템 인벤토리 매니져
    public void SetSkillMgr(SkillManager skillMgr) => _skillMgr = skillMgr;                             // 스킬 매니져
    public void SetPlayerUIMgr(PlayerUIManager playerUIMgr) => _playerUIMgr = playerUIMgr;              // 플레이어 UI 매니져
    public void SetMerChantMgr(MerChantInventoryManager merChantMgr) => _merChantInvenMgr = merChantMgr;
    public void SetCameraCtr(CameraController value) => _cameraCtr = value;                             // 카메라
    public void SetVillageScene(string name) => m_sCurVillageScene = name;
    #endregion

    public void UpdateNPCQuest()
    {
        if(OnSettingNPCQuest != null) OnSettingNPCQuest();
    }

    PlayerStatusData _playerStatData;

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

        //공격속도
        _anim.SetFloat(hashAttackSpeed, m_fAttackDelay);

        _playerStatData = new PlayerStatusData(this);
        
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
        UpdateQuest(m_nLevel); //플레이어 레벨 기준 퀘스트 정보 업데이트
        _currentQuestList = _questSystem.GetInProgressQuestList(); //진행중인 퀘스트 받아오기
        CurrentQuestInit(_currentQuestList);

        UpdateNPCQuest();

        PlayerUI_Init();

        _itemInvenMgr.UpdateRubyAmount(m_nRubyAmount);

        base.Start(); //skill_List 가 부모에서 초기화 됌
    }


    private void FixedUpdate()
    {
        CharacterMovement();
    }

    /// <summary> BossAttack관련 충돌 처리 </summary>
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
    bool isComplete = false;  //완료 퀘스트 여부
    bool isInprogress = false;  //진행 중인 퀘스트 여부
    bool isPossible = false;  //가능한 퀘스트 여부
    /// <summary> NPC 상호작용 처리</summary>
    private void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {

            //1. Npc 만났을 때 최초 초기화 정보
            if (npcCtr == null)
            {
                npcCtr = coll.gameObject?.GetComponent<NPC>();
                Debug.Assert(npcCtr != null, "npcCtr is NULL");

                Vector3 pos = this.transform.position + (Vector3.up * 1.5f) + (transform.right * 0.5f);

                isComplete = npcCtr.GetIsCompleteQuest();
                isInprogress = npcCtr.GetIsInProgressQuest();
                isPossible = npcCtr.GetIsPossibleQuest();


                //이벤트 퀘스트 있을 때만 상호작용키 활성화
                if (isPossible || isInprogress || isComplete || npcCtr is MerChantNPC)
                    _playerUIMgr.ConversationKeyActiveOn(pos);
            }


            //2.NPC와 상호작용하기
            if (Input.GetKeyDown(KeyCode.F) && !_isPlayDialog)
            {
                _playerUIMgr.ConversationKeyActiveOff();
                PlayerIdleState();

                #region 퀘스트 상호작용
                //1. 완료 퀘스트가 있을 때
                if (isComplete)
                {
                    QuestData quest = npcCtr.GetQuestData();
                    int index = npcCtr.GetCompleteQuestIndex();
                    StartCoroutine(_gameMgr.CompleteDialog(quest, index)); // 해당 퀘스트의 Dialog를 띄운다.
                }
                //2. 진행 중인 퀘스트가 있을 때
                else if(isInprogress)
                {
                    int unsolvedIndex = npcCtr.GetUnSolvedQuestID();
                    StartCoroutine(_gameMgr.UnsolvedQuestDialog(unsolvedIndex));
                }
                //3. 진행 가능한 퀘스트가 있을 때
                else if (isPossible)
                {
                    QuestData quest = npcCtr.GetQuestData();
                    StartCoroutine(_gameMgr.PlayDialog(quest));//해당 퀘스트의 Dialog를 띄운다.

                }
                #endregion

                if(npcCtr is MerChantNPC merCtr)
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
            _playerUIMgr.ConversationKeyActiveOff(); //상호작용키 비가시화

            if (npcCtr is MerChantNPC merCtr)
            {
                merCtr.MerChantInvenOff();
                _itemInvenMgr.SetWindowActive(false);
                _merChantInvenMgr.SetWindowActive(false);
            }

            isComplete = false;  //완료 퀘스트 여부
            isInprogress = false;  //진행 중인 퀘스트 여부
            isPossible = false;  //가능한 퀘스트 여부
            npcCtr = null;
        }
    }

    private void Update()
    {
        Input_init();

        if(m_fAttackInitTime > m_fAttackRunTime) m_fAttackRunTime += Time.deltaTime;

        StatusWindowActiveButton();
        PlayerAttack();
    }

    #region Init
    /// <summary> Player Info UI Set </summary>
    private void PlayerUI_Init()
    {
        _playerUIMgr.SetHPbar(m_nCurHP, m_nMaxHP);
        _playerUIMgr.SetMPbar(m_nCurMP, m_nMaxMP);
        _playerUIMgr.SetEXPbar(m_nCurExp, m_nMaxExp);

        _playerUIMgr.SetPlayerStatusData(_playerStatData);
    }

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

        DoRolling(ref movementSpeed); // 이동속도 ref .. 구를시 더 빨라지게 하기 위해


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

    #region Rolling 관련
    /// <summary> shift + spacebar 누를 시 구르기 가능 </summary>
    public void DoRolling(ref float movementSpeed)
    {
        if (_rollingInput && _runInput > 0.5f && !_isRollingCoolTime)
        {
            StartCoroutine(CheckRolling());
            movementSpeed *= 1.5f;
            _anim.SetBool(hashRolling, true);
        }
        else
        {
            _anim.SetBool(hashRolling, false);
        }
    }

    /// <summary>
    /// 구르는 중인지에 대한 여부와 구르기 쿨타임 체크 함수
    /// Layer를 Ignore로 구를동안 바꿔준다.
    /// </summary>
    IEnumerator CheckRolling()
    {
        _isRolling = true;
        _isRollingCoolTime = true;

        gameObject.layer = 2;
        yield return new WaitForSeconds(1.0f);

        gameObject.layer = 3;
        _isRolling = false;

        yield return new WaitForSeconds(4.0f);
        _isRollingCoolTime = false;
    }
    #endregion
    #region Attack 관련
    /// <summary> Player의 기본/스킬 공격 구성 </summary>
    private void PlayerAttack()
    {
        if (GetFreezePlayer()) return;

        if (_attackInput) StartCoroutine(Attack());
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
            m_bisAttack = true;
            _weaponCtr.Use(); //무기 콜라이더 활성화 / 비활성화 루틴
            //기본공격의 시간이 공격초기화 시간보다 길어진다면 콤보 x
            if (m_fAttackRunTime > m_fAttackInitTime) m_nAttackCount = 0;
            _anim.SetTrigger(hashAttack[m_nAttackCount]);

            m_nAttackCount++;
            if (m_nAttackCount >= 3) m_nAttackCount = 0;

            m_fAttackRunTime = 0;
            //마지막 공격 후, 더 길게 대기
            if(m_nAttackCount == 0)
                yield return new WaitForSeconds(m_fAttackDelay + 1.0f);
            else
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
        SkillData curSkill = _skill_Datas[skill_Idx];

        //획득하지 않은 상태면
        if (curSkill == null) yield break;

        //사용 가능 상태가 아니면
        if (curSkill.GetInAvailable())
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
        _playerUIMgr.SetMPbar(m_nCurMP, m_nMaxMP);

        _skillMgr.StartSkillCoolTime(skill_Idx, curSkill);

        yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        m_bisAttack = false;
        //circualrQueue.DeQueue(); //스킬 순서 당겨 주기
    }
    #endregion

    /// <summary> 플레이어 IDLE화 </summary>
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
        Commandkey.CheckInput(ActionType.StatusWindow, ref _isUseStatusWindow, _playerUIMgr);

        #region 이전
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

    /// <summary> 컨트롤러가 잠겨야 하는지 체크 한다. </summary>
    public void CheckFreezController()
    {
        bool isWindowOn = IsActiveWindowOnState();

        GetCameraCtr().UseWindow(isWindowOn); //카메라의 회전 제어

        if (isWindowOn) GameManager.instance.VisibleCursor();
        else
            GameManager.instance.InvisibleCursor();
    }

    private bool IsActiveWindowOnState()
    {
        return _isUseInven || _isUseSkillWindow || _isUseStatusWindow;
    }

    /// <summary> 플레이어가 수락한 퀘스트 추가 </summary>
    public void AddPlayerQuest(QuestData questData)
    {
        questData.bIsProgress = true;
        _currentQuestList.Add(questData);
    }

    /// <summary> 플레이어 레벨에 비례해서 퀘스트 목록들 갱신 </summary>
    private void UpdateQuest(int nLevel) => _gameMgr.UpdateQuestList(nLevel);

    public override void Buff(int _str)
    {
        base.Buff(_str);
        _playerUIMgr.SetHPbar(m_nCurHP, m_nMaxHP); //HP Bar UI 수정
    }

    /// <summary> 플레이어 대미지 발생 이벤트 </summary>
    public override void OnDamage(int _str, bool _isKnockback = false, Transform posTr = null)
    {
        int nDamage = _str - m_nDefence;
        if (nDamage < 0) nDamage = 0;

        this.m_nCurHP -= nDamage;

        ActiveDamageText(nDamage);

        //if (m_fCurHP <= 0) this.objState = ObjectState.DEAD; //사망 없음

        if (_isKnockback == true)
        {
            if(posTr != null)
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

        if (m_nCurHP <= 0) m_nCurHP = 0;
        _playerUIMgr.SetHPbar(m_nCurHP, m_nMaxHP); //HP Bar UI 수정
    }
    /// <summary> 피격당한 이후, Down 상태 시간 체크 </summary>
    IEnumerator CheckDownTime()
    {
        _isPlayerDown = true;
        yield return new WaitForSeconds(1.5f);
        _isPlayerDown = false;
    }

    /// <summary> 플레이어의 사망 처리 </summary>
    protected override void Die()
    {
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
        _playerUIMgr.SetHPbar(m_nCurHP, m_nMaxHP);
    }

    /// <summary> 마나 회복 작용 </summary>
    public void RecoveryMP(int value)
    {
        m_nCurMP += value;

        if (m_nCurMP > m_nMaxHP) m_nCurMP = m_nMaxMP;
        _playerUIMgr.SetMPbar(m_nCurMP, m_nMaxHP);
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
        _playerUIMgr.SetEXPbar(m_nCurExp, m_nMaxExp);
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

    /// <summary>
    /// 플레이어 머니 추가
    /// </summary>
    public void AddMoney(int nMoney)
    {
        m_nRubyAmount += nMoney;
        _itemInvenMgr.UpdateRubyAmount(m_nRubyAmount);
    }


    /// <summary> 플레이어의 정보 저장하기 </summary>
    public void SavePlayer() => SaveSys.SavePlayer(this);

    /// <summary> 플레이어의 정보 로드 </summary>
    protected override void LoadData()
    {
        _playerData = SaveSys.LoadObject("PlayerData.Json");

        _playerLevelData = SaveSys.LoadAllData().PlayerLevelDB;

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

            m_nRubyAmount = _playerData.nRubyAmount;

            m_sCurVillageScene = _playerData.sVillageScene;

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

        UpdateNPCQuest();
        base.LevelUP();
    }

    /// <summary> 플레이어의 레벨업 및 캐릭터 정보 초기화 함수 </summary>
    /// <param name="value"> 현재 레벨을 넣으면 레벨업 후 정보를 불러 온다. </param>
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


    private void ClearStaticField()
    {
        instance = null;
        OnPlayerDie = null;
        OnSettingNPCQuest = null;
    }

    private void OnDestroy()
    {
        ClearStaticField();
        SavePlayer();
    }
}
