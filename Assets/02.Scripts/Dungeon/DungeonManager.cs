using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.AI;
using InHwan.CircularQueue;

[System.Serializable]
public class StageData
{
    public string sMonsterName;
    public int nMonsterID;
    public int nMonsterAmount;
    public int nDeadMonsterCount;
    public int nCurProgressNum;
    public int nMaxProgressNum;

    public bool bIsBoss; //보스 인가
}

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance = null;

    [SerializeField] private float _respawnDistance;
    [SerializeField] private Transform spwanTr;
    [SerializeField] private StageData[] stageDataArray;
    [SerializeField] private PlayableDirector _bossStartDirector;

    private DungeonUI _dungeonUI;

    private PlayerController _playerCtr;

    private GameObject monsterRootObj;
    private CircularQueue<GameObject> monsterQue;
    private int circleQueSize = 0;

    private string _dungeonName;

    //private int destMonsterAmount;

    private bool m_bisDungeonActive;
    private int m_nStageIndex = 0;

    bool m_bisPlayerDie;
    // Start is called before the first frame update
    private void Awake()
    {
        #region SingleTone
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        #endregion

        monsterRootObj = new GameObject("@MonsterRoot");
        _dungeonUI = FindObjectOfType<DungeonUI>();
        _dungeonUI.gameObject.SetActive(false);
        _dungeonName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    void Start()
    {
        _playerCtr = PlayerController.instance;

        CreateMonsterPool();


        //던전 시작시 던전UI 내용 초기화
        _dungeonUI.UpdateDungeonUI(stageDataArray[m_nStageIndex], _dungeonName);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            _bossStartDirector.Play();
        }
    }

    /// <summary> StageData에 따른 몬스터 Pool 생성 </summary>
    public void CreateMonsterPool()
    {
        //모든 몬스터의 수만큼 데이터 크기 정한다.
        foreach (var stageData in stageDataArray)
        {
            circleQueSize += stageData.nMonsterAmount;
        }

        monsterQue = new CircularQueue<GameObject>(circleQueSize);

        for (int i = 0; i < stageDataArray.Length; i++)
        {
            if (stageDataArray[i].bIsBoss) continue;

            string path = Path.Combine("Prefab", "Monster", stageDataArray[i].sMonsterName);
            Monster monster = Resources.Load<Monster>(path);
            monster.SetID(stageDataArray[i].nMonsterID);
            monster.gameObject.SetActive(false);

            //정해진 몬스터의 개수만큼 랜덤한 위치에 몬스터 생성 후 비활성화
            for (int j = 0; j < stageDataArray[i].nMonsterAmount; j++)
            {
                GameObject monsterGo = Instantiate(monster.gameObject, monsterRootObj.transform);
                //monsterGo.SetActive(false);
                monsterQue.EnQueue(monsterGo);
                //monsterList.Enqueue(monsterGo);
            }
        }

    }

    //몬스터 생성주기
    IEnumerator UpdateMonster()
    {
        while (m_nStageIndex != stageDataArray.Length)
        {
            m_bisDungeonActive = true;

            SetStageMonster();

            yield return new WaitUntil(() => stageDataArray[m_nStageIndex].nMonsterAmount
                                             <= stageDataArray[m_nStageIndex].nDeadMonsterCount); // 해당 몬스터 다 잡을때까지 대기

            yield return new WaitForSeconds(5.0f);
            m_nStageIndex++;

            
            if(m_nStageIndex >= stageDataArray.Length)
            {
                // 보스 잡았을 때 이벤트.
                // 로아처럼?
                Debug.Log("Boss Clear");
               
                yield break;
            }
            //yield return new WaitForSeconds(respawnTime);
        }
    }

    /// <summary> 풀링된 몬스터를 세팅한다. </summary>
    private void SetStageMonster()
    {
        for (int i = 0; i < stageDataArray[m_nStageIndex].nMonsterAmount; i++)
        {
            GameObject monsterGo = monsterQue.Front();
            monsterQue.DeQueue();

            //보스가 아니라면
            if(!stageDataArray[m_nStageIndex].bIsBoss)
            {
                //랜덤한 위치로 생성
                Vector3 randPos = GetRandomPos();
                monsterGo.transform.SetPositionAndRotation(spwanTr.position + randPos, Quaternion.identity);
                monsterGo.SetActive(true);
            }
            //보스라면
            else
            {
                _bossStartDirector.Play(); //보스 TimeLine 재생
            }
        }
        _dungeonUI.UpdateDungeonUI(stageDataArray[m_nStageIndex], _dungeonName);
    }

    public Vector3 GetRandomPos()
    {
        float randRotNum = Random.Range(0, 360.0f);
        Quaternion randRot = Quaternion.Euler(spwanTr.up * randRotNum);

        //생성 스테이지 주변으로 respawnDistance 만큼 떨어짐. 각도는 랜덤
        Vector3 randPos = spwanTr.rotation * randRot * (spwanTr.forward * _respawnDistance);

        return randPos;
    }

    /// <summary> 현재 스테이지의 몬스터의 사망개수를 증가 시킨다. </summary>
    public void MonsterDead()
    {
        stageDataArray[m_nStageIndex].nDeadMonsterCount++;
        _dungeonUI.UpdateDungeonUI(stageDataArray[m_nStageIndex], _dungeonName);
    }

    /// <summary> 특정구역에 플레이어가 들어왔다면 던전 활성화 </summary>
    private void OnTriggerEnter(Collider coll)
    {
        if(coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if(!m_bisDungeonActive)
            {
                _dungeonUI.gameObject.SetActive(true);
                _dungeonUI.UpdateDungeonUI(stageDataArray[m_nStageIndex], _dungeonName);
                StartCoroutine(UpdateMonster());
            }
            
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
