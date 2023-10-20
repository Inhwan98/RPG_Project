using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectData
{
    public int nID;

    public bool bIsMultiple;

    public int nMultipleAmount;

    public string sName;

    public float[] fPosArray;

    public float[] fRotArray;

    [Newtonsoft.Json.JsonProperty]
    private float fDisableAfter;
    [Newtonsoft.Json.JsonProperty]
    private bool bUseLocalPosition;


    private Vector3 pos;
    private Quaternion rot;

    public void PosAndRot_Init()
    {
        pos = new Vector3(fPosArray[0], fPosArray[1], fPosArray[2]);
        rot = Quaternion.Euler(fRotArray[0], fRotArray[1], fRotArray[2]);
    }
    public Vector3 GetPos() => pos;
    public Quaternion GetRot() => rot;

    public float GetDisableAfter() => fDisableAfter;
    public bool GetUseLocalPos() => bUseLocalPosition;
}


public class AnimationEventEffects : MonoBehaviour
{
    protected EffectData[] _effectDatas; //GameManger Awake에서 Load

    protected GameObject[] skillEffects;
    protected List<WaitForSeconds> waitSecondList = new List<WaitForSeconds>();

    protected GameObject _skillEffectGo;

    protected GameObject[,] _multipleSkillEffectsGo;

    protected virtual void Awake()
    {
        //skillEffects = Resources.LoadAll<GameObject>("SkillEffect");
    }

    protected virtual void Start()
    {
        _multipleSkillEffectsGo = new GameObject[skillEffects.Length, 10];

        for (int i = 0; i < skillEffects.Length; i++)
        {
            if(_effectDatas[i].bIsMultiple)
            {
                for(int k = 0; k < _effectDatas[i].nMultipleAmount; k++)
                {
                    _multipleSkillEffectsGo[i, k] = Instantiate(skillEffects[i]);

                    GameObject curEffectGo = _multipleSkillEffectsGo[i, k];

                    SkillEffect_Init(curEffectGo, i);
                }
                
            }
            else
            {
                skillEffects[i] = Instantiate(skillEffects[i]);
                GameObject curEffectGo = skillEffects[i];

                SkillEffect_Init(curEffectGo, i);
            }
            
        }
    }

    public void SkillEffect_Init(GameObject effectGo, int idx)
    {
        effectGo.SetActive(false);
        effectGo.transform.SetParent(transform);
        effectGo.transform.localPosition = _effectDatas[idx].GetPos();
        effectGo.transform.localRotation = _effectDatas[idx].GetRot();

        waitSecondList.Add(new WaitForSeconds(_effectDatas[idx].GetDisableAfter()));
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

        //다중 스킬이 아닌 경우
        if(!_effectDatas[EffectNumber].bIsMultiple)
        {
            _skillEffectGo = skillEffects[EffectNumber];

            EffectStart(_skillEffectGo, EffectNumber);
        }
        //다중 스킬 이라면
        else
        {
            for(int i = 0; i < _effectDatas[EffectNumber].nMultipleAmount; i++)
            {
                _skillEffectGo =  _multipleSkillEffectsGo[EffectNumber, i];

                if (_skillEffectGo == null) break;

                EffectStart(_skillEffectGo, EffectNumber);
            }
        }
    }

    private void EffectStart(GameObject skillEffectGo, int effectNumber)
    {
        if (_effectDatas[effectNumber].GetUseLocalPos())
        {
            skillEffectGo.transform.SetParent(null);
        }
        SkillEffectInit(skillEffectGo, effectNumber);

        skillEffectGo.SetActive(true);
        StartCoroutine(SetActive(skillEffectGo, effectNumber));
    }

    protected virtual void SkillEffectInit(GameObject skillEffectGo, int EffectNumber)
    {

    }

    IEnumerator SetActive(GameObject curEffectGo, int index)
    {
        yield return waitSecondList[index];
        curEffectGo.SetActive(false);
        curEffectGo.transform.SetParent(transform);
        curEffectGo.transform.localPosition = _effectDatas[index].GetPos();
        curEffectGo.transform.localRotation = _effectDatas[index].GetRot();
    }
}