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

    private bool m_bisPlayerDie; //�÷��̾ �׾��°�
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
    /// <summary> �÷��̾� ����Ʈ ����Ʈ �Ҵ� </summary>
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
    /// <summary> 1. Find�Լ��� ����ؼ� ������Ʈ�� �Ҵ�  </summary>
    private void Init_FindObj()
    {
        _playerCtr = FindObjectOfType<PlayerController>();
        _dialogUICtr = FindObjectOfType<DialogUI>();
        _questUICtr = FindObjectOfType<QuestUI>();
    }
    /// <summary> 2. AllData �ε� �� �Ҵ� </summary>
    private void Init_DataSystem()
    {
        _allData = SaveSys.LoadAllData();
        _resourcesData = new ResourcesData(_allData);
        _dialogSystem = new DialogSystem(_allData.DialogDB);
        _questSystem = new QuestSystem(_allData.QuestDB);
    }
    /// <summary>
    /// 3. GameManger Obj�� ���ϰ� �ִ� Manager Script�� �ʱ�ȭ 
    /// <para> PlayerController���� ���� ���� �Ǿ�� �� </para>
    /// </summary>
    private void Init_AllManager()
    {
        //GetComponet
        _skillMgr = GetComponent<Skill_InvenManager>();
        _itemInvenMgr = GetComponent<ItemInventoryManager>();
        _merChantInvenMgr = GetComponent<MerChantInventoryManager>();
        _playerStatMgr = GetComponent<PlayerStatManager>();

        //�÷��̾� ��ų �Ŵ���
        _skillMgr.SetPlayerCtr(_playerCtr);
        _skillMgr.Init_Skills(_allData.SkillDB, _allData.PlayerSkillDB); //��ų ����
        //�÷��̾� ������ �Ŵ���
        _itemInvenMgr.SetPlayerCtr(_playerCtr);
        _itemInvenMgr.SetPlayerStatMgr(_playerStatMgr);

        //�÷��̾� ���� �Ŵ���
        _playerStatMgr.SetItemInvenMgr(_itemInvenMgr);
        _playerStatMgr.SetPlayerCtr(_playerCtr);

        //���� ������ �Ŵ���
        _merChantInvenMgr.SetPlayerCtr(_playerCtr);
        _merChantInvenMgr.SetItemInvenMgr(_itemInvenMgr);
    }
    /// <summary>  4. PlayerCotontroller �ʱ�ȭ </summary>
    private void Init_PlayerController()
    {
        _playerCtr.SetQuestSystem(_questSystem);
        _playerCtr.SetGameManager(this);
        _playerCtr.SetItemInvenManager(_itemInvenMgr);
        _playerCtr.SetSkillMgr(_skillMgr);
        _playerCtr.SetMerChantMgr(_merChantInvenMgr);
        _playerCtr.SetPlayerUIMgr(_playerStatMgr);

        //�÷��̾ ���ԵǾ� �ִ� ����Ʈ ��ũ��Ʈ ���� ��, ������ �Ҵ�
        var playerAnimEffectCtr = _playerCtr.gameObject.GetComponent<AnimationEventEffects>();
        playerAnimEffectCtr.SetEffectData(_allData.SkillEffectDB);
    }
    /// <summary> 5. ����ƮUI�� ������ �Ҵ� </summary>
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

    //���͸� ����� ��
    public void MonsterDead(int nMonsterID)
    {
        int monindex = nMonsterID - 100;
        //���͸� ���� ���� ���(���� �̺�Ʈ�� ���)
        monsterRecords[monindex]++;

        if (_playerQuestList == null) return;

        for (int i = 0; i < _playerQuestList.Count; i++)
        {
            if (_playerQuestList[i].nDestID == nMonsterID)
            {
                _playerQuestList[i].nCurCnt++;


                //��ǥ ������ �������� ��,
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
    /// ����Ʈ �Ϸ� ó��
    /// </summary>
    public void CompleteQuest(QuestData currentQuestData, int index)
    {
        var nextQuest = _questSystem.TryGetNextQuest(currentQuestData);

        //null�̶�� ���� ����Ʈ�� ���� �Ϸ�� ����Ʈ�̱� ������, ���� ���� ����Ʈ���� ����
        if (nextQuest == null)
        {
            _playerCtr.SetEXP(currentQuestData.nRewardEXP);    //����ġ ����
            _playerCtr.AddMoney(currentQuestData.nRewardGold); //�� ����

            //�������� ������ŭ �����۰� ���� ����
            for (int i = 0; i < currentQuestData.nRewardItem.Length; i++)
            {
                int nID = currentQuestData.nRewardItem[i];
                int nAmount = currentQuestData.nItemAmount[i];
                _playerCtr.AddInven(_resourcesData.GetItem(nID), nAmount);
            }

            //����Ʈ�� �Ϸ�ó�� �� QuestUI ������Ʈ
            //�̹� ������ questDate�� isCompelte true�� ����� ������ Update�� ������ ��ģ��.
            UpdateQuestUI(_playerQuestList);

            _playerQuestList.Remove(currentQuestData);
        }
        else
        {
            //�ٲ� �����͸� PlayerController QuestList �����Ϳ��� ������ �ֱ� ����
            //���� ���� ��ü(���� ����Ʈ)�� ���� �ּҿ� ���� �־��ش�.
            _playerQuestList[index] = nextQuest;

            UpdateQuestUI(_playerQuestList);
        }

        //����Ʈ ����ܰ谡 ������ �Ǹ� NPC�鿡�� ����Ʈ ���� �����ؼ� Update ��Ų��.
        UpdateNPCQuest();
    }
    /// <summary> ������ �������� Inventory�� ���� </summary>
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
    /// NPC�� ������ ��� �� ���̴�.
    /// </summary>
    public IEnumerator UnsolvedQuestDialog(int nQuestID)
    {
        _dialogUICtr.UpdateDialogList(nQuestID, 0);
        _playerCtr.SetUsePlayDialog(true); // �÷��̾� ���� ����
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());
        _playerCtr.SetUsePlayDialog(false); // �÷��̾� ���� ����
    }

    /// <summary>
    /// ���� ����Ʈ �Ϸ� ���̾�α� Ȱ��ȭ �� ����Ʈ �Ϸ� ���� ó��
    /// </summary>
    public IEnumerator CompleteDialog(QuestData questData, int index)
    {
        _playerCtr.SetUsePlayDialog(true); // �÷��̾� ���� ����

        int questId = questData.nQuestID;
        int branch = questData.nBranch;

        //�ش� QuestID, brach�� �´� ä�� ���̷α� ����
        _dialogUICtr.UpdateDialogList(questId, branch);
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());

        _playerCtr.SetUsePlayDialog(false); // �÷��̾� ���� ����
        CompleteQuest(questData, index);
    }
    /// <summary>
    /// ����Ʈ�� ���� NPC�� Dialog�� �� �� �ִ�.
    /// ���ÿ� ����Ʈ�� �����ϰ� ���� ����Ʈ UI�� ������Ʈ �Ѵ�.
    /// </summary>
    /// <param name="questData"> �ش� ����Ʈ�� ID, Branch�� Dialog�� ID, Branch ��</param>
    public IEnumerator PlayDialog(QuestData questData)
    {
        _playerCtr.SetUsePlayDialog(true); // �÷��̾� ���� ����

        int questId = questData.nQuestID;
        int branch = questData.nBranch;

        //�ش� QuestID, brach�� �´� ä�� ���̷α� ����
        _dialogUICtr.UpdateDialogList(questId, branch);
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());

        _playerCtr.SetUsePlayDialog(false); // �÷��̾� ���� ����

        //��ȭ�� ������ �÷��̾ �����ϴ� ����Ʈ ��Ͽ� �߰�
        // Yes, No �˾��� ����� ���� / ���� �ص� �ȴ�.
        _playerCtr.AddPlayerQuest(questData);
        _playerQuestList = _playerCtr.GetPlayerQuestList();

        //�ܼ� ��ȭ��ǥ��� �Ϸ�ó��
        if(questData.eObjectives == QuestObjectives.CONVERSATION)
        {
            int index = _playerQuestList.IndexOf(questData);
            CompleteQuest(questData, index);
        }

        UpdateQuestUI(_playerQuestList);
    }

    #endregion

}
