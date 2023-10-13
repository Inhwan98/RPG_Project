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

    public bool bIsBoss; //���� �ΰ�
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


        //���� ���۽� ����UI ���� �ʱ�ȭ
        _dungeonUI.UpdateDungeonUI(stageDataArray[m_nStageIndex], _dungeonName);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            _bossStartDirector.Play();
        }
    }

    /// <summary> StageData�� ���� ���� Pool ���� </summary>
    public void CreateMonsterPool()
    {
        //��� ������ ����ŭ ������ ũ�� ���Ѵ�.
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

            //������ ������ ������ŭ ������ ��ġ�� ���� ���� �� ��Ȱ��ȭ
            for (int j = 0; j < stageDataArray[i].nMonsterAmount; j++)
            {
                GameObject monsterGo = Instantiate(monster.gameObject, monsterRootObj.transform);
                //monsterGo.SetActive(false);
                monsterQue.EnQueue(monsterGo);
                //monsterList.Enqueue(monsterGo);
            }
        }

    }

    //���� �����ֱ�
    IEnumerator UpdateMonster()
    {
        while (m_nStageIndex != stageDataArray.Length)
        {
            m_bisDungeonActive = true;

            SetStageMonster();

            yield return new WaitUntil(() => stageDataArray[m_nStageIndex].nMonsterAmount
                                             <= stageDataArray[m_nStageIndex].nDeadMonsterCount); // �ش� ���� �� ���������� ���

            yield return new WaitForSeconds(5.0f);
            m_nStageIndex++;

            
            if(m_nStageIndex >= stageDataArray.Length)
            {
                // ���� ����� �� �̺�Ʈ.
                // �ξ�ó��?
                Debug.Log("Boss Clear");
               
                yield break;
            }
            //yield return new WaitForSeconds(respawnTime);
        }
    }

    /// <summary> Ǯ���� ���͸� �����Ѵ�. </summary>
    private void SetStageMonster()
    {
        for (int i = 0; i < stageDataArray[m_nStageIndex].nMonsterAmount; i++)
        {
            GameObject monsterGo = monsterQue.Front();
            monsterQue.DeQueue();

            //������ �ƴ϶��
            if(!stageDataArray[m_nStageIndex].bIsBoss)
            {
                //������ ��ġ�� ����
                Vector3 randPos = GetRandomPos();
                monsterGo.transform.SetPositionAndRotation(spwanTr.position + randPos, Quaternion.identity);
                monsterGo.SetActive(true);
            }
            //�������
            else
            {
                _bossStartDirector.Play(); //���� TimeLine ���
            }
        }
        _dungeonUI.UpdateDungeonUI(stageDataArray[m_nStageIndex], _dungeonName);
    }

    public Vector3 GetRandomPos()
    {
        float randRotNum = Random.Range(0, 360.0f);
        Quaternion randRot = Quaternion.Euler(spwanTr.up * randRotNum);

        //���� �������� �ֺ����� respawnDistance ��ŭ ������. ������ ����
        Vector3 randPos = spwanTr.rotation * randRot * (spwanTr.forward * _respawnDistance);

        return randPos;
    }

    /// <summary> ���� ���������� ������ ��������� ���� ��Ų��. </summary>
    public void MonsterDead()
    {
        stageDataArray[m_nStageIndex].nDeadMonsterCount++;
        _dungeonUI.UpdateDungeonUI(stageDataArray[m_nStageIndex], _dungeonName);
    }

    /// <summary> Ư�������� �÷��̾ ���Դٸ� ���� Ȱ��ȭ </summary>
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
