

public class PlayerStatusData
{
    PlayerController m_playerCtr;

    public string m_sClassName;
    public int m_nLevel;
    public int m_nCurHP;
    public int m_nMaxHP;
    public int m_nCurMP;
    public int m_nMaxMP;
    public int m_nStr;
    public int m_MinPower;
    public int m_nMaxPower;
    public int m_nDefence;


    public PlayerStatusData(PlayerController playerCtr)
    {
        m_playerCtr = playerCtr;
        UpdateStatusData();
    }


    /// <summary> Player StatusInfo Update</summary>
    public void UpdateStatusData()
    {
        m_sClassName = m_playerCtr.GetClassName();
        m_nLevel     = m_playerCtr.GetLevel();
        m_nCurHP     = m_playerCtr.GetCurHP();
        m_nMaxHP     = m_playerCtr.GetMaxHP();
        m_nCurMP     = m_playerCtr.GetCurMP();
        m_nMaxMP     = m_playerCtr.GetMaxMP();
        m_nStr       = m_playerCtr.GetCurStr();
        m_MinPower   = m_nStr * m_playerCtr.GetMinAttack();
        m_nMaxPower  = m_nStr * m_playerCtr.GetMaxAttack();
        m_nDefence   = m_playerCtr.GetDefence();
    }
}
