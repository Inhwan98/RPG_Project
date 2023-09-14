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
    [JsonProperty, SerializeField] private string m_sSkillName;
    [JsonProperty, SerializeField] private string m_sAnimParameterName; // ������ �ִϸ��̼� �̸�
    [JsonProperty, SerializeField] private string m_sToolTip; //��ų ����
    [JsonProperty, SerializeField] private string m_sSpritePath;
    [JsonProperty, SerializeField] private int    m_nSkillDamagePer; // ��ų % ������
    [JsonProperty, SerializeField] private float  m_fCooldown;
    [JsonProperty, SerializeField] private float  m_fManaAmount; // �䱸 ����
    

    private Sprite m_iSprite;
    private bool m_bInUse;
    private int anim_Hash;
    private float m_fSkillDamage;

    //public int  GetHash() { return anim_hash; }
    //public void SetHash(int _anim_Hash) { this.anim_hash = _anim_Hash; }

    /// <summary> �ִϸ��̼��� Parameter�� int�� �ؽ� �Ѵ�. </summary>
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

    /// <summary> �ִϸ��̼��� �ؽ��ڵ� </summary>
    public int GetAnimHash() { return anim_Hash; }

    public float GetSkillManaAmount() { return m_fManaAmount; }

    /// <summary> ��ų�� ������ </summary>
    public float GetSkillDamagePer() { return m_nSkillDamagePer; }
    public void SetSkillDamagePer(int _skillDamagePer) { m_nSkillDamagePer = _skillDamagePer; }

    public float GetSkillDamage() { return m_fSkillDamage; }
    public void SetSkillDamage(float _power)
    {
        m_fSkillDamage = (m_nSkillDamagePer/100 * _power) + _power;
    }

    //��ų�̹���
    public Sprite GetSkill_Sprite() { return m_iSprite; }

    //��ų ��� ����
    public bool GetInUse() { return m_bInUse; }
    public void SetInUse(bool _bInUse) { this.m_bInUse = _bInUse; }

    //�ִϸ��̼� �̸�
    public string GetAnimName() { return m_sAnimParameterName; }

    //��ų�� ��Ÿ��
    public float GetCoolDown() { return m_fCooldown; }
}
