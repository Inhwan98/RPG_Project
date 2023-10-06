using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class StageData
{
    public string sMonsterName;
    public int nMonsterID;
    public int monsterAmount;
}

public class DungeonManager : MonoBehaviour
{

    [SerializeField] private float _respawnDistance;

    PlayerController playerCtr;
    Transform _playerTr;

    [SerializeField]
    StageData[] stageData;

    //Que
    Queue<GameObject> monsterList = new Queue<GameObject>();


    private int m_nStageIndex = 0;

    bool m_bisPlayerDie;
    // Start is called before the first frame update
    void Start()
    {
        playerCtr = PlayerController.instance;

        for (int i = 0; i < stageData.Length; i++)
        {
            string path = Path.Combine("Prefab", stageData[i].sMonsterName);
            Monster monster = Resources.Load<Monster>(path);
            monster.SetID(stageData[i].nMonsterID);

            for (int j = 0; j < stageData[i].monsterAmount; j++)
            {
                Vector3 randPos = GetRandomPos();
                monsterList.Enqueue(Instantiate(monster.gameObject, randPos, Quaternion.identity));
                monsterList.Peek().SetActive(false);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetRandomPos()
    {
        float randRotNum = Random.Range(0, 360.0f);
        Quaternion randRot = Quaternion.Euler(_playerTr.up * randRotNum);
        //Vector3 randPos = randRot * (playerTr.forward * respawnDistance);

        //플레이어 주변으로 respawnDistance 만큼 떨어짐. 각도는 랜덤
        Vector3 randPos = (_playerTr.rotation * randRot * (_playerTr.forward * _respawnDistance)) + _playerTr.position;

        return randPos;
    }

    //몬스터 생성주기
    IEnumerator UpdateMonster()
    {
        while (!m_bisPlayerDie)
        {
            float randRotNum = Random.Range(0, 360.0f);
            Quaternion randRot = Quaternion.Euler(_playerTr.up * randRotNum);
            //Vector3 randPos = randRot * (playerTr.forward * respawnDistance);

            //플레이어 주변으로 respawnDistance 만큼 떨어짐. 각도는 랜덤
            Vector3 randPos = _playerTr.rotation * randRot * (_playerTr.forward * _respawnDistance);
            //GameObject mon = Instantiate(monObj, _playerTr.position + randPos, Quaternion.identity);
            GameObject monsterGo = monsterList.Dequeue();

            yield return null;
            //yield return new WaitForSeconds(respawnTime);
            //yield return new WaitUntil(() => currentMonsters < maxMonsters); // 최대 생성 몬스터 개수에 도달하면 대기
        }
    }
}
