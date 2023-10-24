using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    /******************************************
     *               Fields
     ******************************************/

    #region static Fields
    public static GameManager instance = null;
    #endregion

    #region private Fields
    private PlayerController _playerCtr;
    private List<QuestData> _playerQuestList;

    private AllData _allData;
    private ResourcesData _resourcesData;

    private DialogSystem _dialogSystem;
    private QuestSystem _questSystem;
    private DialogUI _dialogUICtr;
    private QuestUI _questUICtr;

    #region Inven Manager
    private Skill_InvenManager _skillMgr;
    private ItemInventoryManager _itemInvenMgr;
    private MerChantInventoryManager _merChantInvenMgr;
    private PlayerStatManager _playerStatMgr;
    #endregion

    private bool m_bisPlayerDie; //플레이어가 죽었는가
    private int[] monsterRecords = new int[100];

    #endregion


    /******************************************
     *         Get, Set, Update Methods
     ******************************************/

    #region Get Methods
    public DialogSystem GetDialogSystem() => _dialogSystem;
    public QuestSystem GetQuestSystem() => _questSystem;
    public ResourcesData GetResourcesData() => _resourcesData;
    public MerChantInventoryManager GetMerChantInvenMgr() => _merChantInvenMgr;
    public AllData GetAllData() => _allData;
    #endregion

    #region Set Methods
    /// <summary> 플레이어 퀘스트 리스트 할당 </summary>
    public void SetPlayerQuestList(List<QuestData> playerQuestList) => _playerQuestList = playerQuestList;
    public void SetPlayerCtr(PlayerController playerCtr) => _playerCtr = playerCtr;
    #endregion

    #region Update Methods
    public void UpdateQuestUI(List<QuestData> playerQuestList) => _questUICtr.UpdateQuestUI(playerQuestList);
    public void UpdateQuestUI(QuestData playerQuestData) => _questUICtr.UpdateQuestUI(playerQuestData);
    public void UpdateQuestList(int m_nLevel) => _questSystem.UpdateQuestList(m_nLevel);
    /// <summary> NPC Quest Update </summary>
    private void UpdateNPCQuest() => _playerCtr.UpdateNPCQuest();
    #endregion


    /******************************************
     *               Unity Event
     ******************************************/
    #region Unity Event
    private void Awake()
    {
        #region SingleTone
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        //DontDestroyOnLoad(this.gameObject);
        #endregion

        Init_FindObj();
        Init_DataSystem();
        Init_AllManager();
        Init_PlayerController();
        Init_QuestUI();
    }

    void Start()
    {
        PlayerController.OnPlayerDie += PlayerDie;
        InvisibleCursor();
    }

    private void OnDestroy()
    {
        SaveAllData();
        instance = null;
    }

    #endregion

    /******************************************
     *               Methods
     ******************************************/
    #region private Methods
    /// <summary> 1. Find함수를 사용해서 오브젝트를 할당  </summary>
    private void Init_FindObj()
    {
        _playerCtr = FindObjectOfType<PlayerController>();
        _dialogUICtr = FindObjectOfType<DialogUI>();
        _questUICtr = FindObjectOfType<QuestUI>();
    }
    /// <summary> 2. AllData 로드 및 할당 </summary>
    private void Init_DataSystem()
    {
        _allData = SaveSys.LoadAllData();
        _resourcesData = new ResourcesData(_allData);
        _dialogSystem = new DialogSystem(_allData.DialogDB);
        _questSystem = new QuestSystem(_allData.QuestDB);
    }
    /// <summary>
    /// 3. GameManger Obj가 지니고 있는 Manager Script들 초기화 
    /// <para> PlayerController보다 먼저 선언 되어야 함 </para>
    /// </summary>
    private void Init_AllManager()
    {
        //GetComponet
        _skillMgr = GetComponent<Skill_InvenManager>();
        _itemInvenMgr = GetComponent<ItemInventoryManager>();
        _merChantInvenMgr = GetComponent<MerChantInventoryManager>();
        _playerStatMgr = GetComponent<PlayerStatManager>();

        //플레이어 스킬 매니져
        _skillMgr.SetPlayerCtr(_playerCtr);
        _skillMgr.Init_Skills(_allData.SkillDB, _allData.PlayerSkillDB); //스킬 세팅
        //플레이어 아이템 매니져
        _itemInvenMgr.SetPlayerCtr(_playerCtr);
        _itemInvenMgr.SetPlayerStatMgr(_playerStatMgr);

        //플레이어 스탯 매니져
        _playerStatMgr.SetItemInvenMgr(_itemInvenMgr);
        _playerStatMgr.SetPlayerCtr(_playerCtr);

        //상인 아이템 매니져
        _merChantInvenMgr.SetPlayerCtr(_playerCtr);
        _merChantInvenMgr.SetItemInvenMgr(_itemInvenMgr);
    }
    /// <summary>  4. PlayerCotontroller 초기화 </summary>
    private void Init_PlayerController()
    {
        _playerCtr.SetQuestSystem(_questSystem);
        _playerCtr.SetGameManager(this);
        _playerCtr.SetItemInvenManager(_itemInvenMgr);
        _playerCtr.SetSkillMgr(_skillMgr);
        _playerCtr.SetMerChantMgr(_merChantInvenMgr);
        _playerCtr.SetPlayerUIMgr(_playerStatMgr);

        //플레이어에 포함되어 있는 이펙트 스크립트 참조 후, 데이터 할당
        var playerAnimEffectCtr = _playerCtr.gameObject.GetComponent<AnimationEventEffects>();
        playerAnimEffectCtr.SetEffectData(_allData.SkillEffectDB);
    }
    /// <summary> 5. 퀘스트UI로 데이터 할당 </summary>
    private void Init_QuestUI() => _questUICtr.QuestUI_Init(_resourcesData);
    private void SaveAllData()
    {
        _allData.QuestDB = _questSystem.GetQuestDataArray();
        _allData.SkillDB = _skillMgr.GetInvenAllSkillData();
        _allData.PlayerSkillDB = _skillMgr.GetPlayerAllSkillData();

        SaveSys.SaveAllData(_allData);
    }
    #endregion

    #region public Methods

    #region Cursor Controll
    public void InvisibleCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void VisibleCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    //몬스터를 잡았을 때
    public void MonsterDead(int nMonsterID)
    {
        int monindex = nMonsterID - 100;
        //몬스터를 잡은 갯수 기록(업적 이벤트에 사용)
        monsterRecords[monindex]++;

        if (_playerQuestList == null) return;

        for (int i = 0; i < _playerQuestList.Count; i++)
        {
            if (_playerQuestList[i].nDestID == nMonsterID)
            {
                _playerQuestList[i].nCurCnt++;


                //목표 개수에 도달했을 시,
                if (_playerQuestList[i].nCurCnt >= _playerQuestList[i].nGoalCnt)
                {
                    CompleteQuest(_playerQuestList[i], i);
                    break;
                }
                else
                    UpdateQuestUI(_playerQuestList[i]);
            }
        }
    }
    public void PlayerDie()
    {
        VisibleCursor();
        m_bisPlayerDie = true;
    }
    /// <summary>
    /// 퀘스트 완료 처리
    /// </summary>
    public void CompleteQuest(QuestData currentQuestData, int index)
    {
        var nextQuest = _questSystem.TryGetNextQuest(currentQuestData);

        //null이라면 다음 퀘스트가 없는 완료된 퀘스트이기 때문에, 현재 진행 퀘스트에서 제거
        if (nextQuest == null)
        {
            _playerCtr.SetEXP(currentQuestData.nRewardEXP);    //경험치 전달
            _playerCtr.AddMoney(currentQuestData.nRewardGold); //돈 전달

            //아이템의 개수만큼 아이템과 수량 전달
            for (int i = 0; i < currentQuestData.nRewardItem.Length; i++)
            {
                int nID = currentQuestData.nRewardItem[i];
                int nAmount = currentQuestData.nItemAmount[i];
                _playerCtr.AddInven(_resourcesData.GetItem(nID), nAmount);
            }

            //퀘스트를 완료처리 후 QuestUI 업데이트
            //이미 위에서 questDate의 isCompelte true를 해줬기 때문에 Update에 영향을 미친다.
            UpdateQuestUI(_playerQuestList);

            _playerQuestList.Remove(currentQuestData);
        }
        else
        {
            //바뀐 데이터를 PlayerController QuestList 데이터에도 영향을 주기 위해
            //새로 생긴 객체(다음 퀘스트)를 참조 주소에 직접 넣어준다.
            _playerQuestList[index] = nextQuest;

            UpdateQuestUI(_playerQuestList);
        }

        //퀘스트 진행단계가 마무리 되면 NPC들에게 퀘스트 정보 전달해서 Update 시킨다.
        UpdateNPCQuest();
    }
    /// <summary> 몬스터의 아이템을 Inventory로 전달 </summary>
    public void AddInven(ItemData itemData, int amount = 1)
    {
        _itemInvenMgr.AddItem(itemData, amount);
    }
    
    #endregion

    /******************************************
     *               Coroutine
     ******************************************/

    #region Coroutine
    /// <summary>
    /// NPC의 불평을 듣게 될 것이다.
    /// </summary>
    public IEnumerator UnsolvedQuestDialog(int nQuestID)
    {
        _dialogUICtr.UpdateDialogList(nQuestID, 0);
        _playerCtr.SetUsePlayDialog(true); // 플레이어 동작 관리
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());
        _playerCtr.SetUsePlayDialog(false); // 플레이어 동작 관리
    }

    /// <summary>
    /// 현재 퀘스트 완료 다이어로그 활성화 및 퀘스트 완료 진행 처리
    /// </summary>
    public IEnumerator CompleteDialog(QuestData questData, int index)
    {
        _playerCtr.SetUsePlayDialog(true); // 플레이어 동작 관리

        int questId = questData.nQuestID;
        int branch = questData.nBranch;

        //해당 QuestID, brach에 맞는 채팅 다이로그 시작
        _dialogUICtr.UpdateDialogList(questId, branch);
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());

        _playerCtr.SetUsePlayDialog(false); // 플레이어 동작 관리
        CompleteQuest(questData, index);
    }
    /// <summary>
    /// 퀘스트에 대한 NPC의 Dialog를 볼 수 있다.
    /// 동시에 퀘스트를 수락하고 진행 퀘스트 UI를 업데이트 한다.
    /// </summary>
    /// <param name="questData"> 해당 퀘스트의 ID, Branch와 Dialog의 ID, Branch 비교</param>
    public IEnumerator PlayDialog(QuestData questData)
    {
        _playerCtr.SetUsePlayDialog(true); // 플레이어 동작 관리

        int questId = questData.nQuestID;
        int branch = questData.nBranch;

        //해당 QuestID, brach에 맞는 채팅 다이로그 시작
        _dialogUICtr.UpdateDialogList(questId, branch);
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());

        _playerCtr.SetUsePlayDialog(false); // 플레이어 동작 관리

        //대화가 끝나면 플레이어가 진행하는 퀘스트 목록에 추가
        // Yes, No 팝업을 띄워서 수락 / 거절 해도 된다.
        _playerCtr.AddPlayerQuest(questData);
        _playerQuestList = _playerCtr.GetPlayerQuestList();

        //단순 대화목표라면 완료처리
        if(questData.eObjectives == QuestObjectives.CONVERSATION)
        {
            int index = _playerQuestList.IndexOf(questData);
            CompleteQuest(questData, index);
        }

        UpdateQuestUI(_playerQuestList);
    }

    #endregion

}
