using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

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
public class SkillData
{
    [JsonProperty, SerializeField] private string m_sSkillName;
    [JsonProperty, SerializeField] private string m_sAnimParameterName; // 동작할 애니메이션 이름
    [JsonProperty, SerializeField] private string m_sToolTip; //스킬 설명
    [JsonProperty, SerializeField] private string m_sSpritePath;
    [JsonProperty, SerializeField] private int    m_nSkillDamagePer; // 스킬 % 데미지
    [JsonProperty, SerializeField] private float  m_fCooldown;
    [JsonProperty, SerializeField] private float  m_fManaAmount; // 요구 마나
    

    private Sprite m_iSprite;
    private bool m_bInUse;
    private int anim_Hash;
    private float m_fSkillDamage;

    //public int  GetHash() { return anim_hash; }
    //public void SetHash(int _anim_Hash) { this.anim_hash = _anim_Hash; }

    /// <summary> 애니메이션의 Parameter를 int로 해싱 한다. </summary>
    /// 
    public void Init()
    {
        SetAnimHash();
        SetSpriteImage();
    }

    public void SetAnimHash()
    {
        anim_Hash = Animator.StringToHash(m_sAnimParameterName);
    }

    public void SetSpriteImage()
    {
        m_iSprite = Resources.Load<Sprite>(m_sSpritePath);
    }

    /// <summary> 애니메이션의 해쉬코드 </summary>
    public int GetAnimHash() { return anim_Hash; }

    public float GetSkillManaAmount() { return m_fManaAmount; }

    /// <summary> 스킬의 데미지 </summary>
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
    public string GetAnimName() { return m_sAnimParameterName; }

    //스킬의 쿨타임
    public float GetCoolDown() { return m_fCooldown; }
}
