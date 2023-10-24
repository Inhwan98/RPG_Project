using UnityEngine;
using Newtonsoft.Json;

public enum BossPhase
{
    ONE,
    TWO,
}

/// <summary>
/// 보스가 스킬을 사용할 시 동작할 데이터를 가지고 있다.
/// <para>실행할 애니메이션 데이터도 지니고있는데, 이 애니메이션클립에 스킬 이펙트 생성에 대한 Event 클립이 추가 되어 있다.</para>
/// </summary>
[System.Serializable]
public class BossSkillData
{
    /**************************************
     *             Fields
     **************************************/
    
    #region serialize Fields
    [JsonProperty] private int m_nID;
    [JsonProperty] private string m_sSkillName;
    [JsonProperty] private string m_sAnimParameterName; // 동작할 애니메이션 이름
    [JsonProperty] private int m_nSkillDamagePer; // 스킬 % 데미지
    [JsonProperty] private float m_fCooldown;
    #endregion
    #region private Fields
    private int m_nSkillDamage;
    private int m_nAnim_Hash;
    #endregion

    /**************************************
    *           Get, Set Methods
    **************************************/
    #region Get Methods
    /// <summary> 몬스터 ID 반환 </summary>
    public int GetID() => m_nID;
    /// <summary> 스킬 이름 반환 </summary>
    public string GetSKillName()     => m_sSkillName;
    /// <summary> 애니메이션의 해쉬코드 </summary>
    public int GetAnimHash()         => m_nAnim_Hash;
    /// <summary> 스킬의 데미지 </summary>
    public float GetSkillDamagePer() => m_nSkillDamagePer;
    public int GetSkillDamage()      => m_nSkillDamage;
    //애니메이션 이름
    public string GetAnimName()      => m_sAnimParameterName;
    //스킬의 쿨타임
    public float GetCoolDown()       => m_fCooldown;
    #endregion

    #region Set Methods
    public void SetSkillDamagePer(int _skillDamagePer) => m_nSkillDamagePer = _skillDamagePer;
    /// <summary> 애니메이션의 Parameter를 int로 해싱 한다. </summary>
    public void SetAnimHash() => m_nAnim_Hash = Animator.StringToHash(m_sAnimParameterName);
    public void SetSkillDamage(float _power)
    {
        m_nSkillDamage = (int)(m_nSkillDamagePer / 100.0 * _power);
    }
    #endregion

    /**************************************
     *             Methods
     **************************************/

    #region public Methods
    public void Init(int power)
    {
        SetAnimHash();
        SetSkillDamage(power);
    }
    #endregion
}

