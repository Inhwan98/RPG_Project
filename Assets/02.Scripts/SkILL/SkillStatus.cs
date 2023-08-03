using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SkillType
{
    SINGLE = 0,     //단일 공격
    ARROUND_PLAYER, //플레이어의 주변
    ARROUND_TARGET, //목표의 주변
    BUFF_SELF,      //셀프 버프 
    BUFF_TARGET     //타겟 버프
}

[System.Serializable]
public class SkillStatus
{
    [SerializeField] private string skillName;
    [SerializeField] private string animParameterName; // 동작할 애니메이션 이름
    [SerializeField, Header("플레이어의 스킬 범위")] private float fAttackRange;
    [Space(5)]
    [SerializeField] private float fCooldown;
    [SerializeField] private float manaAmount; // 요구 마나
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private SkillType skillType;
    private int anim_Hash;

    //public int  GetHash() { return anim_hash; }
    //public void SetHash(int _anim_Hash) { this.anim_hash = _anim_Hash; }

    public void ChangeAnimHash()
    {
        anim_Hash = Animator.StringToHash(animParameterName);
    }

    public int GetAnimHash() { return anim_Hash; }

    public float GetSkillManaAmount() { return manaAmount; }


    public string GetAnimName() { return animParameterName; }
    public float GetCoolDown() { return fCooldown; }

    public float GetAttackRange() { return fAttackRange; }
    public void SetAttackRange(float _fAttackRange) { this.fAttackRange = _fAttackRange; }

    public GameObject GetEffectObj() { return effectPrefab; }

    public SkillType GetSkillType() { return skillType; }
}
