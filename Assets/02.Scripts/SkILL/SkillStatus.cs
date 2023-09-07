using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
public class SkillStatus
{
    [SerializeField] private string skillName;
    [SerializeField] private string animParameterName; // ������ �ִϸ��̼� �̸�
    [Space(5)]
    [SerializeField] private int m_nSkillDamagePer; // ��ų % ������
    [SerializeField] private float m_fCooldown;
    [SerializeField] private float m_fManaAmount; // �䱸 ����
    [SerializeField] private GameObject effectPrefab; // ����Ʈ ȿ��
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

    //��ų������
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
    public string GetAnimName() { return animParameterName; }

    //��ų�� ��Ÿ��
    public float GetCoolDown() { return m_fCooldown; }

    public GameObject GetEffectObj() { return effectPrefab; }

    public SkillType GetSkillType() { return skillType; }
}
