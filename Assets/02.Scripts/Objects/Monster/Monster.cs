using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : ObjectBase
{
    protected PlayerController _playerCtr;
    private Transform _playerTr;
    protected MonsterData _monsterData;
    private bool m_bisChase = false;

    [Header("Target")]
    [SerializeField] protected LayerMask targetLayer;

    [Header("Detect Sensor")]
    [SerializeField] private float m_fSensorRadius = 35.0f;
    [SerializeField] private float m_fSensorRange = 0.0f;
    [Header("Attack Sensor")]
    [SerializeField] private float m_fTargetRadius = 1.5f;
    [SerializeField] private float m_fTargetRange = 0.5f;


    [SerializeField] protected ObjectState objState = ObjectState.NONE; // 상태에 따른 액션

    // 공격 지연 관련
    [Header("Attack Delay")]
    [SerializeField]  protected float _attackDelay = 5.0f;
    protected bool _isAttackWaiting;

    private float _fIdleTimer;
    private Vector3 _destPos;

    private Material _mat; //활성화 / 비활성화 시 COLOR에서 ALPHA값을 변경할 것

    //타겟팅 레이케스트 저장
    protected RaycastHit[] _rayHits;
    protected RaycastHit[] _attackHits;


    //Drop Item
    protected List<ItemData> _itemDatas = new List<ItemData>();
    protected Dictionary<ItemData, int> _itemDic = new Dictionary<ItemData, int>();

    protected string m_sName; //Data Load
    protected int m_nDromExp; //Data Load
    protected int m_nDropMoney; //Data Load
    protected int[] m_nDropItemArray; //Data Load

    protected int m_nPortionDrop_MinAmount;
    protected int m_nPortionDrop_MaxAmount;
    protected int m_nItemDrop_percentage;


    #region Animation Setting
    protected readonly int hashTrace = Animator.StringToHash("IsWalk");
    protected readonly int hashAttack = Animator.StringToHash("IsAttack");
    #endregion

    #region NavMesh Setting
    protected NavMeshAgent _nav;
    protected Transform _destTr;
    #endregion

    public Transform GetDestTr() => _destTr;

    protected override void Awake()
    {
        base.Awake();
        _nav = GetComponent<NavMeshAgent>();
        _mat = GetComponentInChildren<SkinnedMeshRenderer>().material;

        //어색한 자동회전은 꺼준다.
        _nav.updateRotation = false;

    }


    protected virtual void OnEnable()
    {
        PlayerController.OnPlayerDie += this.OnPlayerDie;

        objState = ObjectState.IDLE;
        m_bisDead = false;

        StartCoroutine(ActiveAlphaValue(true)); //투명 => 불투명 해지며 활성화
        StartCoroutine(ObjectAction());
    }


    protected override void Start()
    {
        base.Start();
        _playerCtr = PlayerController.instance;
        _playerTr = _playerCtr.transform;
        //_destTr = _playerTr;

        ChangeDamageTextColor(new Color32(255, 142, 198, 255));

        SettingDropItem();
    }

    private void FixedUpdate()
    {
        if (!m_bisDead)
        {
            Targetting();
        }
    }

    private void Update()
    {
        if (!m_bisDead)
        {
            SmoothRotation();
        }
    }

    #region NavMesh On/Off Func
    protected void ChaseStart()
    {
        if (!_nav.enabled) return;

        _nav.SetDestination(_destPos);
        m_bisChase = true;
        _anim.SetBool(hashTrace, true);
        _nav.isStopped = false;
    }

    protected void ChaseEnd()
    {
        if (!_nav.enabled) return;

        m_bisChase = false;
        _anim.SetBool(hashTrace, false);
        _nav.isStopped = true;
    }
    #endregion

    /// <summary> ray를 통해 상태 변환 </summary>
    protected virtual void Targetting()
    {
        // RaycastHit[] ray = new RaycastHit[10];
        //Physics.SphereCastNonAlloc(transform.position, m_fSensorRadius, transform.forward, ray, m_fSensorRange, targetLayer);

        _rayHits =  Physics.SphereCastAll(transform.position,
                                         m_fSensorRadius,
                                         transform.forward,
                                         m_fSensorRange,
                                         targetLayer);
                                         
        _attackHits =  Physics.SphereCastAll(transform.position,
                                            m_fTargetRadius,
                                            transform.forward,
                                            m_fTargetRange,
                                            targetLayer);
        //Physics.SphereCastAll:
        //위치, (구체)반지름, 방향, 구체범위, 구체의 들어온 레이어판별

        //센서에 잡히지 않았으며, 패트롤, 아이들 상태가 아닐때... 추격 혹은 공격중일 때 IDLE로 변환
        // IDLE과 PATROL은 순환해야함... IDLE 유지는 x
        if (_rayHits.Length == 0 && objState != ObjectState.PATROL && objState != ObjectState.IDLE)
            objState = ObjectState.IDLE;

        //탐지 센서에 감지 되었다면...
        if (_rayHits.Length > 0 && !m_bisAttack)
        {
            //공격센서 감지되지 않았다면
            if (_attackHits.Length == 0 && !m_bisAttack)
            {
                _destTr = _rayHits[0].transform;
                _destPos = _destTr.position;

                objState = ObjectState.MOVE;
            }
            //공격센서에 감지되었다면
            DetectedAttackRay();
        }
    }

    /// <summary> 공격센서 안에 들어갈 경우 FSM 공격패턴 </summary>
    protected virtual void DetectedAttackRay()
    {
        if (_attackHits.Length > 0 && !m_bisAttack)
        {
            objState = ObjectState.MELEEATK;
        }
    }

    /// <summary> Monster의 공격 코루틴 함수 </summary>
    protected override IEnumerator Attack()
    {
        StartCoroutine(StartAttackCoolTime(_attackDelay));
        ChaseEnd();

        m_bisAttack = true;
        //transform.LookAt(_destTr);
        _anim.SetBool(hashAttack, true);

        Debug.Assert(_destTr, "destTr is NULL !!!");

        ObjectBase _objCtr = _destTr.GetComponent<ObjectBase>();
        Debug.Assert(_objCtr, "_objCtr is NULL !!!");

        int nDamage = GetAttackPower(m_nCurSTR);
        _objCtr.OnDamage(nDamage);
        yield return new WaitForSeconds(1);
        m_bisAttack = false;
    }

    protected virtual IEnumerator SkillAttack()
    {

        yield return null;
    }

    /// <summary> 순찰에 필요한 NavMesh의 랜덤 포지션값 설정 </summary>
    private void SetRandomTargetPosition()
    {
        // 무작위로 새로운 목표 위치 설정 (예시로 -10에서 10 사이의 범위로 설정)
        float randomX = Random.Range(transform.position.x - 10f, transform.position.x + 10f);
        float randomZ = Random.Range(transform.position.z - 10f, transform.position.z + 10f);
        _destPos = new Vector3(randomX, transform.position.y, randomZ);

        _fIdleTimer = Random.Range(0.3f, 3.0f);
    }

    /// <summary Nav의 목적지 설정 </summary>
    protected void SetDestination(Transform _destTr)
    {
        this._destTr = _destTr;
        _nav.isStopped = false;
        _nav.SetDestination(_destTr.position);
    }

    /// <summary> AI의 회전 처리 </summary>
    private void SmoothRotation()
    {
        if(_nav.remainingDistance >= 2.0f)
        {
            //에이전트의 이동 방향
            Vector3 direction = _nav.desiredVelocity;
            //회전 각도(쿼터니언) 산출
            if(direction != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(direction);
                //구면 선형보간 함수로 부드러운 회전 처리
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      rot,
                                                      Time.deltaTime * 10.0f);
            }
        }
    }

    /// <summary> Monster FSM </summary>
    IEnumerator ObjectAction()
    {
        while (!m_bisDead)
        {
            switch (objState)
            {
                case ObjectState.NONE:
                    break;

                case ObjectState.IDLE:
                    ChaseEnd();
                    _anim.SetBool(hashAttack, false);
                    yield return new WaitForSeconds(_fIdleTimer);

                    SetRandomTargetPosition();
                    objState = ObjectState.PATROL;

                    break;

                case ObjectState.PATROL:
                    _anim.SetBool(hashAttack, false);

                    ChaseStart();

                    if(_nav.velocity == Vector3.zero)
                    {
                        SetRandomTargetPosition();
                    }

                    if (Vector3.Distance(_destPos, transform.position) < 0.1f)
                    {
                        objState = ObjectState.IDLE;
                    }
                    break;

                case ObjectState.MOVE:
                    _anim.SetBool(hashAttack, false);
                    
                    ChaseStart();
                    
                    break;

                case ObjectState.MELEEATK:

                    ChaseEnd();
                    if (!_isAttackWaiting)
                    {
                        StartCoroutine(Attack());
                    }

                    break;

                case ObjectState.SKILLATK:
                    ChaseEnd();
                    if (!_isAttackWaiting)
                    {
                        StartCoroutine(SkillAttack());
                    }

                    break;

                case ObjectState.DEAD:
                    Die();
                    break;

              
            }
            yield return null;
        }
    }

    /// <summary> 몬스터의 드롭아이템 추가 </summary>
    public void AddDropItem(ItemData _itemData, int _amount = 1, int _percentage = 100)
    {
        bool chance = ChanceMaker.GetThisChanceResult_Percentage(_percentage);
        if (chance == false) return;

        if(_itemData == null)
        {
            Debug.LogError("Itemdata is Null");
        }

        _itemDic.Add(_itemData, _amount);
    }


    /// <summary>  탐지, 센서 범위 가시화 </summary>
    protected virtual void OnDrawGizmos()
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

    /// <summary> 플레이어의 무기, 스킬과 충돌이 있을때 </summary>
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            int playerPower = _playerCtr.GetCurStr();
            int nDamage = _playerCtr.GetAttackPower(playerPower);

            OnDamage(nDamage);
        }
        else if (coll.gameObject.layer == LayerMask.NameToLayer("PlayerSkill"))
        {
            int playerSkillPower = _playerCtr.GetSkillDamage();
            int nDamage = _playerCtr.GetAttackPower(playerSkillPower);
            OnDamage(nDamage);
        }
    }

    /// <summary> 데미지를 받았을 시 </summary>
    public override void OnDamage(int _power, bool _isKnockback = false, Transform posTr = null)
    {
        //죽어있다면 데미지 이벤트 발생 x
        if (m_bisDead) return;

        //방어력을 가졌다면 증감해서 대미지 입는다

        int nDamage = _power - m_nDefence;
        if (nDamage < 0) nDamage = 0;

        this.m_nCurHP -= nDamage;

        ActiveDamageText(nDamage);

        if (m_nCurHP <= 0)// 체력이 0 이하
        {
            this.objState = ObjectState.DEAD;
        }
    }

    /// <summary> 공격 후 매개변수 시간에 따라 공격이 가능한지에 대한 bool값 설정 </summary>
    protected IEnumerator StartAttackCoolTime(float waitTime)
    {
        _isAttackWaiting = true;
        yield return new WaitForSeconds(waitTime);
        _isAttackWaiting = false;
    }


    /// <summary>
    /// 몬스터가 사망했을 시 이벤트
    /// <br>플레이어에게 아이템, 경험치 전달</br>
    /// <br>게임매니져에게 몬스터 ID 전달 (업적, 퀘스트에 활용)</br>
    /// </summary>
    protected override void Die()
    {
        PlayerController.OnPlayerDie -= this.OnPlayerDie;
        GameManager.instance.MonsterDead(m_nID);

        if(BaseSceneManager.instance is DungeonSceneManager dgScene)
        {
            dgScene.MonsterDead();
        }

        //StopAllCoroutines(); //FSM 중단

        StartCoroutine(ActiveAlphaValue(false)); //투명해지며 죽는다.

        _playerCtr.AddInven(_itemDic);//플레이어에게 아이템 전달
        _playerCtr.AddMoney(m_nDropMoney); //플레이어에게 돈 전달
        _playerCtr.SetEXP(m_nDromExp); //플레이어에게 경험치 전달

        this.gameObject.layer = 2;    // Ignore Raycast        
        _anim.SetTrigger(hashDead);   // 쓰러지는 애니메이션
        _nav.enabled = false;        // Nav Mesh 중단
        m_bisDead = true;             // 죽음 상태 true
        StartCoroutine(SetActive(false, 2.0f)); //2초 뒤 비활성화
    }

    IEnumerator SetActive(bool value, float time)
    {
        yield return new WaitForSeconds(time);
        transform.gameObject.SetActive(value);
    }



    /// <summary> 활성화, 비활성화 여부에 따른 현재 오브젝트의 불투명도 </summary>
    IEnumerator ActiveAlphaValue(bool value)
    {
        ChangeRanderingMode.Fade(ref _mat); //쉐이더 랜더링모드를 Fade로 변경

        Color color = _mat.color;

        float time = 0.0f;
        float t = 0.0f;
        float destTime = 2.0f; //2초에 걸쳐서

        if (value == true)
        {
            color.a = 0;
            while (color.a != 1)
            {
                if (time < destTime)
                {
                    time += Time.deltaTime;
                    t = time / destTime;
                }

                color.a = Mathf.Lerp(0, 1, t);

                _mat.color = color;
                yield return null;
            }
            ChangeRanderingMode.Opaque(ref _mat);
        }
        else
        {
            color.a = 1;
            while (color.a != 0)
            {
                if (time < destTime)
                {
                    time += Time.deltaTime;
                    t = time / destTime;
                }

                color.a = Mathf.Lerp(1, 0, t);

                _mat.color = color;
                yield return null;
            }
        }
    }


    /// <summary>
    /// 플레이어가 사망했을시 이벤트
    /// <br></br>Player의 델리게이트에 추가
    /// </summary>
    private void OnPlayerDie()
    {
        StopAllCoroutines();

        //추적 중지
        _nav.enabled = false;
        _destTr = null;

        _anim.SetBool(hashTrace, false);
        _anim.SetBool(hashAttack, false);
    }

    /// <summary> 몬스터의 Die 이후 드롭 할 아이템 세팅 </summary>
    public void SettingDropItem()
    {
        //아이템 ID가 담긴 배열을 반복하며
        //해당 ID의 아이템들을 드롭아이템으로 가진다.
        foreach (int itemID in m_nDropItemArray)
        {
            ItemData itemdata = _resourcesData.GetItem(itemID);

            if (itemdata is PortionItemData portionItemData)
            {
                //포션의 양 랜덤하게 설정
                int nPortionAmount = RandNum(m_nPortionDrop_MinAmount, m_nPortionDrop_MaxAmount);

                AddDropItem(itemdata, nPortionAmount);
            }
            else
            {
                //아이템의 드랍확률에 따라 드랍시킨다.
                AddDropItem(itemdata, 1, m_nItemDrop_percentage);
            }
        }

        int RandNum(int minAmount, int maxAmount)
        {
            int randNum = Random.Range(minAmount, maxAmount);
            return randNum;
        }
    }

    /// <summary>
    /// 몬스터의 데이터를 불러온뒤, 본인 정보에 적용
    /// </summary>
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
        m_sName  = _monsterData.sName;

        m_nMaxHP = _monsterData.nMaxHP;
        m_nCurHP = _monsterData.nMaxHP;

        m_nMaxMP = _monsterData.nMaxMP;
        m_nCurMP = _monsterData.nMaxMP;


        m_nCurSTR  = _monsterData.nCurSTR;
        m_nDefence = _monsterData.nDefence;

        m_nDromExp = _monsterData.nDropExp;
        m_nDropMoney = _monsterData.nDropMoney;

        //아이템 ID가 담긴 Array
        m_nDropItemArray = _monsterData.nDropItemArray;
    }

    private void OnDisable()
    {
        objState = ObjectState.NONE;
    }

    private void OnDestroy()
    {
        
    }
}

