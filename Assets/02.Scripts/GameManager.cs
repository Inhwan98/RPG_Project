using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MonsterRecord
{
    public int[] monsterRecords;
}

public class GameManager : MonoBehaviour
{
    private bool m_bisPlayerDie; //�÷��̾ �׾��°�

    private int[] monsterRecords = new int[100];

    [SerializeField]
    private PlayerController _playerCtr;
    private List<QuestData> _playerQuestList;

    private ResourcesData _resourcesData;

    public static GameManager instance = null;
    private DialogSystem _dialogSystem;
    private QuestSystem _questSystem;


    [SerializeField] private DialogUI _dialogUICtr;
    [SerializeField] private QuestUI _questUICtr;

    private SkillManager _skillMgr;
    private ItemInventoryManager _itemInvenMgr;

    private void Awake()
    {
        #region SingleTone
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        #endregion
        _resourcesData = new ResourcesData();
        _dialogSystem = new DialogSystem();
        _questSystem = new QuestSystem();
        

          _skillMgr = GetComponent<SkillManager>();
        _itemInvenMgr = GetComponent<ItemInventoryManager>();

        _skillMgr.SetPlayerCtr(_playerCtr);
        _itemInvenMgr.SetPlayerCtr(_playerCtr);

        //skillManager Awake() ���� skinvenUI�� �Ҵ� �Ǿ���.(��α� �� ���)
        Skill_InvenUI skInvenUI = _skillMgr.GetSkInvenUI();

        _playerCtr.SetSkill_InvenUI(skInvenUI);
        _playerCtr.SetGameManager(this);
        _playerCtr.SetItemInvenManager(_itemInvenMgr);
        _playerCtr.SetSkillMgr(_skillMgr);
        _playerCtr.SetQuestSystem(_questSystem);

    }

    void Start()
    {

        InvisibleCursor();

        //StartCoroutine(UpdateMonster());
    }

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

    ////���� �����ֱ�
    //IEnumerator UpdateMonster()
    //{
    //    while(!m_bisPlayerDie)
    //    {
    //        float randRotNum = Random.Range(0, 360.0f);
    //        Quaternion randRot = Quaternion.Euler(playerTr.up * randRotNum);
    //        //Vector3 randPos = randRot * (playerTr.forward * respawnDistance);

    //        //�÷��̾� �ֺ����� respawnDistance ��ŭ ������. ������ ����
    //        Vector3 randPos = playerTr.rotation * randRot * (playerTr.forward * respawnDistance);
    //        GameObject mon = Instantiate(monObj, playerTr.position + randPos, Quaternion.identity);
    //        playerCtr.AddMonsterObjs(AllMonstersNum-1, mon.transform); // 07 26

    //        yield return new WaitForSeconds(respawnTime);
    //        yield return new WaitUntil(() => currentMonsters < maxMonsters); // �ִ� ���� ���� ������ �����ϸ� ���
    //    }
    //}

    //������ ����
    public void AddCurrentMonsters(int nMonsterID)
    {
         
    }


    public DialogSystem GetDialogSystem() => _dialogSystem;
    public QuestSystem GetQuestSystem() => _questSystem;
    public ResourcesData GetResourcesData() => _resourcesData;

    public void SetPlayerCtr(PlayerController playerCtr) => _playerCtr = playerCtr;

    public void UpdateQuestList(int m_nLevel) => _questSystem.UpdateQuestList(m_nLevel);

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
                    _questUICtr.UpdateQuestUI(_playerQuestList[i]);
            }
        }
    }


    public void PlayerDie()
    {
        m_bisPlayerDie = true;
        Debug.Log("����");
    }

    //9.24 ������
    public IEnumerator CheckQuest(QuestData questData)
    {
        QuestObjectives questObjectives = (QuestObjectives)questData.eObjectives;


        switch (questObjectives)
        {
            case QuestObjectives.CONVERSATION:
                break;
            case QuestObjectives.HUNT:
                
                int monindex = questData.nDestID - 100; // monster�� ID�� 100�� ���� ����.

                //����Ʈ�� ��ǥ ������ �ش�ID ���͸� ���� ī��Ʈ�� �ٸ��ٸ� �ڷ�ƾ ��� �ݺ�
                //questData.nCurCnt�� ���� �����
                while (questData.nGoalCnt > questData.nCurCnt)
                {



                    yield return null;
                }
                break;
            case QuestObjectives.SKILL:
                break;
        }

        yield return null;
    }


    /// <summary>
    /// NPC�� ������ ��� �� ���̴�.
    /// </summary>
    public IEnumerator UnsolvedQuestDialog(int id)
    {
        _dialogUICtr.SetDialogList(id, 0);
        _playerCtr.SetUsePlayDialog(true); // �÷��̾� ���� ����
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());
        _playerCtr.SetUsePlayDialog(false); // �÷��̾� ���� ����
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
        _dialogUICtr.SetDialogList(questId, branch);
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());

        _playerCtr.SetUsePlayDialog(false); // �÷��̾� ���� ����

        //��ȭ�� ������ �÷��̾ �����ϴ� ����Ʈ ��Ͽ� �߰�
        // Yes, No �˾��� ����� ���� / ���� �ص� �ȴ�.
        _playerCtr.AddPlayerQuest(questData); 

        _playerQuestList = _playerCtr.GetPlayerQuestList();

        _questUICtr.UpdateQuestUI(_playerQuestList);
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
        _dialogUICtr.SetDialogList(questId, branch);
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());

        _playerCtr.SetUsePlayDialog(false); // �÷��̾� ���� ����
        CompleteQuest(questData, index);
    }

    /// <summary>
    /// ����Ʈ �Ϸ� ó��
    /// </summary>
    public void CompleteQuest(QuestData currentQuestData, int index)
    {
        var nextQuest = _questSystem.NextQuest(currentQuestData);

        //null�̶�� ���� ����Ʈ�� ���� �Ϸ�� ����Ʈ�̱� ������, ���� ���� ����Ʈ���� ����
        if (nextQuest == null)
        {
            Debug.Log("������ �ִ� ����");
            _playerCtr.SetEXP(currentQuestData.nRewardEXP); //����ġ ����


            _questUICtr.UpdateQuestUI(_playerQuestList);
            _playerQuestList.Remove(currentQuestData);
        }
        else
        {
            //�ٲ� �����͸� PlayerController QuestList �����Ϳ��� ������ �ֱ� ����
            //���� ���� ��ü(���� ����Ʈ)�� ���� �ּҿ� ���� �־��ش�.
            _playerQuestList[index] = nextQuest;
            _questUICtr.UpdateQuestUI(_playerQuestList);
        }
    }


    /// <summary> ������ �������� Inventory�� ���� </summary>
    public void AddInven(Dictionary<ItemData, int> _itemDic)
    {
        foreach (var itemDic in _itemDic)
        {
            _itemInvenMgr.AddItem(itemDic.Key, itemDic.Value);
        }
    }



}
