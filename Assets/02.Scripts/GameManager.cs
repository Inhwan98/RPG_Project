using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private PlayerController playerCtr;
    private Transform playerTr;
    private ResourcesData _resourcesData;

    public static GameManager instance = null;
    private DialogSystem _dialogSystem;
    private QuestSystem _questSystem;

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
        _dialogSystem = GetComponent<DialogSystem>();
        _questSystem = GetComponent<QuestSystem>();

    }

    void Start()
    {
        playerCtr = PlayerController.instance;
        playerTr  = playerCtr.transform;

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

    public void PlayerDie()
    {
        m_bisPlayerDie = true;
        Debug.Log("다이");
    }

    public DialogSystem GetDialogSystem() =>  _dialogSystem;
    public QuestSystem GetQuestSystem() => _questSystem;
}
