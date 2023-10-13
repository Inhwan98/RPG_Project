using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventEffect : AnimationEventEffects
{
    protected override void Awake()
    {
        skillEffects = Resources.LoadAll<GameObject>("SkillEffect");
    }
}