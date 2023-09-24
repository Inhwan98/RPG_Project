using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;

public class QuestUI : MonoBehaviour
{

    [SerializeField] private int n_maxManageableQuest = 3;
    [SerializeField] private GameObject _QuestPanel;

    [SerializeField] private TMP_Text[] _questHeaders;
    [SerializeField] private TMP_Text[] _disCriptions;
    private int[] _questSlotIDArray;

    private int _playerLevel;

    private TMP_Text _questHeader;
    private TMP_Text _disCription;

    private QuestSystem _questSystem;
    private List<QuestData> _currentQuest;

    ResourcesData _resourcesData;
    GameManager _gameMgr;

    private void Awake()
    {
        _questHeaders = new TMP_Text[n_maxManageableQuest];
        _disCriptions = new TMP_Text[n_maxManageableQuest];
        _questSlotIDArray = new int[n_maxManageableQuest];

        _resourcesData = new ResourcesData();

        _questHeader = _resourcesData.GetQuestHeader();
        _disCription = _resourcesData.GetQuestConText();

        //제목, 내용 생성과 할당.
        for(int i = 0; i < n_maxManageableQuest; i++)
        {
            //퀘스트 패널의 자식으로 생성한다.
            _questHeaders[i] = Instantiate<TMP_Text>(_questHeader, _QuestPanel.transform);
            _disCriptions[i] = Instantiate<TMP_Text>(_disCription, _QuestPanel.transform);

            //생성 후 비활성화
            _questHeaders[i].gameObject.SetActive(false);
            _disCriptions[i].gameObject.SetActive(false);
        }

    }


    private void Start()
    {
        _gameMgr = GameManager.instance;

    }

    public void UpdateQuestUI(List<QuestData> currentQuest)
    {
        int size = currentQuest.Count;
        int preID = 0;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < size; i++)
        {
            //초기 퀘스트 슬롯 index 세팅
            if(_questSlotIDArray[i] == 0)
            {
                _questSlotIDArray[i] = currentQuest[i].nQuestID;
                _questHeaders[i].gameObject.SetActive(true);
                _disCriptions[i].gameObject.SetActive(true);
            }

            int index = Array.IndexOf(_questSlotIDArray, currentQuest[i].nQuestID);


            Debug.Assert(index != -1, "Quest Slot index NULL");

            if (currentQuest[i].eQuestType == 1)
                sb.Append($"<color=orange>[메인]");
            else
                sb.Append($"[일반]");

            sb.Append($"{currentQuest[i].sQuestName}");
            _questHeaders[index].text = sb.ToString();

            sb.Clear();


            sb.Append($"{currentQuest[i].sDiscription}");
            _disCriptions[index].text = sb.ToString();
        }
    }
}
