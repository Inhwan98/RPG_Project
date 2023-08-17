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

    private bool m_bisChase = false;

    [Header("Target")]
    [SerializeField] private LayerMask targetLayer;

    [Header("Detect Sensor")]
    [SerializeField] private float m_fSensorRadius = 0.6f;
    [SerializeField] private float m_fSensorRange = 0.2f;
    [Header("Attack Sensor")]
    [SerializeField] private float m_fTargetRadius = 0.6f;
    [SerializeField] private float m_fTargetRange = 0.2f;

    [Header("Damage UI")]
    [SerializeField] private Transform damageTr;
    private GameObject damageTextObj;


    #region Animation Setting
    protected readonly int hashTrace = Animator.StringToHash("IsWalk");
    protected readonly int hashAttack = Animator.StringToHash("IsAttack");
    #endregion

    #region NavMesh Setting
    protected NavMeshAgent agent;
    protected Transform destTr;
    #endregion



    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();

        damageTextObj = Resources.Load<GameObject>("Prefab/DamageUI");
    }

    protected override void Start()
    {
        playerCtr = PlayerController.instance;
        playerTr = playerCtr.transform;
        destTr = playerTr;

        GameManager.instance.AddCurrentMonsters(out monidx);
        base.Start();
        //StartCoroutine(CheckObjState());
        StartCoroutine(ObjectAction());
    }

    protected virtual void OnEnable()
    {
        PlayerController.OnPlayerDie += this.OnPlayerDie;
    }

    private void FixedUpdate()
    {
        if (!m_bisDead)
        {
            StartCoroutine(Targetting());
        }
        else
            StopCoroutine(Targetting());
    }

    IEnumerator Targetting()
    {
        RaycastHit[] rayHits =
           Physics.SphereCastAll(transform.position,
                                 m_fSensorRadius,
                                 transform.forward,
                                 m_fSensorRange,
                                 targetLayer);

        RaycastHit[] attackHits =
            Physics.SphereCastAll(transform.position,
                                  m_fTargetRadius,
                                  transform.forward,
                                  m_fTargetRange,
                                  targetLayer);
        //Physics.SphereCastAll:
        //위치, (구체)반지름, 방향, 구체범위, 구체의 들어온 레이어판별

        if(rayHits.Length == 0) objState = ObjectState.IDLE;

        if (rayHits.Length > 0 && !m_bisAttack)
        {
            if(attackHits.Length == 0)
            {
                objState = ObjectState.MOVE;
            }
            else if (attackHits.Length > 0 && !m_bisAttack)
            {
                objState = ObjectState.ATK;

            }
        }

        yield return null;
    }

    private void Update()
    {
        if (agent.enabled)
        {
            agent.SetDestination(destTr.position);
            agent.isStopped = !m_bisChase; //isChase가 false면 iStopped은 true...
        }
    }

    void ChaseStart()
    {
        m_bisChase = true;
        anim.SetBool(hashTrace, true);
        agent.enabled = true;
    }

    void ChaseEnd()
    {
        m_bisChase = false;
        anim.SetBool(hashTrace, false);
        agent.enabled = false;
    }

    protected override IEnumerator Attack()
    {
        ChaseEnd();

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
    //IEnumerator CheckObjState()
    //{
    //    while (!m_bisDead)
    //    {
    //        yield return null;

    //        if (objState == ObjectState.DEAD) yield break;
    //    }
    //}
    #endregion

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
                    ChaseStart();
                    anim.SetBool(hashAttack, false);
                    break;

                case ObjectState.ATK:

                    if(!m_bisAttack)
                    {
                        StartCoroutine(Attack());
                    }
                    

                    break;

                case ObjectState.DEAD:
                    Debug.Log("Monster Die");
                    Die();
                    break;

                case ObjectState.IDLE:
                    ChaseEnd();
                    anim.SetBool(hashAttack, false);
                    break;
            }
            yield return null;
        }
    }

    public override void OnDamage(float _str)
    {
        if (m_bisDead) return;

        this.m_fCurHP -= _str;

        UI_Damage damageUICtr;
        GameObject _damageText;
        
        _damageText = Instantiate(damageTextObj, damageTr.position, Quaternion.identity, transform);
        damageUICtr = _damageText.GetComponent<UI_Damage>();
        damageUICtr.SetDamageText(_str);
        //Debug.Log($"{this.name} 가 {_str} 대미지를 입었다. 현재 체력 {m_nCurHP}");
        if (m_fCurHP <= 0)// 체력이 0 이하
        {
            playerCtr.SetEXP(m_nCurExp);
                
            this.objState = ObjectState.DEAD;
            return;
        }
    }

    private void OnDrawGizmos()
    {
        //추격탐지
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, transform.position + transform.forward * m_fSensorRange);
        Gizmos.DrawWireSphere(transform.position + transform.forward * m_fSensorRange, m_fSensorRadius);
        //공격 센서 범위
        Gizmos.color = Color.blue;
        Debug.DrawLine(transform.position, transform.position + transform.forward * m_fTargetRange);
        Gizmos.DrawWireSphere(transform.position + transform.forward * m_fTargetRange, m_fTargetRadius);
    }


    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            Debug.Log("PlayerWeapon");

            float playerStr = playerCtr.GetSTR();
            OnDamage(playerStr);
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("PlayerSkill"))
        {
            Debug.Log("PlayerSkill");

            float playerSkillPower = playerCtr.GetSkillDamage();
            OnDamage(playerSkillPower);
        }
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

