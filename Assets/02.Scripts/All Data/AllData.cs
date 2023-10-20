[System.Serializable]
public class AllData
{
    public DialogData[] DialogDB;
    public QuestData[] QuestDB;
    public MonsterData[] MonsterDB;
    public NPCData[] NPCDB;
    public DungeonStage[] DungeonStageDB;
    public StageNPCData[] StageNPCDB;
    public PlayerLevelData[] PlayerLevelDB;
    public SkillData[] SkillDB;
    public SkillData[] PlayerSkillDB;
    public EffectData[] SkillEffectDB;
    public EffectData[] BossSkillEffectDB;
    public BossSkillData[] BossMeleeAttackDB;
    public BossSkillData[] BossRangeAttackDB;
    public ArmorItemData[] ArmorItemDB;
    public WeaponItemData[] WeaponItemDB;
    public PortionItemData[] PortionItemDB;
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
    public int nCurCnt;
    public int nRewardEXP;
    public int nRewardGold;
    public int[] nRewardItem;
    public int[] nItemAmount;
    public bool bIsComplete;
    public bool bIsProgress;
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
    public int nDefence;
    public int nDropExp;
    public int nDropMoney;
    public int[] nDropItemArray;
}
[System.Serializable]
public class PlayerData
{
    public int nLevel;
    public int nMaxHP;
    public int nCurHP;
    public int nMaxMP;
    public int nCurMP;
    public int nCurSTR;
    public int ntotalExp;
    public int nCurExp;
    public int nRubyAmount;

    public string sVillageScene;

    public PlayerData() { }

    public PlayerData(PlayerController playerCtr)
    {
        nLevel = playerCtr.GetLevel();

        nMaxHP = playerCtr.GetMaxHP();
        nCurHP = playerCtr.GetCurHP();

        nMaxMP = playerCtr.GetMaxHP();
        nCurMP = playerCtr.GetCurMP();

        nCurSTR = playerCtr.GetCurStr();

        ntotalExp = playerCtr.GetMaxExp();
        nCurExp = playerCtr.GetCurExp();

        nRubyAmount = playerCtr.GetRubyAmount();

        sVillageScene = playerCtr.GetPlayerStayingVilliage();
    }
}
[System.Serializable]
public class PlayerLevelData
{
    public int nLevel;
    public int nMaxHP;
    public int nMaxMP;
    public int nSTR;
    public int nDefence;
    public int ntotalExp;
}




