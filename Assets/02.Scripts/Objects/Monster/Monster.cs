using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : ObjectBase
{

    [SerializeField] private int exp;
    private int monidx; //몬스터 고유 번호
    PlayerController playerCtr;
    Transform playerTr;

    #region Animation Setting
    protected readonly int hashTrace = Animator.StringToHash("IsWalk");
    protected readonly int hashAttack = Animator.StringToHash("IsAttack");
   
    #endregion

    #region NavMesh Setting
    protected NavMeshAgent agent;
    protected Transform destTr;
    #endregion

    protected virtual void OnEnable()
    {
        playerCtr = PlayerController.instance;
        playerTr  = playerCtr.transform;

        PlayerController.OnPlayerDie += this.OnPlayerDie;
        
        GameManager.instance.AddCurrentMonsters(out monidx); //base.OnEnable에서 gameMgr이 할당 됌
    }

    protected override void Start()
    {
        InitObj();
        base.Start();
        StartCoroutine(CheckObjState());
        StartCoroutine(ObjectAction());
    }

    protected override IEnumerator Attack()
    {
        agent.isStopped = true;
        anim.SetBool(hashTrace, false);

        m_bisAttack = true;
        transform.LookAt(destTr);
        anim.SetBool(hashAttack, true);

        Debug.Assert(destTr, "destTr is NULL !!!");

        ObjectBase _objCtr = destTr.GetComponent<ObjectBase>();

        _objCtr.OnDamage(m_fCurSTR);
        yield return new WaitForSeconds(1);
        m_bisAttack = false;
    }

    #region Coroutine State
    IEnumerator CheckObjState()
    {
        while (!m_bisDead)
        {
            yield return null;

            if (destTr == null)
            {
                objState = ObjectState.IDLE;
                continue;
            }

            if (objState == ObjectState.DEAD) yield break;

            float distance = Vector3.Distance(this.transform.position, destTr.position);

            if (distance <= m_fAttackRange)
            {
                objState = ObjectState.ATK;
            }
            else
            {
                objState = ObjectState.MOVE;
            }
        }
    }
    #endregion

    //Init Object Setting
    private void InitObj()
    {
        //Status Set
        m_nLevel = statusSetting.GetLevel();
        m_nCurExp = statusSetting.GetExp();
        m_fMaxHP = statusSetting.GetMaxHP();
        m_fMaxMP = statusSetting.GetMaxMP();
        m_fCurSTR = statusSetting.GetSTR();
        m_fAttackRange = statusSetting.GetAttackRange();

        m_fCurHP = m_fMaxHP;
        m_fCurMP = m_fMaxMP;
        //State Set
        objState = ObjectState.IDLE;

        //Component Set
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = m_fAttackRange;

    }
        //목적지 설정
        protected void SetDestination(Transform _destTr)
    {
        destTr = _destTr;
        agent.isStopped = false;
        agent.SetDestination(_destTr.position);
    }


    IEnumerator ObjectAction()
    {
        while (!m_bisDead)
        {
            switch (objState)
            {
                case ObjectState.MOVE:
                    agent.SetDestination(destTr.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    break;

                case ObjectState.ATK:

                    if (!m_bisAttack)
                    {
                        StartCoroutine(Attack());
                    }

                    break;

                case ObjectState.DEAD:

                    Die();
                    break;

                case ObjectState.IDLE:
                    anim.SetBool(hashTrace, false);

                    anim.SetBool(hashAttack, false);
                    break;
            }
            yield return null;
        }
    }

    public override void OnDamage(float _str)
    {
        this.m_fCurHP -= _str;
        //Debug.Log($"{this.name} 가 {_str} 대미지를 입었다. 현재 체력 {m_nCurHP}");
        if (m_fCurHP <= 0)// 체력이 0 이하
        {
            playerCtr.SubMonsterObjs(monidx, this.transform);
            playerCtr.SetEXP(m_nCurExp);
                
            this.objState = ObjectState.DEAD;
            skinMesh.material = dieMat;
            return;
        }
        InvokeRepeating("HitMat", 0.3f, 0.1f);
        Invoke("CancleInvokeLog", 0.6f);
    }
    private void CancleInvokeLog()
    {
        CancelInvoke("HitMat");
    }

    public void HitMat()
    {
        skinMesh.material = hitMat;
        Invoke("ChangeMat", 0.05f);
    }

    public void ChangeMat()
    {
        skinMesh.material = originMat;
    }


    protected override void Die()
    {
        PlayerController.OnPlayerDie -= this.OnPlayerDie;

        this.gameObject.layer = 2; // Ignore Raycast
        StopAllCoroutines();
        anim.SetTrigger(hashDead);
        agent.enabled = false;
        m_bisDead = true;
        Destroy(this.gameObject, 2.0f);
    }

    private void OnPlayerDie()
    {
        StopAllCoroutines();

        //추적 중지
        agent.enabled = false;
        destTr = null;

        anim.SetBool(hashTrace, false);
        anim.SetBool(hashAttack, false);
    }

    private void OnDestroy()
    {
        GameManager.instance.SubCurrentMonsters();    
    }
}

