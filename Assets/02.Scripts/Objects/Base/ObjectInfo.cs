using UnityEngine;

[System.Serializable]
public class ObjectInfo
{
    [SerializeField] private int nLevel;
 
    [SerializeField, Header("몬스터, 플레이어 체력")]           private float fMaxHP;
    [SerializeField, Header("몬스터, 플레이어 마나")]           private float fMaxMP;
    [SerializeField, Header("몬스터, 플레이어 공격력")]         private float fSTR;
    [SerializeField, Header("몬스터, 플레이어 공격 사거리")]    private float fAttackRange;

    [SerializeField, Header("몬스터, 플레이어 경험치"), Tooltip("몬스터는 주는 경험치, 플레이어는 받는 경험치를 의미 한다.")]
    private int nCurExp;


    #region Get, Set Func
    public float GetMaxHP() { return fMaxHP; }
    public void SetMaxHP(float m_fHp) { this.fMaxHP = m_fHp; }

    public float GetMaxMP() { return fMaxMP; }
    public void SetMaxMP(float m_fMp) { this.fMaxMP = m_fMp; }


    public float GetSTR() { return fSTR; }
    public void SetSTR(float m_fSTR) { this.fSTR = m_fSTR; }

    public float GetAttackRange() { return fAttackRange; }
    public void SetAttackRange(int m_nAttackRange) { this.fAttackRange = m_nAttackRange; }

    public int GetLevel() { return nLevel; }
    public void SetLevel(int m_nLevel) { this.nLevel+= m_nLevel; }

    public int GetExp() { return nCurExp; }
    public void SetExp(int m_nExp) { this.nCurExp += m_nExp; }

    public void LevelUP()
    {
        nLevel       += 1;
        fMaxHP       *= 1.5f;
        fMaxMP       *= 1.4f;
        fSTR         *= 1.5f;
        fAttackRange *= 1.1f;
        nCurExp       = 0;
    }
    #endregion
}

