using UnityEngine;

public class ObjectData
{
    [Newtonsoft.Json.JsonProperty]
    private int nLevel;

    [Newtonsoft.Json.JsonProperty]
    private float fMaxHP;

    [Newtonsoft.Json.JsonProperty]
    private float fCurHP;

    [Newtonsoft.Json.JsonProperty]
    private float fMaxMP;
    
    [Newtonsoft.Json.JsonProperty]
    private float fCurMP;

    [Newtonsoft.Json.JsonProperty]
    private float fCurSTR;

    [Newtonsoft.Json.JsonProperty]
    private int nCurExp;

    public ObjectData()
    {
        Init();
    }

    public ObjectData(ObjectBase objBase)
    {
        nLevel  = objBase.GetLevel();

        fMaxHP  = objBase.GetMaxHP();
        fCurHP  = objBase.GetCurHP();

        fMaxMP  = objBase.GetMaxHP();
        fCurMP  = objBase.GetCurMP();
        nCurExp = objBase.GetCurExp();
    }

    public void Init()
    {
        nLevel = 0;

        fMaxHP = 0;
        fCurHP = 0;

        fMaxMP = 0;
        fCurMP = 0;
        nCurExp = 0;
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

