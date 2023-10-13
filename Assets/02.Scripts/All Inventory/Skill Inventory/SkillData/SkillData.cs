using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

[System.Serializable]
public enum SkillType
{
    SINGLE = 0,     //���� ����
    ARROUND_PLAYER, //�÷��̾��� �ֺ�
    ARROUND_TARGET, //��ǥ�� �ֺ�
    BUFF_SELF,      //���� ���� 
    BUFF_TARGET     //Ÿ�� ����
}

[System.Serializable]
public class SkillData
{
    [JsonProperty] private int    m_nID;
    [JsonProperty] private string m_sSkillName;
    [JsonProperty] private int    m_nSkillLevel;
    [JsonProperty] private int    m_nUsedLevel; // ���� ���� ����
    [JsonProperty] private string m_sAnimParameterName; // ������ �ִϸ��̼� �̸�
    [JsonProperty] private string m_sToolTip; //��ų ����
    [JsonProperty] private string m_sSpritePath;
    [JsonProperty] private int    m_nSkillDamagePer; // ��ų % ������
    [JsonProperty] private float  m_fCooldown;
    [JsonProperty] private int    m_nManaAmount; // �䱸 ����
    [JsonProperty] private bool   m_bAcquired;  // ��ų ���� üũ


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

    /// <summary> �ִϸ��̼��� Parameter�� int�� �ؽ� �Ѵ�. </summary>
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

    /// <summary> ��ų �̸� ��ȯ </summary>
    public string GetSKillName() => m_sSkillName;

    public int GetSkillLevel() => m_nSkillLevel;
    public void SetSkillLevel(int value) => m_nSkillLevel = value;

    public int GetSkillUsedLevel() => m_nUsedLevel;

    /// <summary> ���� ���� ��ȯ </summary>
    public string GetToolTip() => m_sToolTip;

    /// <summary> ��ų ȹ�� ���� </summary>
    public bool GetIsAcquired() => m_bAcquired;
    /// <summary> ��ų ȹ�� ���� ���� </summary>
    public void SetIsAcquired(bool value) => m_bAcquired = value;

    /// <summary> �ִϸ��̼��� �ؽ��ڵ� </summary>
    public int GetAnimHash() { return _anim_Hash; }

    public int GetSkillManaAmount() { return m_nManaAmount; }

    /// <summary> ��ų�� ������ </summary>
    public float GetSkillDamagePer() { return m_nSkillDamagePer; }
    public void SetSkillDamagePer(int _skillDamagePer) { m_nSkillDamagePer = _skillDamagePer; }

    public int GetSkillDamage() { return m_nSkillDamage; }
    public void SetSkillDamage(float _power)
    {
        m_nSkillDamage = (int)(m_nSkillDamagePer/100.0 * _power);
    }

    //��ų�̹���
    public Sprite GetSkill_Sprite() { return m_iSprite; }

    //��ų ��� ����
    public bool GetInAvailable() { return m_bInAvailable; }
    
    public void SetInAvailable(bool value) { this.m_bInAvailable = value; }

    //�ִϸ��̼� �̸�
    public string GetAnimName() { return m_sAnimParameterName; }

    //��ų�� ��Ÿ��
    public float GetCoolDown() { return m_fCooldown; }

    public void LevelUP()
    {
        m_nSkillLevel += 1;

        m_nSkillDamagePer = (int)(m_nSkillDamagePer * 1.25f);
        m_fCooldown -= 1;
        m_nManaAmount = (int)(m_nManaAmount * 1.5f);
    }
}
