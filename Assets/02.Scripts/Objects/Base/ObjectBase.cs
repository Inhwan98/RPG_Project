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
    [SerializeField] protected int   m_nMaxExp;
    [SerializeField] protected int   m_nCurExp;
    [SerializeField] protected int   m_nMaxHP;
    [SerializeField] protected int   m_nCurHP;
    [SerializeField] protected int   m_nCurSTR;
    [SerializeField] protected int   m_nSkillDamage;
    [SerializeField] protected int   m_nMaxMP;
    [SerializeField] protected int   m_nCurMP;
    [SerializeField] protected bool  m_bisAttack;
    [SerializeField] protected bool  m_bisDead;
    [SerializeField] protected ObjectState objState; // 상태에 따른 액션
    
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
        //_resourcesData = new ResourcesData();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Getinfo()
    {
        Debug.Log($"HP : {m_nCurHP}");
        Debug.Log($"STR : {m_nCurSTR}");
    }

    //protected abstract void OnAttack1Trigger();

    //Init Object Setting
    private void InitObj()
    {
        //Status Set
        m_nLevel       = objData.GetLevel();
        m_nCurExp      = objData.GetCurExp();
        m_nMaxHP       = objData.GetMaxHP();
        m_nMaxMP       = objData.GetMaxMP();
        m_nCurSTR      = objData.GetCurSTR();

        m_nCurHP       = m_nMaxHP;
        m_nCurMP       = m_nMaxMP;
        //State Set
        objState = ObjectState.IDLE;

        //Component Set
 
        _anim = GetComponent<Animator>();
    }

    /// <summary> 스킬데미지 값 반환 </summary>
    public int GetSkillDamage() => m_nSkillDamage;

    /// <summary> 스킬데미지 값 수정 </summary>
    public void  SetSkillDamage(int _skillDamage) => m_nSkillDamage = _skillDamage;


    /// <summary> State값 반환 </summary>
    public ObjectState GetObjState() => objState;
    /// <summary> Object Die 상태 반환 </summary>
    public bool GetIsDie() => m_bisDead;

    /// <summary> 대미지를 입었을 때 함수 </summary>
    public virtual void OnDamage(int _str)
    {
        this.m_nCurHP -= _str;
        //Debug.Log($"{this.name} 가 {_str} 대미지를 입었다. 현재 체력 {m_nCurHP}");
        if (m_nCurHP <= 0)// 체력이 0 이하
        {
            this.objState = ObjectState.DEAD;
        }
    }

    
    public virtual void Buff(int _str) { }

    /// <summary> 공격 관련 </summary>
    protected abstract IEnumerator Attack();

    /// <summary> 체력이 0 이하일 때 Die Event </summary>
    protected abstract void Die();

    protected abstract void LoadData();


}



