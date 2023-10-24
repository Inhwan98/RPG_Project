using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class BossMonster : Monster
{
    /*********************************************
     *                   Fields
     ********************************************/
    #region option Fields
    [Header("SKILL Sensor")]
    [SerializeField] protected float m_fRangeAttackRadius = 1.0f;
    [SerializeField] protected float m_fRangeAttackRange = 0.5f;
    #endregion

    #region protected Fields
    protected BossPhase _eBossPhase = BossPhase.ONE;

    protected List<BossSkillData> _bossMeleeAttackList = new List<BossSkillData>();
    protected List<BossSkillData> _bossRangeAttackList = new List<BossSkillData>();
    protected int _meleeAttackIndex;
    protected int _rangeAttackIndex;
    //범위 공격 Ray
    protected RaycastHit[] _rangeAttackHits;
    #endregion

    private BossMeleeAttack _bossMeleeAttack;
    //페이즈 변경할 체력 기점
    private int nPhaseChangeHP;


    /********************************************
    *                 Unity Event
    ********************************************/

    #region Unity Event
    protected override void Awake()
    {
        base.Awake();

        _bossMeleeAttack = GetComponent<BossMeleeAttack>();

    }
    protected override void Start()
    {
        base.Start();
        BossSkillData[] bossMeleeAtkDB = GameManager.instance.GetAllData().BossMeleeAttackDB;
        BossSkillData[] bossRangeAtkDB = GameManager.instance.GetAllData().BossRangeAttackDB;

        foreach(BossSkillData meleeATKData in bossMeleeAtkDB)
        {

            if(meleeATKData.GetID() == m_nID)
            {
                meleeATKData.Init(m_nCurSTR);
                _bossMeleeAttackList.Add(meleeATKData);
            }
        }

        foreach(BossSkillData rangeATKData in bossRangeAtkDB)
        {
            if(rangeATKData.GetID() == m_nID)
            {
                rangeATKData.Init(m_nCurSTR);
                _bossRangeAttackList.Add(rangeATKData);
            }
        }

        //페이즈 피 구간 정하기
        nPhaseChangeHP = (int)(m_nMaxHP * 0.5f);
    }

    /// <summary>  탐지, 센서 범위 가시화 </summary>
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        //공격 센서 범위
        Gizmos.color = Color.yellow;
        Debug.DrawLine(transform.position, transform.position + transform.forward * m_fRangeAttackRange);
        Gizmos.DrawWireSphere(transform.position + transform.forward * m_fRangeAttackRange, m_fRangeAttackRadius);
    }

    #endregion


    /********************************************
    *                 Methods
    ********************************************/

    #region override Methods
    protected override void Targetting()
    {
        _rangeAttackHits = Physics.SphereCastAll(transform.position,
                                    m_fRangeAttackRadius,
                                    transform.forward,
                                    m_fRangeAttackRange,
                                    targetLayer);
        base.Targetting();
    }
    protected override void DetectedAttackRay()
    {

        if (_attackHits.Length > 0 && !m_bisAttack)
        {
            objState = ObjectState.MELEEATK;
        }
        else if (_rangeAttackHits.Length > 0 && !m_bisAttack && _eBossPhase != BossPhase.ONE && !_isAttackWaiting)
        {
            objState = ObjectState.SKILLATK;
        }
    }

    /// <summary> 대부분 Monster와 동일하나, 일정 체력 밑으로 내려가면 페이즈 변경 추가 </summary>
    public override void OnDamage(int str, bool _isKnockback = false, Transform posTr = null)
    {
        base.OnDamage(str);

        if (m_nCurHP <= nPhaseChangeHP)
        {
            _eBossPhase = (BossPhase)((int)_eBossPhase + 1);
            nPhaseChangeHP = 0;
        }
    }

    protected override IEnumerator Attack()
    {
        BossSkillData curMeleeAttack = _bossMeleeAttackList[_meleeAttackIndex];
        Debug.Assert(curMeleeAttack != null, "curMeleeAttack is NULL");

        m_bisAttack = true;                                 //공격중인 상태 - true
        StartCoolTime(curMeleeAttack);                      //공격 딜레이 설정 (코루틴함수)
        _anim.SetTrigger(curMeleeAttack.GetAnimHash());     //공격 애니메이션 실행
        _bossMeleeAttack.Use(_meleeAttackIndex);            //어택 콜라이더 활성/비활성

        int attackDamage = curMeleeAttack.GetSkillDamage(); //대미지 설정 - 1
        SetSkillDamage(attackDamage);                       //대미지 설정 - 2

        yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        _meleeAttackIndex++;                                //다음 공격패턴 이동

        if (_bossMeleeAttackList.Count == _meleeAttackIndex) //최대치에 도달했을 시 다시 처음패턴부터
            _meleeAttackIndex = 0;
        m_bisAttack = false;                                 //공격중인 상태 - false
    }
    protected override IEnumerator SkillAttack()
    {
        if (_eBossPhase == BossPhase.TWO)
        {
            BossSkillData curSkillAttack = _bossRangeAttackList[_rangeAttackIndex];
            StartCoolTime(curSkillAttack);                      //공격 딜레이 설정 (코루틴함수)

            int skillDamage = curSkillAttack.GetSkillDamage();  //대미지 설정 - 1
            SetSkillDamage(skillDamage);                        //대미지 설정 - 2

            _anim.SetTrigger(curSkillAttack.GetAnimHash());

        }
        m_bisAttack = true;
        yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

        _rangeAttackIndex++;                                   //다음 공격패턴 이동

        //최대치에 도달했을 시 다시 처음패턴부터
        if (_bossRangeAttackList.Count == _rangeAttackIndex) _rangeAttackIndex = 0;

        m_bisAttack = false;
    }
    #endregion

    #region private Methods
    /// <summary> 스킬데이터의 쿨타임 시간의 비례해서 스킬 사용 여부를 정함 </summary>
    private void StartCoolTime(BossSkillData bossSkillData)
    {
        _attackDelay = bossSkillData.GetCoolDown();
        StartCoroutine(StartAttackCoolTime(_attackDelay));
    }
    #endregion

    #region public Methods
    public void StartTimeLine()
    {
        _nav.enabled = false;
        this.enabled = false;
    }
    public void EndTimeLine()
    {
        _nav.enabled = true;
        this.enabled = true;
    }
    #endregion
}

