using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    
    [상속 구조]

    AnimationEventEffects(abstract)
        
        * 플레이어
        - PlayerAnimationEventEffect : AnimEvent에 맞춰서 생성될 Player skillEffects를 Reousces.Load 함
            
        * 드래곤 보스
        - DragonAnimEffects  : AnimEvent에 맞춰서 생성될 Boss skillEffects를 Reousces.Load 함
                               Boss의 스킬 데이터 초기화.  ex) 보스의 공격력 할당, 스킬의 목표지점
*/




public abstract class AnimationEventEffects : MonoBehaviour
{
    /***********************************************
     *                  Fields
     ***********************************************/
    #region protected Fields
    protected EffectData[] _effectDatas; //GameManger Awake에서 Load
    protected GameObject[] skillEffectsGo;
    protected GameObject _skillEffectGo;
    protected GameObject[,] _multipleSkillEffectsGo;
    protected List<WaitForSeconds> waitSecondList = new List<WaitForSeconds>();
    #endregion

    /***********************************************
     *                 Unity Evenet
     ***********************************************/
    #region Unity Event
    protected virtual void Awake()
    {
        Init_Effects();
    }

    protected virtual void Start()
    {
         CreateSkillEffectPool();
    }
    #endregion

    /***********************************************
     *                    Methods
     ***********************************************/
    #region Animation Event Methods
    /// <summary>
    /// 이펙트의 활성화 (오브젝트 풀링)
    /// </summary>
    void InstantiateEffect(int EffectNumber)
    {
        if (_effectDatas == null || _effectDatas.Length <= EffectNumber)
        {

            Debug.LogError("Incorrect effect number or effect is null");
        }

        //다중 스킬이 아닌 경우
        if(!_effectDatas[EffectNumber].bIsMultiple)
        {
            _skillEffectGo = skillEffectsGo[EffectNumber];

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
    #endregion

    #region abstract Methods
    protected abstract void SkillEffectSetting(GameObject skillEffectGo, int EffectNumber);
    protected abstract void Init_Effects();
    #endregion

    #region private Methods
    /// <summary>
    /// 스킬이펙트를 오브젝트 풀링하기 위해 미리 풀을 생성해 둔다.
    /// 다중생성 공격 이펙트와 구분한다.
    /// </summary>
    private void CreateSkillEffectPool()
    {
        _multipleSkillEffectsGo = new GameObject[skillEffectsGo.Length, 10];

        for (int i = 0; i < skillEffectsGo.Length; i++)
        {
            if (_effectDatas[i].bIsMultiple)
            {
                for (int k = 0; k < _effectDatas[i].nMultipleAmount; k++)
                {
                    _multipleSkillEffectsGo[i, k] = Instantiate(skillEffectsGo[i]);

                    GameObject curEffectGo = _multipleSkillEffectsGo[i, k];

                    SkillEffect_Init(curEffectGo, i);
                }
            }
            else
            {
                skillEffectsGo[i] = Instantiate(skillEffectsGo[i]);
                GameObject curEffectGo = skillEffectsGo[i];

                SkillEffect_Init(curEffectGo, i);
            }

        }
    }

    /// <summary> Effect 활성화 함수 </summary>
    private void EffectStart(GameObject skillEffectGo, int effectNumber)
    {
        if (_effectDatas[effectNumber].GetUseLocalPos())
        {
            skillEffectGo.transform.SetParent(null);
        }
        SkillEffectSetting(skillEffectGo, effectNumber);

        skillEffectGo.SetActive(true);
        StartCoroutine(SetActive(skillEffectGo, effectNumber));
    }
    #endregion

    #region public Methods
    /// <summary> GameManager Awake()에서 Load </summary>
    public void SetEffectData(EffectData[] effectDatas)
    {
        this._effectDatas = effectDatas;

        foreach (var a in _effectDatas)
        {
            a.PosAndRot_Init();
        }
    }

    /// <summary> 스킬 이펙트의 정보 초기화 </summary>
    public void SkillEffect_Init(GameObject effectGo, int idx)
    {
        effectGo.SetActive(false);
        effectGo.transform.SetParent(transform);
        effectGo.transform.localPosition = _effectDatas[idx].GetPos();
        effectGo.transform.localRotation = _effectDatas[idx].GetRot();

        waitSecondList.Add(new WaitForSeconds(_effectDatas[idx].GetDisableAfter()));
    }
    #endregion


    /***********************************************
     *                    Coroutine
     ***********************************************/

    #region Coroutine
    /// <summary> 일정 시간 이후 활성 / 비활성화 </summary>
    IEnumerator SetActive(GameObject curEffectGo, int index)
    {
        yield return waitSecondList[index];
        curEffectGo.SetActive(false);
        curEffectGo.transform.SetParent(transform);
        curEffectGo.transform.localPosition = _effectDatas[index].GetPos();
        curEffectGo.transform.localRotation = _effectDatas[index].GetRot();
    }
    #endregion
}