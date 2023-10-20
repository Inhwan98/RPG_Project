using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimEffects : AnimationEventEffects
{
    BossMonster _bossMonster;

    ProjectileController _fireballCtr;
    SpellAttackController[] _spellAttackCtr = new SpellAttackController[10];

    int _skilldamage;
   
    protected override void Awake()
    {
         skillEffects = Resources.LoadAll<GameObject>("BossSkillEffect/DragonInferno");
         
        _bossMonster = GetComponent<Monster>() as BossMonster;
    }

    protected override void Start()
    {
        var bossEffectDB = GameManager.instance.GetAllData().BossSkillEffectDB;
        SetEffectData(bossEffectDB);

        base.Start();

        

        for (int i = 0; i < skillEffects.Length; i++)
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
                _fireballCtr = skillEffects[0].GetComponent<ProjectileController>();
            }

        }
    }
        

    protected override void SkillEffectInit(GameObject curEffect, int EffectNumber)
    {
        Vector3 destPos = _bossMonster.GetDestTr().position;
        int nSkillDamage = _bossMonster.GetSkillDamage();
        _skilldamage = _bossMonster.GetAttackPower(nSkillDamage);

        switch (EffectNumber)
        {
            case 0:
                _fireballCtr.SetDamage(_skilldamage);

                //_fireballCtr = effectGo.GetComponent<Projectile>();
                _fireballCtr.Init(curEffect.transform.position, destPos + (Vector3.up));
                break;

            case 1:
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

}
