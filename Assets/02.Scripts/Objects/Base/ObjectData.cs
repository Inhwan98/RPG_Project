using UnityEngine;

public class ObjectData
{
    [Newtonsoft.Json.JsonProperty]
    private int nLevel;

    [Newtonsoft.Json.JsonProperty]
    private int nMaxHP;

    [Newtonsoft.Json.JsonProperty]
    private int nCurHP;

    [Newtonsoft.Json.JsonProperty]
    private int nMaxMP;
    
    [Newtonsoft.Json.JsonProperty]
    private int nCurMP;

    [Newtonsoft.Json.JsonProperty]
    private int nCurSTR;

    [Newtonsoft.Json.JsonProperty]
    private int nCurExp;

    public ObjectData()
    {
        Init();
    }

    public ObjectData(ObjectBase objBase)
    {
        nLevel  = objBase.GetLevel();

        nMaxHP  = objBase.GetMaxHP();
        nCurHP  = objBase.GetCurHP();

        nMaxMP  = objBase.GetMaxHP();
        nCurMP  = objBase.GetCurMP();
        nCurExp = objBase.GetCurExp();
    }

    public void Init()
    {
        nLevel = 0;

        nMaxHP = 0;
        nCurHP = 0;

        nMaxMP = 0;
        nCurMP = 0;
        nCurExp = 0;
    }

    #region Get, Set Func
    public int GetMaxHP() { return nMaxHP; }
    public int GetCurHP() { return nCurHP; }

    public int GetMaxMP() { return nMaxMP; }
    public int GetCurMP() { return nCurMP; }

    public int GetCurSTR() { return nCurSTR; }

    public int GetLevel() { return nLevel; }

    public int GetCurExp() { return nCurExp; }

    public void LevelUP()
    {
        nLevel   = 1;
        nMaxHP   = (int)(nMaxHP  * 1.5f);
        nMaxMP   = (int)(nMaxMP  * 1.4f);
        nCurSTR  = (int)(nCurSTR * 1.5f);
        nCurExp  = 0;
    }

    public void Start_Init()
    {
        nLevel = 1;
        nMaxHP = 100;
        nCurHP = nMaxHP;

        nMaxMP = 100;
        nCurMP = nMaxHP;

        nCurSTR = 10;
        nCurExp = 0;
    }


    #endregion
}

