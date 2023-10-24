using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventEffect : AnimationEventEffects
{
    protected override void Init_Effects()
    {
        skillEffectsGo = Resources.LoadAll<GameObject>("SkillEffect");
    }

    protected override void SkillEffectSetting(GameObject skillEffectGo, int EffectNumber)
    {
        return;
    }
}
