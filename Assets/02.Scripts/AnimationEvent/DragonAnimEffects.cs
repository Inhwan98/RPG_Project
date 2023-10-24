using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimEffects : AnimationEventEffects
{
    enum SkillType
    {
        NONE = -1,
        FireBall,
        BigBang
    }

    /***********************************************
     *                    Fields
     ***********************************************/

    #region privaet Fields
    private BossMonster _bossMonster;
    private ProjectileController _fireballCtr;
    private SpellAttackController[] _spellAttackCtr = new SpellAttackController[10];

    private SkillType _skilltype = SkillType.NONE;
    private int _skilldamage;
    #endregion

    /***********************************************
     *                  Unity Event
     ***********************************************/

    #region Unity Event
    protected override void Start()
    {
        Init_BossEffect();

        base.Start();
        Init_SkillController();
    }
    #endregion

    /***********************************************
    *                  Methods
    ***********************************************/
    #region override Methods
    protected override void Init_Effects()
    {
        skillEffectsGo = Resources.LoadAll<GameObject>("BossSkillEffect/DragonInferno");

        _bossMonster = GetComponent<Monster>() as BossMonster;
    }

    /// <summary> 스킬이 활성화 되기전 세팅 ex) 각 스킬의 데미지, 생성 위치 </summary>
    protected override void SkillEffectSetting(GameObject curEffect, int EffectNumber)
    {
        Vector3 destPos  = _bossMonster.GetDestTr().position;
        int nSkillDamage = _bossMonster.GetSkillDamage();
        _skilldamage     = _bossMonster.GetAttackPower(nSkillDamage);
        _skilltype       = (SkillType)EffectNumber;

        switch (_skilltype)
        {
            case SkillType.FireBall:
                _fireballCtr.SetDamage(_skilldamage);

                _fireballCtr.Init(curEffect.transform.position, destPos + (Vector3.up));
                break;

            case SkillType.BigBang:
                for (int i = 0; i < _effectDatas[EffectNumber].nMultipleAmount; i++)
                {
                    if (_multipleSkillEffectsGo[EffectNumber, i] == null) break;
                    _spellAttackCtr[i].SetDamage(_skilldamage);

                    int distance = Random.Range(0, 10);
                    _multipleSkillEffectsGo[EffectNumber, i].transform.position = RandomTr.GetRandomPos(PlayerController.instance.transform, distance);
                }

                break;
        }
    }
    #endregion

    #region Init Methods

    /// <summary> GameManager에서 BossDB를 불러 온다. </summary>
    private void Init_BossEffect()
    {
        var bossEffectDB = GameManager.instance.GetAllData().BossSkillEffectDB;
        SetEffectData(bossEffectDB);
    }

    /// <summary> 각각의 스킬을 컨트롤 하기 위한 스킬마다의 컨트롤러 할당 </summary>
    private void Init_SkillController()
    {

        for (int i = 0; i < skillEffectsGo.Length; i++)
        {
            if (_effectDatas[i].bIsMultiple)
            {
                for (int k = 0; k < _effectDatas[i].nMultipleAmount; k++)
                {
                    _spellAttackCtr[k] = _multipleSkillEffectsGo[i, k].GetComponent<SpellAttackController>();
                }

            }
            else
            {
                _fireballCtr = skillEffectsGo[0].GetComponent<ProjectileController>();
            }
        }
    }
    #endregion

}
