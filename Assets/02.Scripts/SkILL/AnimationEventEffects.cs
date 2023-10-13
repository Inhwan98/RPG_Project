﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectData
{
    public int nID;

    public string sName;

    public float[] fPosArray;

    public float[] fRotArray;

    [Newtonsoft.Json.JsonProperty]
    private float fDisableAfter;
    [Newtonsoft.Json.JsonProperty]
    private bool bUseLocalPosition;

    [Newtonsoft.Json.JsonIgnore]
    [SerializeField]
    private Vector3 pos;
    [Newtonsoft.Json.JsonIgnore]
    [SerializeField]
    private Quaternion rot;

    public void PosAndRot_Init()
    {
        pos = new Vector3(fPosArray[0], fPosArray[1], fPosArray[2]);
        rot = Quaternion.Euler(fRotArray[0], fRotArray[1], fRotArray[2]);
    }

    public float GetDisableAfter() => fDisableAfter;
    public bool GetUseLocalPos() => bUseLocalPosition;

    public Vector3 GetPos() => pos;
    public Quaternion GetRot() => rot;
}


public class AnimationEventEffects : MonoBehaviour
{
    protected EffectData[] _effectDatas; //GameManger Awake에서 Load

    protected GameObject[] skillEffects;
    protected List<WaitForSeconds> waitSecondList = new List<WaitForSeconds>();

    protected GameObject skillEffectGo;

    protected virtual void Awake()
    {
        //skillEffects = Resources.LoadAll<GameObject>("SkillEffect");
    }

    protected virtual void Start()
    {
        for (int i = 0; i < skillEffects.Length; i++)
        {
            skillEffects[i] = Instantiate(skillEffects[i]);
            skillEffects[i].SetActive(false);
            skillEffects[i].transform.SetParent(transform);
            skillEffects[i].transform.localPosition = _effectDatas[i].GetPos();
            skillEffects[i].transform.localRotation = _effectDatas[i].GetRot();

            waitSecondList.Add(new WaitForSeconds(_effectDatas[i].GetDisableAfter()));
        }
    }


    /// <summary> GameManager Awake()에서 Load </summary>
    public void SetEffectData(EffectData[] effectDatas)
    {
        this._effectDatas = effectDatas;

        foreach (var a in _effectDatas)
        {
            a.PosAndRot_Init();
        }

    }

    void InstantiateEffect(int EffectNumber)
    {
        if (_effectDatas == null || _effectDatas.Length <= EffectNumber)
        {

            Debug.LogError("Incorrect effect number or effect is null");
        }

        skillEffectGo = skillEffects[EffectNumber];


        if (_effectDatas[EffectNumber].GetUseLocalPos())
        {
            skillEffectGo.transform.SetParent(null);
        }

        SkillEffectInit(EffectNumber);

        skillEffectGo.SetActive(true);

        StartCoroutine(SetActive(EffectNumber));
    }

    protected virtual void SkillEffectInit(int EffectNumber)
    {

    }

    IEnumerator SetActive(int index)
    {
        yield return waitSecondList[index];
        skillEffects[index].SetActive(false);
        skillEffects[index].transform.SetParent(transform);
        skillEffects[index].transform.localPosition = _effectDatas[index].GetPos();
        skillEffects[index].transform.localRotation = _effectDatas[index].GetRot();
    }
}