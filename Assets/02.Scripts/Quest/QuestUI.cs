using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject _QuestPanel;

    private TMP_Text _questHeader;
    private TMP_Text _disCription;

    private int _playerLevel;

    private QuestSystem _questSystem;
    private List<QuestData> _currentQuest;

    ResourcesData _resourcesData;
    GameManager _gameMgr;

    private void Awake()
    {
        _resourcesData = new ResourcesData();

        _questHeader = _resourcesData.GetQuestHeader();
        _disCription = _resourcesData.GetQuestConText();
    }


    private void Start()
    {
        _gameMgr = GameManager.instance;

    }

    public void UpdateQuestUI(List<QuestData> currentQuest)
    {
        ClearPanel();

        int size = currentQuest.Count;
        int preID = 0;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < size; i++)
        {
            if(preID != currentQuest[i].nQuestID)
            {
                string questName = currentQuest[i].sQuestName;

                //중복 헤더 Name은 생략한다.

                if (currentQuest[i].eQuestType == 1)
                    sb.Append($"<color=orange>[메인]");
                else
                    sb.Append($"[일반]");


                string name = questName;
                sb.Append(name);

                var headText = Instantiate<TMP_Text>(_questHeader, _QuestPanel.transform);
                headText.text = sb.ToString();
                sb.Clear();

                preID = currentQuest[i].nID;
            }

            // 현재 퀘스트 설명
            string discription = currentQuest[i].sDiscription;

            //완료된 퀘스트라면 회색 밑줄 처리
            if (currentQuest[i].bIsComplete)
            {
                sb.Append($"<s><color=grey>{discription}</s>");
            }
            else
            {
                sb.Append($"{discription}");
            }

            var discriptionText = Instantiate<TMP_Text>(_disCription, _QuestPanel.transform);
            discriptionText.text = sb.ToString();

            sb.Clear();
        }
    }

    public void ClearPanel()
    {
        int count = _QuestPanel.transform.childCount;

        for(int i = 0; i < count; i++)
        {
            Destroy(_QuestPanel.transform.GetChild(i).gameObject);
        }

    }

}