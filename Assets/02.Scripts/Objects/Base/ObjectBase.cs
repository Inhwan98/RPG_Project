 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviour
{
    //Objectinfo : Serializable
    protected ObjectData objData;

    [Space(10)]

    [Header("Currnet Info")] // status의 정보에 맞게 초기화 할 것
    [SerializeField] protected int   m_nID;
    [SerializeField] protected int   m_nLevel;

    [SerializeField] protected int   m_nCurExp;
    
    [SerializeField] protected int   m_nCurHP;
    [SerializeField] protected int   m_nCurMP;
    [SerializeField] protected int   m_nCurSTR;
    [SerializeField] protected int   m_nSkillDamage;

    protected int   m_nMaxHP;
    protected int   m_nMaxMP;
    protected bool  m_bisAttack;
    protected bool  m_bisDead;
    


    protected ResourcesData _resourcesData;

    #region Animation Setting
    protected Animator _anim;
    protected readonly int hashDead = Animator.StringToHash("DoDead");
    #endregion

    public int GetLevel() => m_nLevel;

    public int GetMaxHP() => m_nMaxHP;
    public int GetCurHP() => m_nCurHP;

    public int GetMaxMP() => m_nMaxMP;
    public int GetCurMP() => m_nCurMP;

    public int GetCurStr() => m_nCurSTR;
    public int GetCurExp() => m_nCurExp;

    /// <summary> Resource Data 클래스 </summary>
    //protected ResourcesData _resourcesData;

    protected virtual void Awake()
    {
        LoadData();
        InitObj();
    }

    protected virtual void Start()
    {
        _resourcesData = GameManager.instance.GetResourcesData();
    }

    protected virtual void Getinfo()
    {
        Debug.Log($"HP : {m_nCurHP}");
        Debug.Log($"STR : {m_nCurSTR}");
    }

    //Init Object Setting
    private void InitObj()
    {
        _anim = GetComponent<Animator>();
    }

    /// <summary> 스킬데미지 값 반환 </summary>
    public int GetSkillDamage() => m_nSkillDamage;

    /// <summary> 스킬데미지 값 수정 </summary>
    public void  SetSkillDamage(int _skillDamage) => m_nSkillDamage = _skillDamage;

    /// <summary> Object Die 상태 반환 </summary>
    public bool GetIsDie() => m_bisDead;

    public virtual void Buff(int _str) { }

    /// <summary> 공격 관련 </summary>
    protected abstract IEnumerator Attack();

    /// <summary> 체력이 0 이하일 때 Die Event </summary>
    protected abstract void Die();

    /// <summary> 대미지를 입었을 때 함수 </summary>
    public abstract void OnDamage(int _str);

    /// <summary> 각 객체에 맞게 Data Load </summary>
    protected abstract void LoadData();


}



