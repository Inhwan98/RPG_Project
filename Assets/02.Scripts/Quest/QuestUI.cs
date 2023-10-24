using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;

public class QuestUI : MonoBehaviour
{
    /********************************************
     *                 Fields
     ********************************************/
    #region option Fields
    [SerializeField] private int n_maxManageableQuest = 3; //수락 가능한 퀘스트 수
    [SerializeField] private GameObject _QuestPanel;

    [SerializeField] private TMP_Text[] _questHeaders;
    [SerializeField] private TMP_Text[] _disCriptions;
    #endregion

    #region private Fields
    private TMP_Text _questHeader;
    private TMP_Text _disCription;
    private ResourcesData _resourcesData;
    private GameManager _gameMgr;

    //내용을 담기위한 StringBuilder. 수정이 잦기 때문에 String은 쓰지 않았다.
    private StringBuilder sb = new StringBuilder();
    private int[] _questSlotIDArray;

    #endregion

    /********************************************
     *                Unity Event
     ********************************************/
    #region Unity Event
    private void Awake()
    {
        _questHeaders = new TMP_Text[n_maxManageableQuest];
        _disCriptions = new TMP_Text[n_maxManageableQuest];
        _questSlotIDArray = new int[n_maxManageableQuest];
    }

    private void Start()
    {
        _gameMgr = GameManager.instance;
        _QuestPanel.SetActive(false);
    }
    #endregion


    /********************************************
     *               Methods
     ********************************************/

    #region public Methods

    /// <summary> GameManager Awake()에서 초기화 </summary>
    public void QuestUI_Init(ResourcesData resourcesData)
    {
        this._resourcesData = resourcesData;

        _questHeader = _resourcesData.GetQuestHeader();
        _disCription = _resourcesData.GetQuestConText();

        //제목, 내용 생성과 할당.
        for (int i = 0; i < n_maxManageableQuest; i++)
        {
            //퀘스트 패널의 자식으로 생성한다.
            _questHeaders[i] = Instantiate<TMP_Text>(_questHeader, _QuestPanel.transform);
            _disCriptions[i] = Instantiate<TMP_Text>(_disCription, _QuestPanel.transform);

            //생성 후 비활성화
            _questHeaders[i].gameObject.SetActive(false);
            _disCriptions[i].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 플레이어가 승인한 퀘스트들을 시각적으로 좌측에 표시할 목록 Update
    /// </summary>
    /// <param name="currentQuestList"> Player Controller이 지닌 현재 퀘스트 리스트 이다. </param>
    public void UpdateQuestUI(List<QuestData> currentQuestList)
    {
        //진행 퀘스트가 없다면 비활성화
        if (currentQuestList == null)
        {
            _QuestPanel.SetActive(false);
            return;
        }

        _QuestPanel.SetActive(true);

        int size = currentQuestList.Count;

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
        }
    }

    public void UpdateQuestUI(QuestData questData)
    {
        int index = Array.IndexOf(_questSlotIDArray, questData.nQuestID);
        Debug.Assert(index != -1, "Quest Slot index NULL");

        //퀘스트 데이터가 완료 처리라면 해당 퀘스트 UI 초기화
        if(questData.bIsComplete == true)
        {
            _questSlotIDArray[index] = 0;
            _questHeaders[index].gameObject.SetActive(false);
            _disCriptions[index].gameObject.SetActive(false);
            return;
        }


        #region Title 설정
        if (questData.eQuestType == QuestType.메인)
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
        if (questData.eObjectives == QuestObjectives.HUNT)
        {
            sb.Append($" {questData.nCurCnt}/{questData.nGoalCnt}");
        }

        _disCriptions[index].text = sb.ToString();
        sb.Clear();
        #endregion
    }


    #endregion
}
