using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    메인 = 1,
    일반 = 2
}

public enum QuestObjectives
{
    CONVERSATION = 1,
    HUNT,
    SKILL
}

public class QuestSystem
{
    /**********************************************
     *                  Fields
     **********************************************/

    #region private Fields
    private QuestData[] _questDatas;
    private List<QuestData> _inProgressQuestList = new List<QuestData>();
    private List<QuestData> _possibleToProceedQuest = new List<QuestData>();
    private List<QuestData> _completeQuest = new List<QuestData>();
    #endregion

    /**********************************************
    *                  Constructor
    **********************************************/
    public QuestSystem(QuestData[] questDatas)
    {
        this._questDatas = questDatas;
    }

    /**********************************************
     *               Get, Set Methods
     **********************************************/
    #region Get Methods
    public List<QuestData> GetPossibleQuest() => _possibleToProceedQuest;
    public List<QuestData> GetInProgressQuestList() => _inProgressQuestList;
    public QuestData[] GetQuestDataArray() => _questDatas;
    #endregion


    /**********************************************
     *                    Methods
     **********************************************/

    #region public Methods
    /// <summary>
    /// 플레이어의 레벨을 기준으로
    /// 가능한 퀘스트 목록을 업데이트한다.
    /// </summary>
    public void UpdateQuestList(int playerLevel)
    {
        _possibleToProceedQuest.Clear();
        _completeQuest.Clear();

        for (int i = 0; i < _questDatas.Length; i++)
        {
            int conditionLevel = _questDatas[i].nConditionLevel;
            bool isCompleteQuest = _questDatas[i].bIsComplete;
            bool isProgress = _questDatas[i].bIsProgress;

            if (isProgress)
            {
                _inProgressQuestList.Add(_questDatas[i]);
            }

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
    }

    /// <summary>
    /// 퀘스트 완료처리 후, 퀘스트의 다음 퀘스트를 반환한다.
    /// 반환이 null 값이라면 퀘스트의 끝이므로 보상을 주도록 한다.
    /// </summary>
    public QuestData TryGetNextQuest(QuestData questData)
    {
        QuestCompleteProcess(questData);

        foreach (var quest in _possibleToProceedQuest)
        {
            //해당 퀘스트ID에서 다음 브렌치가 있다면 다음 퀘스트를 리턴
            if (quest.nQuestID == questData.nQuestID && quest.nBranch == questData.nBranch + 1)
            {
                quest.bIsProgress = true;
                return quest;
            }
        }
        return null;
    }
    #endregion
    #region private Methods
    /// <summary>
    /// 퀘스트 시스템에서 퀘스트 완료 처리
    /// 완료에 대한 UI도 업데이트 처리 한다.
    /// </summary>
    private void QuestCompleteProcess(QuestData questData)
    {
        questData.bIsProgress = false;
        questData.bIsComplete = true;

        _possibleToProceedQuest.Remove(questData);

        //포함되어 있지 않다면
        if (!_completeQuest.Contains(questData))
        {
            _completeQuest.Add(questData);
        }
    }
    /// <summary> 
    /// Player Level과 Quest 제한 레벨 비교
    /// 플레이어 레벨이 더 높거나 같아야 참이다.
    /// </summary>
    private bool CompareToPlayerLevel(int playerLevel, int questLevel)
    {
        return playerLevel >= questLevel;
    }
    #endregion
}
