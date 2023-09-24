using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MonsterRecord
{
    public int[] monsterRecords;
}

public class GameManager : MonoBehaviour
{
    //[SerializeField] private GameObject monObj;

    [Header("Monster Respawn Info")]
    [SerializeField] private float respawnDistance;
    //���� ���� ����
    [SerializeField, Header("������ ���� �ֱ�")] private float respawnTime;
    // ������ ������ �ִ� ����
    [SerializeField, Header("������ ���� ���� ������ �ִ� ��")] private int maxMonsters = 5;
    [SerializeField] private int currentMonsters;
    private int AllMonstersNum; //������ ��� ������ ��
    private bool m_bisPlayerDie; //�÷��̾ �׾��°�

    private int[] monsterRecords = new int[100];

    [SerializeField]
    private PlayerController _playerCtr;

    private ResourcesData _resourcesData;

    public static GameManager instance = null;
    private DialogSystem _dialogSystem;
    private QuestSystem _questSystem;


    [SerializeField] private DialogUI _dialogUI;
    [SerializeField] private QuestUI _questUI;

    private SkillManager _skillMgr;
    private ItemInventoryManager _itemIvenMgr;

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
        _itemIvenMgr = GetComponent<ItemInventoryManager>();

        _skillMgr.SetPlayerCtr(_playerCtr);
        _itemIvenMgr.SetPlayerCtr(_playerCtr);

        //skillManager Awake() ���� skinvenUI�� �Ҵ� �Ǿ���.(��α� �� ���)
        Skill_InvenUI skInvenUI = _skillMgr.GetSkInvenUI();
        _playerCtr.SetSkill_InvenUI(skInvenUI);

    }

    void Start()
    {

        InvisibleCursor();

        //StartCoroutine(UpdateMonster());
    }

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


    public ResourcesData GetResourcesData()
    {
        return _resourcesData;
    }


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

    public void AddCurrentMonsters(out int _monidx)
    {
        _monidx = AllMonstersNum;
        ++AllMonstersNum;
        ++currentMonsters;
        
    }

    public void SubCurrentMonsters()
    {
        --currentMonsters;
    }

    public void SetPlayerCtr(PlayerController playerCtr)
    {
        _playerCtr = playerCtr;
    }

    public void PlayerDie()
    {
        m_bisPlayerDie = true;
        Debug.Log("����");
    }

    public IEnumerator CheckQuest(QuestData questData)
    {
        QuestObjectives questObjectives = (QuestObjectives)questData.eObjectives;


        switch (questObjectives)
        {
            case QuestObjectives.CONVERSATION:
                break;
            case QuestObjectives.HUNT:
                while (questData.nGoalCnt > 0)
                {

                    yield return null;
                }
                break;
            case QuestObjectives.SKILL:
                break;
        }

        yield return null;

    }

    public List<QuestData> GetPossibleQuest()
    {
        return _questSystem.GetPossibleQuest();
    }

    /// <summary>
    /// NPC�� ������ ��� �� ���̴�.
    /// </summary>
    public IEnumerator UnsolvedQuestDialog(int id)
    {
        _dialogUI.SetDialogList(id, 0);
        yield return new WaitUntil(() => _dialogUI.UpdateDialog());
    }

    public IEnumerator PlayDialog(QuestData questData)
    {
        int id = questData.nID;
        int branch = questData.nBranch;

        Debug.Log($"{id}, {branch}");

        _dialogUI.SetDialogList(id, branch);
        yield return new WaitUntil(() => _dialogUI.UpdateDialog());

        _playerCtr.AddPlayerQuest(questData);

        var currentQuest = _playerCtr.GetPlayerQuest();

        _questUI.UpdateQuestUI(currentQuest);
        //_questSystem.CompleteQuest(questData);

        //UpdateQuest();
    }

    /// <summary> ������ �������� Inventory�� ���� </summary>
    public void AddInven(Dictionary<ItemData, int> _itemDic)
    {
        foreach (var itemDic in _itemDic)
        {
            _itemIvenMgr.Add(itemDic.Key, itemDic.Value);
        }

    }

    public void UpdateQuestList(int playerLevel) => _questSystem.UpdateQuestList(playerLevel);

    public DialogSystem GetDialogSystem() =>  _dialogSystem;
    public QuestSystem GetQuestSystem() => _questSystem;
}
