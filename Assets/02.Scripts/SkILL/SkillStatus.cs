using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [Space(5)]
    [SerializeField] private int m_nSkillDamagePer; // 스킬 % 데미지
    [SerializeField] private float m_fCooldown;
    [SerializeField] private float m_fManaAmount; // 요구 마나
    [SerializeField] private GameObject effectPrefab; // 이펙트 효과
    [SerializeField] private Sprite m_iSprite;
    [SerializeField] private SkillType skillType;
    private bool m_bInUse;
    private int anim_Hash;
    private float m_fSkillDamage;

    //public int  GetHash() { return anim_hash; }
    //public void SetHash(int _anim_Hash) { this.anim_hash = _anim_Hash; }

    public void ChangeAnimHash()
    {
        anim_Hash = Animator.StringToHash(animParameterName);
    }

    public int GetAnimHash() { return anim_Hash; }

    public float GetSkillManaAmount() { return m_fManaAmount; }

    //스킬데미지
    public float GetSkillDamagePer() { return m_nSkillDamagePer; }
    public void SetSkillDamagePer(int _skillDamagePer) { m_nSkillDamagePer = _skillDamagePer; }

    public float GetSkillDamage() { return m_fSkillDamage; }
    public void SetSkillDamage(float _power)
    {
        m_fSkillDamage = (m_nSkillDamagePer/100 * _power) + _power;
    }

    //스킬이미지
    public Sprite GetSkill_Sprite() { return m_iSprite; }

    //스킬 사용 여부
    public bool GetInUse() { return m_bInUse; }
    public void SetInUse(bool _bInUse) { this.m_bInUse = _bInUse; }

    //애니메이션 이름
    public string GetAnimName() { return animParameterName; }

    //스킬의 쿨타임
    public float GetCoolDown() { return m_fCooldown; }

    public GameObject GetEffectObj() { return effectPrefab; }

    public SkillType GetSkillType() { return skillType; }
}
