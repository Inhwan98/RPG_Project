using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void PlayerDie()
    {
        m_bisPlayerDie = true;
        Debug.Log("����");
    }

    public DialogSystem GetDialogSystem() =>  _dialogSystem;
    public QuestSystem GetQuestSystem() => _questSystem;
}
