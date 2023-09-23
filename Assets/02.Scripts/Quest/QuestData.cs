[System.Serializable]
public class QuestData
{
    public int nQuestID;
    public int nBranch;
    public int nConditionLevel;
    public int nID;
    public int nDestID;
    public int eQuestType;
    public string sQuestName;
    public string sDiscription;
    public int nGoalCnt;
    public int nRewardEXP;
    public int nRewardGold;
    public bool bIsComplete;
}

public enum QuestType
{
    메인 = 1,
    일반 = 2
}

