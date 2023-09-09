 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviour
{
    //Objectinfo : Serializable
    [SerializeField] protected ObjectData objData;

    [Space(10)]

    [Header("Currnet Info")] // status의 정보에 맞게 초기화 할 것
    [SerializeField] protected int   m_nLevel;
    [SerializeField] protected int   m_nMaxExp;
    [SerializeField] protected int   m_nCurExp;
    [SerializeField] protected float m_fMaxHP;
    [SerializeField] protected float m_fCurHP;
    [SerializeField] protected float m_fCurSTR;
    [SerializeField] protected float m_fSkillDamage;
    [SerializeField] protected float m_fMaxMP;
    [SerializeField] protected float m_fCurMP;
    [SerializeField] protected bool  m_bisAttack;
    [SerializeField] protected bool  m_bisDead;
    [SerializeField] protected ObjectState objState; // 상태에 따른 액션
    
    #region Animation Setting
    protected Animator anim;
    protected readonly int hashDead = Animator.StringToHash("DoDead");
    #endregion

    public int   GetLevel() => m_nLevel;

    public float GetMaxHP() => m_fMaxHP;
    public float GetCurHP() => m_fCurHP;

    public float GetMaxMP() => m_fMaxMP;
    public float GetCurMP() => m_fCurMP;

    public float GetCurStr() => m_fCurSTR;
    public int GetCurExp() => m_nCurExp;


    /// <summary> Resource Data 클래스 </summary>
    protected ResourcesData _resourcesData;

    protected virtual void Awake()
    {
        InitObj();
        _resourcesData = new ResourcesData();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Getinfo()
    {
        Debug.Log($"HP : {m_fCurHP}");
        Debug.Log($"STR : {m_fCurSTR}");
    }

    //protected abstract void OnAttack1Trigger();

    //Init Object Setting
    private void InitObj()
    {
        //Status Set
        m_nLevel       = objData.GetLevel();
        m_nCurExp      = objData.GetCurExp();
        m_fMaxHP       = objData.GetMaxHP();
        m_fMaxMP       = objData.GetMaxMP();
        m_fCurSTR      = objData.GetCurSTR();

        m_fCurHP       = m_fMaxHP;
        m_fCurMP       = m_fMaxMP;
        //State Set
        objState = ObjectState.IDLE;

        //Component Set
 
        anim = GetComponent<Animator>();
    }

    /// <summary> 스킬데미지 값 반환 </summary>
    public float GetSkillDamage() => m_fSkillDamage;

    /// <summary> 스킬데미지 값 수정 </summary>
    public void  SetSkillDamage(float _skillDamage) => m_fSkillDamage = _skillDamage;


    /// <summary> State값 반환 </summary>
    public ObjectState GetObjState() => objState;
    /// <summary> Object Die 상태 반환 </summary>
    public bool GetIsDie() => m_bisDead;

    /// <summary> 대미지를 입었을 때 함수 </summary>
    public virtual void OnDamage(float _str)
    {
        this.m_fCurHP -= _str;
        //Debug.Log($"{this.name} 가 {_str} 대미지를 입었다. 현재 체력 {m_nCurHP}");
        if (m_fCurHP <= 0)// 체력이 0 이하
        {
            this.objState = ObjectState.DEAD;
        }
    }

    
    public virtual void Buff(float _str) { }

    /// <summary> 공격 관련 </summary>
    protected abstract IEnumerator Attack();

    /// <summary> 체력이 0 이하일 때 Die Event </summary>
    protected abstract void Die();


}



