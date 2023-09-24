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
    //몬스터 생성 간격
    [SerializeField, Header("몬스터의 생성 주기")] private float respawnTime;
    // 생성할 몬스터의 최대 개수
    [SerializeField, Header("몬스터의 동시 생성 가능한 최대 수")] private int maxMonsters = 5;
    [SerializeField] private int currentMonsters;
    private int AllMonstersNum; //생성된 모든 몬스터의 수
    private bool m_bisPlayerDie; //플레이어가 죽었는가

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

        //skillManager Awake() 전에 skinvenUI는 할당 되었다.(드로그 앤 드랍)
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


    ////몬스터 생성주기
    //IEnumerator UpdateMonster()
    //{
    //    while(!m_bisPlayerDie)
    //    {
    //        float randRotNum = Random.Range(0, 360.0f);
    //        Quaternion randRot = Quaternion.Euler(playerTr.up * randRotNum);
    //        //Vector3 randPos = randRot * (playerTr.forward * respawnDistance);

    //        //플레이어 주변으로 respawnDistance 만큼 떨어짐. 각도는 랜덤
    //        Vector3 randPos = playerTr.rotation * randRot * (playerTr.forward * respawnDistance);
    //        GameObject mon = Instantiate(monObj, playerTr.position + randPos, Quaternion.identity);
    //        playerCtr.AddMonsterObjs(AllMonstersNum-1, mon.transform); // 07 26

    //        yield return new WaitForSeconds(respawnTime);
    //        yield return new WaitUntil(() => currentMonsters < maxMonsters); // 최대 생성 몬스터 개수에 도달하면 대기
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
        Debug.Log("다이");
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
    /// NPC의 불평을 듣게 될 것이다.
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

    /// <summary> 몬스터의 아이템을 Inventory로 전달 </summary>
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
