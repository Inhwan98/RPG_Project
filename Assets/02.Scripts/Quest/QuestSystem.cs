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
            //플레이어 레벨과 퀘스트 조건 레벨 비교
            if (CompareToPlayerLevel(conditionLevel))
            {
                possibleToProceedQuest.Add(questDatas[i]);
            }

        }
    }


    /// <summary> 
    /// Player Level과 Quest 제한 레벨 비교
    /// 플레이어 레벨이 더 높거나 같아야 참이다.
    /// </summary>
    public bool CompareToPlayerLevel(int questLevel)
    {
        return PlayerController.instance.GetLevel() >= questLevel;
    }



}
