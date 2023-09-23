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
    /// �÷��̾��� ������ ��������
    /// ������ ����Ʈ ����� ������Ʈ�Ѵ�.
    /// </summary>
    public List<QuestData> GetQuestList(int playerLevel)
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
        return _possibleToProceedQuest;
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
