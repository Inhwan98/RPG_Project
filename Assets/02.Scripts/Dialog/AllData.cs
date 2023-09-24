[System.Serializable]
public class AllData
{
    public DialogData[] DialogDB;
    public QuestData[] QuestDB;
    public MonsterData[] MonsterDB;
    public PlayerLevelData[] PlayerLevelDB;
    public SkillData[] SkillDB;
}

[System.Serializable]
public class QuestData
{
    public int nQuestID;
    public int nBranch;
    public int nConditionLevel;
    public int nID;
    public int nDestID;
    public int eQuestType;
    public int eObjectives;
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

public enum QuestObjectives
{
    CONVERSATION = 1,
    HUNT,
    SKILL
}


[System.Serializable]
public class DialogData
{
    public int nQuestID;
    public int nBranch;
    public string sName;
    public string sDialog;
}

[System.Serializable]
public class PlayerLevelData
{
    public int nLevel;
    public int nMaxHP;
    public int nMaxMP;
    public int nSTR;
    public int ntotalExp;
}

[System.Serializable]
public class MonsterData
{
    public int nID;
    public string sName;
    public int nLevel;
    public int nMaxHP;
    public int nCurHP;
    public int nMaxMP;
    public int nCurSTR;
    public int nDropExp;
}




