using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem
{

    private QuestData[] _questDatas;
    private List<QuestData> _possibleToProceedQuest = new List<QuestData>();
    private List<QuestData> _completeQuest = new List<QuestData>();

    public QuestSystem()
    {
        _questDatas = SaveSys.LoadDialogData().Quest;
    }


    /// <summary>
    /// 플레이어의 레벨을 기준으로
    /// 가능한 퀘스트 목록을 업데이트한다.
    /// </summary>
    public List<QuestData> GetQuestList(int playerLevel)
    {
        _possibleToProceedQuest.Clear();
        _completeQuest.Clear();

        for (int i = 0; i < _questDatas.Length; i++)
        {
            int conditionLevel = _questDatas[i].nConditionLevel;
            bool isCompleteQuest = _questDatas[i].bIsComplete;
            //플레이어 레벨과 퀘스트 조건 레벨 비교
            if (CompareToPlayerLevel(playerLevel, conditionLevel))
            {
                //완료되지 않은 퀘스트는 현재 퀘스트 목록에 추가
                if(!isCompleteQuest)
                {
                    _possibleToProceedQuest.Add(_questDatas[i]);
                }
                else
                {
                    _completeQuest.Add(_questDatas[i]);
                }

            }
        }
        return _possibleToProceedQuest;
    }



    /// <summary> 
    /// Player Level과 Quest 제한 레벨 비교
    /// 플레이어 레벨이 더 높거나 같아야 참이다.
    /// </summary>
    public bool CompareToPlayerLevel(int playerLevel, int questLevel)
    {
        return playerLevel >= questLevel;
    }





}
