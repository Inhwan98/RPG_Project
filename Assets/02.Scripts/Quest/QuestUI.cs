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

    /// <summary>
    /// 플레이어가 승인한 퀘스트들을 시각적으로 좌측에 표시할 목록 Update
    /// </summary>
    /// <param name="currentQuestList"> Player Controller이 지닌 현재 퀘스트 리스트 이다. </param>
    public void UpdateQuestUI(List<QuestData> currentQuestList)
    {
        int size = currentQuestList.Count;
        int preID = 0;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < size; i++)
        {
            //초기 퀘스트 슬롯 index 세팅
            if(_questSlotIDArray[i] == 0)
            {
                _questSlotIDArray[i] = currentQuestList[i].nQuestID;
                _questHeaders[i].gameObject.SetActive(true);
                _disCriptions[i].gameObject.SetActive(true);
            }

            //오버로딩된 UpdateQuestUI 재호출
            UpdateQuestUI(currentQuestList[i]);

            #region 오버로딩 함수 삽입 전
            //int index = Array.IndexOf(_questSlotIDArray, currentQuestList[i].nQuestID);
            //Debug.Assert(index != -1, "Quest Slot index NULL");

            //#region Title 설정
            //if (currentQuestList[i].eQuestType == 1)
            //    sb.Append($"<color=orange>[메인]");
            //else
            //    sb.Append($"[일반]");

            //sb.Append($"{currentQuestList[i].sQuestName}");

            ////사냥 퀘스트라면 진행도를 UI Text로 나타낸다.
            //if (currentQuestList[i].eObjectives == (int)QuestObjectives.HUNT)
            //{
            //    sb.Append($"<color=white> {currentQuestList[i].nCurCnt}/{currentQuestList[i].nGoalCnt}");
            //}
            //_questHeaders[index].text = sb.ToString();

            //sb.Clear();
            //#endregion


            //#region 퀘스트 내용 설정
            //sb.Append($"{currentQuestList[i].sDiscription}");
            //_disCriptions[index].text = sb.ToString();
            //#endregion
            #endregion 오버로딩 함수 삽입 후
        }
    }

    public void UpdateQuestUI(QuestData questData)
    {
        StringBuilder sb = new StringBuilder();

        int index = Array.IndexOf(_questSlotIDArray, questData.nQuestID);
        Debug.Assert(index != -1, "Quest Slot index NULL");

        #region Title 설정
        if (questData.eQuestType == 1)
            sb.Append($"<color=orange>[메인]");
        else
            sb.Append($"[일반]");

        sb.Append($"{questData.sQuestName}");

       
        _questHeaders[index].text = sb.ToString();

        sb.Clear();
        #endregion


        #region 퀘스트 내용 설정
        sb.Append($"{questData.sDiscription}");

        //사냥 퀘스트라면 진행도를 UI Text로 나타낸다.
        if (questData.eObjectives == (int)QuestObjectives.HUNT)
        {
            sb.Append($" {questData.nCurCnt}/{questData.nGoalCnt}");
        }

        _disCriptions[index].text = sb.ToString();
        #endregion
    }
}
