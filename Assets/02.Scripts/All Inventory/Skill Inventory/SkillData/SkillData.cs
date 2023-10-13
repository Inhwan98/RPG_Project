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
    [JsonProperty] private int    m_nID;
    [JsonProperty] private string m_sSkillName;
    [JsonProperty] private int    m_nSkillLevel;
    [JsonProperty] private int    m_nUsedLevel; // 습득 조건 레벨
    [JsonProperty] private string m_sAnimParameterName; // 동작할 애니메이션 이름
    [JsonProperty] private string m_sToolTip; //스킬 설명
    [JsonProperty] private string m_sSpritePath;
    [JsonProperty] private int    m_nSkillDamagePer; // 스킬 % 데미지
    [JsonProperty] private float  m_fCooldown;
    [JsonProperty] private int    m_nManaAmount; // 요구 마나
    [JsonProperty] private bool   m_bAcquired;  // 스킬 습득 체크


    private Sprite m_iSprite;
    private bool m_bInAvailable;
    private int  m_nSkillDamage;
    private int _anim_Hash;
    

    //public int  GetHash() { return anim_hash; }
    //public void SetHash(int _anim_Hash) { this.anim_hash = _anim_Hash; }

   
    public void Init()
    {
        SetAnimHash();
        SetSpriteImage();
    }

    /// <summary> 애니메이션의 Parameter를 int로 해싱 한다. </summary>
    public void SetAnimHash()
    {
        _anim_Hash = Animator.StringToHash(m_sAnimParameterName);
    }

    public void SetSpriteImage()
    {
        m_iSprite = Resources.Load<Sprite>(m_sSpritePath);
    }

    public void DisPlay()
    {
        Debug.Log(m_sSkillName);
        Debug.Log(m_sAnimParameterName);
        Debug.Log(m_sToolTip);
        Debug.Log(m_sSpritePath);
        Debug.Log(m_nSkillDamagePer);
        Debug.Log(m_fCooldown);
        Debug.Log(m_nManaAmount);
    }



    public int GetID() => m_nID;

    /// <summary> 스킬 이름 반환 </summary>
    public string GetSKillName() => m_sSkillName;

    public int GetSkillLevel() => m_nSkillLevel;
    public void SetSkillLevel(int value) => m_nSkillLevel = value;

    public int GetSkillUsedLevel() => m_nUsedLevel;

    /// <summary> 툴팁 내용 반환 </summary>
    public string GetToolTip() => m_sToolTip;

    /// <summary> 스킬 획득 여부 </summary>
    public bool GetIsAcquired() => m_bAcquired;
    /// <summary> 스킬 획득 여부 수정 </summary>
    public void SetIsAcquired(bool value) => m_bAcquired = value;

    /// <summary> 애니메이션의 해쉬코드 </summary>
    public int GetAnimHash() { return _anim_Hash; }

    public int GetSkillManaAmount() { return m_nManaAmount; }

    /// <summary> 스킬의 데미지 </summary>
    public float GetSkillDamagePer() { return m_nSkillDamagePer; }
    public void SetSkillDamagePer(int _skillDamagePer) { m_nSkillDamagePer = _skillDamagePer; }

    public int GetSkillDamage() { return m_nSkillDamage; }
    public void SetSkillDamage(float _power)
    {
        m_nSkillDamage = (int)(m_nSkillDamagePer/100.0 * _power);
    }

    //스킬이미지
    public Sprite GetSkill_Sprite() { return m_iSprite; }

    //스킬 사용 여부
    public bool GetInAvailable() { return m_bInAvailable; }
    
    public void SetInAvailable(bool value) { this.m_bInAvailable = value; }

    //애니메이션 이름
    public string GetAnimName() { return m_sAnimParameterName; }

    //스킬의 쿨타임
    public float GetCoolDown() { return m_fCooldown; }

    public void LevelUP()
    {
        m_nSkillLevel += 1;

        m_nSkillDamagePer = (int)(m_nSkillDamagePer * 1.25f);
        m_fCooldown -= 1;
        m_nManaAmount = (int)(m_nManaAmount * 1.5f);
    }
}
