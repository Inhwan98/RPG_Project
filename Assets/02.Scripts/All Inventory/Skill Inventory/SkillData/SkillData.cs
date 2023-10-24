using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

/* 사용
 * 
 *   -SkILL Tree의 Skill Data
 *   -Player가 습득한 Skill Data
 *   
 */


[System.Serializable]
public class SkillData
{
    /**********************************************
     *                Field
     **********************************************/
    #region option Field
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
    #endregion
    #region private Field
    private Sprite m_iSprite;
    private bool m_bInAvailable;
    private int m_nSkillDamage;
    private int _anim_Hash;
    #endregion

    /**********************************************
    *                Get, Set Methods
    **********************************************/
    #region Get
    public int GetID() => m_nID;
    /// <summary> 스킬 이름 반환 </summary>
    public string GetSKillName() => m_sSkillName;
    public int GetSkillLevel() => m_nSkillLevel;
    /// <summary> 스킬 제한 레벨값 반환</summary>
    public int GetSkillUsedLevel() => m_nUsedLevel;
    /// <summary> 툴팁 내용 반환 </summary>
    public string GetToolTip() => m_sToolTip;
    /// <summary> 스킬 획득 여부 </summary>
    public bool GetIsAcquired() => m_bAcquired;
    /// <summary> 스킬 데미지 값 반환 </summary>
    public int GetSkillDamage() { return m_nSkillDamage; }
    /// <summary> 애니메이션의 해쉬코드 </summary>
    public int GetAnimHash() { return _anim_Hash; }
    /// <summary> 마나 소모량 값 반환 </summary>
    public int GetSkillManaAmount() { return m_nManaAmount; }
    /// <summary> 스킬의 데미지 </summary>
    public float GetSkillDamagePer() { return m_nSkillDamagePer; }
    ///<summary> 스킬 sprite 반환  </summary>
    public Sprite GetSkill_Sprite() { return m_iSprite; }
    /// <summary>  애니메이션 이름 </summary>
    public string GetAnimName() { return m_sAnimParameterName; }
    /// <summary>  스킬의 쿨타임 </summary>
    public float GetCoolDown() { return m_fCooldown; }
    ///<summary>  스킬 사용 여부 </summary>
    public bool GetInAvailable() { return m_bInAvailable; }
    #endregion
    #region Set Methods
    public void SetSkillDamagePer(int _skillDamagePer) { m_nSkillDamagePer = _skillDamagePer; }
    /// <summary> 스킬 획득 여부 수정 </summary>
    public void SetIsAcquired(bool value) => m_bAcquired = value;
    /// <summary> 스킬의 레벨 </summary>
    public void SetSkillLevel(int value) => m_nSkillLevel = value;
    ///<summary>  스킬 사용 여부 재설정 </summary>
    public void SetInAvailable(bool value) { this.m_bInAvailable = value; }
    /// <summary> 스킬 대미지 설정 </summary>
    public void SetSkillDamage(float _power)
    {
        m_nSkillDamage = (int)(m_nSkillDamagePer / 100.0 * _power);
    }
    #endregion

    /**********************************************
    *                   Methods
    **********************************************/
    #region public Methods
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
    /// <summary> 스프라이트 이미지를 Path를 통해 불러온다. </summary>
    public void SetSpriteImage()
    {
        m_iSprite = Resources.Load<Sprite>(m_sSpritePath);
    }
    /// <summary> 스킬의 레벨 업 시스템 </summary>
    public void LevelUP()
    {
        m_nSkillLevel += 1;
        m_nUsedLevel += 3;
        m_nSkillDamagePer = (int)(m_nSkillDamagePer * 1.25f);
        m_fCooldown -= 1;
        m_nManaAmount = (int)(m_nManaAmount * 1.5f);
    }
    #endregion
}
