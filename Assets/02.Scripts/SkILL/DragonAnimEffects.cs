using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimEffects : AnimationEventEffects
{
    BossMonster _bossMonster;
    Projectile _fireballCtr;

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

        _fireballCtr = skillEffects[0].GetComponent<Projectile>();
    }

    protected override void SkillEffectInit(int EffectNumber)
    {
        if(EffectNumber == 0)
        {
            Vector3 destPos = _bossMonster.GetDestTr().position;
            var damage = _bossMonster.GetSkillDamage();
            _fireballCtr.SetDamage(damage);

            //_fireballCtr = effectGo.GetComponent<Projectile>();
            _fireballCtr.Init(skillEffects[EffectNumber].transform.localPosition, destPos + (Vector3.up));
        }
    }
}
