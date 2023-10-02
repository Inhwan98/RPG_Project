using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MonsterRecord
{
    public int[] monsterRecords;
}

public class GameManager : MonoBehaviour
{
    private bool m_bisPlayerDie; //플레이어가 죽었는가

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

        //skillManager Awake() 전에 skinvenUI는 할당 되었다.(드로그 앤 드랍)
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

    //몬스터의 생성
    public void AddCurrentMonsters(int nMonsterID)
    {
         
    }


    public DialogSystem GetDialogSystem() => _dialogSystem;
    public QuestSystem GetQuestSystem() => _questSystem;
    public ResourcesData GetResourcesData() => _resourcesData;

    public void SetPlayerCtr(PlayerController playerCtr) => _playerCtr = playerCtr;

    public void UpdateQuestList(int m_nLevel) => _questSystem.UpdateQuestList(m_nLevel);

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
                    _questUICtr.UpdateQuestUI(_playerQuestList[i]);
            }
        }
    }


    public void PlayerDie()
    {
        m_bisPlayerDie = true;
        Debug.Log("다이");
    }

    //9.24 생성중
    public IEnumerator CheckQuest(QuestData questData)
    {
        QuestObjectives questObjectives = (QuestObjectives)questData.eObjectives;


        switch (questObjectives)
        {
            case QuestObjectives.CONVERSATION:
                break;
            case QuestObjectives.HUNT:
                
                int monindex = questData.nDestID - 100; // monster의 ID는 100대 부터 시작.

                //퀘스트의 목표 개수와 해당ID 몬스터를 잡은 카운트가 다르다면 코루틴 계속 반복
                //questData.nCurCnt는 몬스터 사망시
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
    /// NPC의 불평을 듣게 될 것이다.
    /// </summary>
    public IEnumerator UnsolvedQuestDialog(int id)
    {
        _dialogUICtr.SetDialogList(id, 0);
        _playerCtr.SetUsePlayDialog(true); // 플레이어 동작 관리
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());
        _playerCtr.SetUsePlayDialog(false); // 플레이어 동작 관리
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
        _dialogUICtr.SetDialogList(questId, branch);
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());

        _playerCtr.SetUsePlayDialog(false); // 플레이어 동작 관리

        //대화가 끝나면 플레이어가 진행하는 퀘스트 목록에 추가
        // Yes, No 팝업을 띄워서 수락 / 거절 해도 된다.
        _playerCtr.AddPlayerQuest(questData); 

        _playerQuestList = _playerCtr.GetPlayerQuestList();

        _questUICtr.UpdateQuestUI(_playerQuestList);
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
        _dialogUICtr.SetDialogList(questId, branch);
        yield return new WaitUntil(() => _dialogUICtr.UpdateDialog());

        _playerCtr.SetUsePlayDialog(false); // 플레이어 동작 관리
        CompleteQuest(questData, index);
    }

    /// <summary>
    /// 퀘스트 완료 처리
    /// </summary>
    public void CompleteQuest(QuestData currentQuestData, int index)
    {
        var nextQuest = _questSystem.NextQuest(currentQuestData);

        //null이라면 다음 퀘스트가 없는 완료된 퀘스트이기 때문에, 현재 진행 퀘스트에서 제거
        if (nextQuest == null)
        {
            Debug.Log("보상을 주는 내용");
            _playerCtr.SetEXP(currentQuestData.nRewardEXP); //경험치 전달

            //아이템의 개수만큼 아이템과 수량 전달
            for(int i = 0; i < currentQuestData.nRewardItem.Length; i++)
            {
                int nID = currentQuestData.nRewardItem[i];
                int nAmount = currentQuestData.nItemAmount[i];
                _playerCtr.AddInven(_resourcesData.GetItem(nID), nAmount);
            }


            _questUICtr.UpdateQuestUI(_playerQuestList);
            _playerQuestList.Remove(currentQuestData);
        }
        else
        {
            //바뀐 데이터를 PlayerController QuestList 데이터에도 영향을 주기 위해
            //새로 생긴 객체(다음 퀘스트)를 참조 주소에 직접 넣어준다.
            _playerQuestList[index] = nextQuest;
            _questUICtr.UpdateQuestUI(_playerQuestList);
        }
    }


    /// <summary> 몬스터의 아이템을 Inventory로 전달 </summary>
    public void AddInven(Dictionary<ItemData, int> _itemDic)
    {
        foreach (var itemDic in _itemDic)
        {
            _itemInvenMgr.AddItem(itemDic.Key, itemDic.Value);
        }
    }



}
