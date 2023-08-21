using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField, Header("�÷��̾��� ��ų ����")] private float fAttackRange;
    [Space(5)]
    [SerializeField] private int m_nSkillDamagePer;
    [SerializeField] private float m_fCooldown;
    [SerializeField] private float m_fManaAmount; // �䱸 ����
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private SkillType skillType;
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

    public string GetAnimName() { return animParameterName; }

    public float GetCoolDown() { return m_fCooldown; }

    //���ݹ���
    public float GetAttackRange() { return fAttackRange; }
    public void SetAttackRange(float _fAttackRange) { this.fAttackRange = _fAttackRange; }

    public GameObject GetEffectObj() { return effectPrefab; }

    public SkillType GetSkillType() { return skillType; }
}
