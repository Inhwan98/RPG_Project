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

    private QuestData[] _questDatas;
    private List<QuestData> _possibleToProceedQuest = new List<QuestData>();
    private List<QuestData> _completeQuest = new List<QuestData>();

    public QuestSystem()
    {
        _questDatas = SaveSys.LoadAllData().QuestDB;
    }


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
    public QuestData NextQuest(QuestData questData)
    {
        QuestCompleteProcess(questData);

        foreach (var quest in _possibleToProceedQuest)
        {
            //�ش� ����ƮID���� ���� �귻ġ�� �ִٸ� ���� ����Ʈ�� ����
            if (quest.nQuestID == questData.nQuestID && quest.nBranch == questData.nBranch + 1)
            {
                return quest;
            }
        }

        return null;
    }

    /// <summary>
    /// ����Ʈ �ý��ۿ��� ����Ʈ �Ϸ� ó��
    /// �Ϸῡ ���� UI�� ������Ʈ ó�� �Ѵ�.
    /// </summary>
    public void QuestCompleteProcess(QuestData questData)
    {
        questData.bIsComplete = true;

        _possibleToProceedQuest.Remove(questData);
  
        //���ԵǾ� ���� �ʴٸ�
        if (!_completeQuest.Contains(questData))
        {
            _completeQuest.Add(questData);
        }
    }



    public List<QuestData> GetPossibleQuest()
    {
        return _possibleToProceedQuest;
    }

    public void CompleteQuest(QuestData questData)
    {
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
    public bool CompareToPlayerLevel(int playerLevel, int questLevel)
    {
        return playerLevel >= questLevel;
    }





}
