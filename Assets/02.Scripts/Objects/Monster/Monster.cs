﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : ObjectBase
{

    [SerializeField] private int exp;

    protected PlayerController _playerCtr;
    private Transform _playerTr;
    protected MonsterData _monsterData;
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
    [SerializeField] private Transform _damageTr;
    private GameObject _damageTextObj;

    //Drop Item
    protected List<ItemData> _itemDatas = new List<ItemData>();
    protected Dictionary<ItemData, int> _itemDic = new Dictionary<ItemData, int>();

    protected int m_nPortionDrop_MinAmount;
    protected int m_nPortionDrop_MaxAmount;
    protected int m_nItemDrop_percentage;


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

        _damageTextObj = Resources.Load<GameObject>("Prefab/DamageText");
    }

    protected override void Start()
    {
        _playerCtr = PlayerController.instance;
        _playerTr = _playerCtr.transform;
        destTr = _playerTr;

        // 손볼필요있음
        GameManager.instance.AddCurrentMonsters(m_nID);
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
            if(attackHits.Length == 0 && !m_bisAttack)
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

    #region NavMesh On/Off Func
    void ChaseStart()
    {
        m_bisChase = true;
        _anim.SetBool(hashTrace, true);
        agent.enabled = true;
    }

    void ChaseEnd()
    {
        m_bisChase = false;
        _anim.SetBool(hashTrace, false);
        agent.enabled = false;
    }
    #endregion

    protected override IEnumerator Attack()
    {
        ChaseEnd();

        m_bisAttack = true;
        transform.LookAt(destTr);
        _anim.SetBool(hashAttack, true);

        Debug.Assert(destTr, "destTr is NULL !!!");

        ObjectBase _objCtr = destTr.GetComponent<ObjectBase>();

        _objCtr.OnDamage(m_nCurSTR);
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

    /// <summary> Monster FSM </summary>
    IEnumerator ObjectAction()
    {
        while (!m_bisDead)
        {
            switch (objState)
            {
                case ObjectState.MOVE:
                    ChaseStart();
                    _anim.SetBool(hashAttack, false);
                    break;

                case ObjectState.ATK:

                    if(!m_bisAttack)
                    {
                        StartCoroutine(Attack());
                    }

                    break;

                case ObjectState.DEAD:
                    Die();
                    break;

                case ObjectState.IDLE:
                    ChaseEnd();
                    _anim.SetBool(hashAttack, false);
                    break;
            }
            yield return null;
        }
    }

    /// <summary> 데미지를 받았을 시 </summary>
    public override void OnDamage(int _str)
    {
        //죽어있다면 데미지 이벤트 발생 x
        if (m_bisDead) return;

        //방어력을 가졌다면 증감해서 대미지 입는다
        this.m_nCurHP -= _str;

        //데미지 텍스트  발생. 오브젝트 풀링할 것
        GameObject damageText;
        damageText = Instantiate(_damageTextObj, _damageTr.position, Quaternion.identity, transform);
        damageText.GetComponent<TextMesh>().text = $"{_str}";
    
        //Debug.Log($"{this.name} 가 {_str} 대미지를 입었다. 현재 체력 {m_nCurHP}");
        if (m_nCurHP <= 0)// 체력이 0 이하
        {
            
            this.objState = ObjectState.DEAD;
            return;
        }
    }

    /// <summary> 몬스터의 드롭아이템 추가 </summary>
    public void AddDropItem(ItemData _itemData, int _amount = 1, int _percentage = 100)
    {
        bool chance = Dods_ChanceMaker.GetThisChanceResult_Percentage(_percentage);
        if (chance == false) return;

        if(_itemData == null)
        {
            Debug.LogError("Itemdata is Null");
        }

        _itemDic.Add(_itemData, _amount);
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
            int playerStr = _playerCtr.GetCurStr();
            OnDamage(playerStr);
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("PlayerSkill"))
        {
            int playerSkillPower = _playerCtr.GetSkillDamage();
            OnDamage(playerSkillPower);
        }
    }

    protected override void Die()
    {
        PlayerController.OnPlayerDie -= this.OnPlayerDie;
        GameManager.instance.MonsterDead(m_nID);
        StopAllCoroutines(); //FSM 중단

        //GameObject _chestObj = Instantiate<GameObject>(chestPrefab, transform.position + (Vector3.up * 0.22f), Quaternion.identity);
        _playerCtr.SetEXP(m_nCurExp); //플레이어에게 경험치 전달
        this.gameObject.layer = 2;    // Ignore Raycast        
        _anim.SetTrigger(hashDead);   // 쓰러지는 애니메이션
        agent.enabled = false;        // Nav Mesh 중단
        m_bisDead = true;             // 죽음 상태 true
        Destroy(this.gameObject, 2.0f); // 쓰러진뒤 2초뒤에 오브젝트 삭제... 오브젝트 풀링시 수정
    }

    private void OnPlayerDie()
    {
        StopAllCoroutines();

        //추적 중지
        agent.enabled = false;
        destTr = null;

        _anim.SetBool(hashTrace, false);
        _anim.SetBool(hashAttack, false);
    }

    protected override void LoadData()
    {
        //몬스터는 100번대 부터 시작한다. 0번째 인덱스 부터 시작하려면 m_ID - 100 
        //현재 몬스터에 해당하는 INDEX를 찾아간다.
        _monsterData = SaveSys.LoadAllData().MonsterDB[m_nID - 100];

        if (_monsterData == null)
        {
            Debug.LogError("MonData is NULL!!");
            return;
        }

        m_nLevel = _monsterData.nLevel;
        m_nMaxHP = _monsterData.nMaxHP;
        m_nCurHP = _monsterData.nMaxHP;

        m_nMaxMP = _monsterData.nMaxMP;
        m_nCurMP = _monsterData.nMaxMP;


        m_nCurSTR = _monsterData.nCurSTR;
        m_nCurExp = _monsterData.nDropExp;
    }

    private void OnDestroy()
    {
        
    }

    [ContextMenu("Item List")]
    public void VisibleItemList()
    {
        foreach(var a in _itemDic)
        {
            Debug.Log($"{a.Key}");
            Debug.Log($"{a.Value}");

        }
        
    }

    
}

