 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class ObjectBase : MonoBehaviour
{
    //Objectinfo : Serializable
    [SerializeField] protected ObjectInfo statusSetting;

    [Space(10)]

    [Header("Currnet Info")] // status의 정보에 맞게 초기화 할 것
    [SerializeField] protected int   m_nLevel;
    [SerializeField] protected int   m_nMaxExp;
    [SerializeField] protected int   m_nCurExp;
    [SerializeField] protected float m_fMaxHP;
    [SerializeField] protected float m_fCurHP;
    [SerializeField] protected float m_fCurSTR;
    [SerializeField] protected float m_fMaxMP;
    [SerializeField] protected float m_fCurMP;
    [SerializeField] protected float m_fAttackRange;
    [SerializeField] protected bool  m_bisAttack;
    [SerializeField] protected bool  m_bisDead;
    [SerializeField] protected ObjectState objState; // 상태에 따른 액션
    

    #region Hit Material Setting
    protected SkinnedMeshRenderer skinMesh;
    [Header("Hit Material")]
    [SerializeField] protected Material hitMat;
    [SerializeField] protected Material dieMat;
    protected Material originMat;
    #endregion

    #region Animation Setting
    protected Animator anim;
    protected readonly int hashDead = Animator.StringToHash("DoDead");

    #endregion


    protected virtual void Awake()
    {
        InitObj();
        skinMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        originMat = skinMesh.material;
    }

    protected virtual void Start()
    {

    }


    


    protected virtual void Getinfo()
    {
        Debug.Log($"HP : {m_fCurHP}");
        Debug.Log($"STR : {m_fCurSTR}");
        Debug.Log($"AttackRange : {m_fAttackRange}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, transform.position + transform.forward * m_fAttackRange);
        Gizmos.DrawWireSphere(transform.position, m_fAttackRange);
    }


    //protected abstract void OnAttack1Trigger();

    //Init Object Setting
    private void InitObj()
    {
        //Status Set
        m_nLevel       = statusSetting.GetLevel();
        m_nCurExp      = statusSetting.GetExp();
        m_fMaxHP       = statusSetting.GetMaxHP();
        m_fMaxMP       = statusSetting.GetMaxMP();
        m_fCurSTR      = statusSetting.GetSTR();
        m_fAttackRange = statusSetting.GetAttackRange();

        m_fCurHP       = m_fMaxHP;
        m_fCurMP       = m_fMaxMP;
        //State Set
        objState = ObjectState.IDLE;

        //Component Set
 
        anim = GetComponent<Animator>();
    }

    //State값 반환
    public ObjectState GetObjState()
    {
        return objState;
    }

    public bool GetIsDie()
    {
        return m_bisDead;
    }
    //데미지 입었을 때 함수
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

    protected abstract IEnumerator Attack();
    //체력이 0 이하 일 때 함수
    protected abstract void Die();


}



