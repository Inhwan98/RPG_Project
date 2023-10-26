using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.AI;
using InHwan.CircularQueue;

public class DungeonSceneManager : GameSceneManager
{
    /************************************************
     *                   Fields
     ************************************************/
    #region option Fields
    [SerializeField] private float _respawnMaxDistance;           //스폰 지점으로 부터 최대 멀어질 거리
    [SerializeField] private Transform spwanTr;                   //몬스터 스폰 지점
    [SerializeField] private PlayableDirector _bossStartDirector; //보스의 타임라인
    #endregion

    #region private Fields
    private DungeonUI _dungeonUI;
    private PlayerController _playerCtr;
    private BossMonster _bossMonsterCtr;

    private GameObject monsterRootObj;
    private GameObject _villiagePortal; //보스 사망시 나타날 포탈

    private List<DungeonStage> stageDataList = new List<DungeonStage>();
    private Coroutine portalFollowCoroutine;

    private CircularQueue<GameObject> monsterQue;

    private int circleQueSize = 0;
    private int m_nStageIndex = 0;
    private bool m_bisDungeonActive;
    private bool m_bisPlayerDie;
    #endregion


    /************************************************
     *                Unity Event
     ************************************************/
    #region Unity Event
    void Start()
    {
        _playerCtr = PlayerController.instance;

        DungeonMonsterInit();
        CreateMonsterPool();
        CreatePortal();
        DungeonReset();
        //던전 시작시 던전UI 내용 초기화
        _dungeonUI.UpdateDungeonUI(stageDataList[m_nStageIndex], _sSceneName);
    }

    /// <summary> 특정구역에 플레이어가 들어왔다면 던전 활성화 </summary>
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (!m_bisDungeonActive)
            {
                _dungeonUI.gameObject.SetActive(true);
                _dungeonUI.UpdateDungeonUI(stageDataList[m_nStageIndex], _sSceneName);
                StartCoroutine(UpdateMonster());
            }

        }
    }

    private void OnDestroy()
    {
        DungeonReset();
        instance = null;
    }

    #endregion


    /************************************************
    *                  Methods
    ************************************************/
    #region override Methods
    protected override void Init()
    {
        base.Init();

        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = Color.black;

        monsterRootObj = new GameObject("@MonsterRoot");
        _dungeonUI = FindObjectOfType<DungeonUI>();
        _bossMonsterCtr = FindObjectOfType<BossMonster>();
        _dungeonUI.gameObject.SetActive(false);
    }
    #endregion

    #region private Methods
    /// <summary> 보스 클리어스 나타날 포탈 생성 </summary>
    private void CreatePortal()
    {
        _villiagePortal = Instantiate(GameManager.instance.GetResourcesData().GetVilliagePortal());
        _villiagePortal.SetActive(false);
    }

    /// <summary> 풀링된 몬스터를 세팅한다. </summary>
    private void SetStageMonster()
    {
        for (int i = 0; i < stageDataList[m_nStageIndex].nMonsterAmount; i++)
        {
            GameObject monsterGo = monsterQue.Front();
            monsterQue.DeQueue();
            //보스가 아니라면
            if (!stageDataList[m_nStageIndex].bIsBoss)
            {
                //랜덤한 위치로 생성
                Vector3 randPos = RandomTr.GetRandomPos(spwanTr, _respawnMaxDistance);
                monsterGo.transform.SetPositionAndRotation(randPos, Quaternion.identity);
                monsterGo.SetActive(true);
            }
            //보스라면
            else
            {
                //귀환포탈이 보스 추적 시작
                portalFollowCoroutine = StartCoroutine(PortalFollowsBoss(_bossMonsterCtr.gameObject));
                _bossStartDirector.Play(); //보스 TimeLine 재생
            }
        }
        _dungeonUI.UpdateDungeonUI(stageDataList[m_nStageIndex], _sSceneName);
    }

    /// <summary> 던전 초기화 </summary>
    private void DungeonReset()
    {
        foreach (var stageMonsterData in stageDataList)
        {
            stageMonsterData.nDeadMonsterCount = 0;
        }
    }
    #endregion

    #region public Methods
    /// <summary> 던전씬의 이름과 맞는 데이터로 스테이지 초기화 </summary>
    public void DungeonMonsterInit()
    {
        var dungeonDB = GameManager.instance.GetAllData().DungeonStageDB;

        foreach (var stageData in dungeonDB)
        {
            //현재 씬에 맞는 데이터만 추가
            if (stageData.sDungenName == _sSceneName)
            {
                stageDataList.Add(stageData);
            }
        }
    }
    /// <summary> StageData에 따른 몬스터 Pool 생성 </summary>
    public void CreateMonsterPool()
    {
        //모든 몬스터의 수만큼 데이터 크기 정한다.
        foreach (var stageData in stageDataList)
        {
            circleQueSize += stageData.nMonsterAmount;
        }

        monsterQue = new CircularQueue<GameObject>(circleQueSize);

        for (int i = 0; i < stageDataList.Count; i++)
        {
            if (stageDataList[i].bIsBoss) continue; //보스는 정적으로 생성해둔다.

            string path = Path.Combine("Prefab", "Monster", stageDataList[i].sMonsterName);
            Monster monster = Resources.Load<Monster>(path);
            monster.SetID(stageDataList[i].nMonsterID);
            monster.gameObject.SetActive(false);

            //정해진 몬스터의 개수만큼 몬스터 생성 후 비활성화
            for (int j = 0; j < stageDataList[i].nMonsterAmount; j++)
            {
                GameObject monsterGo = Instantiate(monster.gameObject, monsterRootObj.transform);
                monsterQue.EnQueue(monsterGo);
            }
        }

    }

    /// <summary> 현재 스테이지의 몬스터의 사망개수를 증가 시킨다. </summary>
    public void MonsterDead()
    {
        stageDataList[m_nStageIndex].nDeadMonsterCount++;
        _dungeonUI.UpdateDungeonUI(stageDataList[m_nStageIndex], _sSceneName);
    }
    #endregion


    /************************************************
    *                  Coroutine
    ************************************************/

    #region Coroutine
    //몬스터 생성주기
    private IEnumerator UpdateMonster()
    {
        while (m_nStageIndex != stageDataList.Count)
        {
            m_bisDungeonActive = true;

            SetStageMonster();

            yield return new WaitUntil(() => stageDataList[m_nStageIndex].nMonsterAmount
                                             <= stageDataList[m_nStageIndex].nDeadMonsterCount); // 해당 몬스터 다 잡을때까지 대기

            yield return new WaitForSeconds(5.0f);
            m_nStageIndex++;


            if (m_nStageIndex >= stageDataList.Count)
            {
                DungeonReset();
                m_bisDungeonActive = false;

                if (portalFollowCoroutine != null)
                {
                    StopCoroutine(portalFollowCoroutine);
                }

                _villiagePortal.transform.LookAt(_playerCtr.transform);

                Vector3 pos = _villiagePortal.transform.position;
                pos.y = _playerCtr.transform.position.y + 2;
                _villiagePortal.transform.position = pos;

                _villiagePortal.SetActive(true);
                // 보스 잡았을 때 이벤트.
                // 로아처럼?

                yield break;
            }
            //yield return new WaitForSeconds(respawnTime);
        }
    }
    /// <summary>
    /// 포탈이 0.2초마다 보스를 따라다님
    /// 보스의 자식으로 두는게 더 좋을까?
    /// </summary>
    private IEnumerator PortalFollowsBoss(GameObject bossGo)
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.2f);
        while (m_bisDungeonActive)
        {
            yield return waitTime;
            if (bossGo.activeSelf == false) yield break;
            _villiagePortal.transform.position = bossGo.transform.position;
        }
    }
    #endregion

  
}
