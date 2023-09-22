using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{

    QuestData[] questDatas;
    List<QuestData> possibleToProceedQuest = new List<QuestData>();

    int playerLevel;

    private void Awake()
    {
        questDatas = SaveSys.LoadDialogData().Quest;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateToQuestList();
    }

    public void UpdateToQuestList()
    {
        for(int i = 0; i < questDatas.Length; i++)
        {
            int conditionLevel = questDatas[i].nConditionLevel;
            //�÷��̾� ������ ����Ʈ ���� ���� ��
            if (CompareToPlayerLevel(conditionLevel))
            {
                possibleToProceedQuest.Add(questDatas[i]);
            }

        }
    }


    /// <summary> 
    /// Player Level�� Quest ���� ���� ��
    /// �÷��̾� ������ �� ���ų� ���ƾ� ���̴�.
    /// </summary>
    public bool CompareToPlayerLevel(int questLevel)
    {
        return PlayerController.instance.GetLevel() >= questLevel;
    }



}
