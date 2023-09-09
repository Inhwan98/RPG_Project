using UnityEngine;

[System.Serializable]
public class ObjectData
{
    [SerializeField] private int nLevel;

    [Header("몬스터, 플레이어 체력")]
    [SerializeField] private float fMaxHP;
    [SerializeField] private float fCurHP;

    [Header("몬스터, 플레이어 마나")]
    [SerializeField] private float fMaxMP;
    [SerializeField] private float fCurMP;

    [Header("몬스터, 플레이어 공격력")]
    [SerializeField] private float fCurSTR;

    [SerializeField, Header("몬스터, 플레이어 경험치"), Tooltip("몬스터는 주는 경험치, 플레이어는 받는 경험치를 의미 한다.")]
    private int nCurExp;

    public ObjectData(ObjectBase objBase)
    {
        nLevel  = objBase.GetLevel();
        fMaxHP  = objBase.GetMaxHP();
        fCurHP  = objBase.GetCurHP();

        fMaxMP  = objBase.GetMaxHP();
        fCurMP  = objBase.GetCurMP();
        nCurExp = objBase.GetCurExp();
    }

    #region Get, Set Func
    public float GetMaxHP() { return fMaxHP; }
    public float GetCurHP() { return fCurHP; }

    public float GetMaxMP() { return fMaxMP; }
    public float GetCurMP() { return fCurMP; }

    public float GetCurSTR() { return fCurSTR; }

    public int GetLevel() { return nLevel; }

    public int GetCurExp() { return nCurExp; }

    public void LevelUP()
    {
        nLevel       += 1;
        fMaxHP       *= 1.5f;
        fMaxMP       *= 1.4f;
        fCurSTR      *= 1.5f;
        nCurExp       = 0;
    }

    public void Start_Init()
    {
        nLevel = 1;
        fMaxHP = 100;
        fCurHP = fMaxHP;

        fMaxMP = 100;
        fCurMP = fMaxHP;

        fCurSTR = 10;
        nCurExp = 0;
    }


    #endregion
}

