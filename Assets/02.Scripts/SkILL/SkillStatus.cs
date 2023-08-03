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
    [SerializeField] private float fCooldown;
    [SerializeField] private float manaAmount; // �䱸 ����
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
