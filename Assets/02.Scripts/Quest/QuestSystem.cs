using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    ���� = 1,
    �Ϲ� = 2
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
    /// �÷��̾��� ������ ��������
    /// ������ ����Ʈ ����� ������Ʈ�Ѵ�.
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

            //�÷��̾� ������ ����Ʈ ���� ���� ��
            if (CompareToPlayerLevel(playerLevel, conditionLevel))
            {
                //�Ϸ���� ���� ����Ʈ�� ���� ����Ʈ ��Ͽ� �߰�
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
    /// ����Ʈ �Ϸ�ó�� ��, ����Ʈ�� ���� ����Ʈ�� ��ȯ�Ѵ�.
    /// ��ȯ�� null ���̶�� ����Ʈ�� ���̹Ƿ� ������ �ֵ��� �Ѵ�.
    /// </summary>
    public QuestData TryGetNextQuest(QuestData questData)
    {
        QuestCompleteProcess(questData);

        foreach (var quest in _possibleToProceedQuest)
        {
            //�ش� ����ƮID���� ���� �귻ġ�� �ִٸ� ���� ����Ʈ�� ����
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
    /// ����Ʈ �ý��ۿ��� ����Ʈ �Ϸ� ó��
    /// �Ϸῡ ���� UI�� ������Ʈ ó�� �Ѵ�.
    /// </summary>
    private void QuestCompleteProcess(QuestData questData)
    {
        questData.bIsProgress = false;
        questData.bIsComplete = true;

        _possibleToProceedQuest.Remove(questData);

        //���ԵǾ� ���� �ʴٸ�
        if (!_completeQuest.Contains(questData))
        {
            _completeQuest.Add(questData);
        }
    }
    /// <summary> 
    /// Player Level�� Quest ���� ���� ��
    /// �÷��̾� ������ �� ���ų� ���ƾ� ���̴�.
    /// </summary>
    private bool CompareToPlayerLevel(int playerLevel, int questLevel)
    {
        return playerLevel >= questLevel;
    }
    #endregion
}
