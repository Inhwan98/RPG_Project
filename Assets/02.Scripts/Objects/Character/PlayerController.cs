using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InHwan.CircularQueue;

public class PlayerController : Character
{
    public static PlayerController instance = null;
    //플레이가 죽었을 때, 모든 몬스터 제어
    public delegate void PlayerDiehandler();             //델리게이트 선언
    public static event PlayerDiehandler OnPlayerDie;    //이벤트 선언

    private Rigidbody rigid;

    private readonly int hashStrafe = Animator.StringToHash("Strafe");
    private readonly int hashForward = Animator.StringToHash("Forward");
    private readonly int hashRun = Animator.StringToHash("Run");
    private readonly int hashAttack = Animator.StringToHash("DoAttack");
    private readonly int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

    [SerializeField] private float moveSpeed = 5.0f;

    Camera cam;
    float xInput;
    float zInput;
    float runInput;
    bool attackInput;

    //몬스터를 자동 추적할 몬스터 리스트
    private List<KeyValuePair<int, Transform>> monsterObjs_list = new List<KeyValuePair<int, Transform>>();
    //플레이어 UI (HPbar) Manager
    private PlayerUIManager playerUICtr;
    


    protected override void Awake()
    {
        base.Awake();
        rigid = GetComponent<Rigidbody>();
        #region SingTone
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        #endregion
    }

    protected override void Start()
    {
        cam = Camera.main;
        playerUICtr   = PlayerUIManager.instance;
        playerUICtr.DisplayInfo(m_nLevel, m_fMaxHP, m_fMaxMP, m_fCurSTR, m_fAttackRange);
        base.Start();
    }

    private void FixedUpdate()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");
        runInput = Input.GetAxis("Run");
        attackInput = Input.GetButtonDown("Fire1");


        Vector3 direction = new Vector3(xInput, 0, zInput);

        if (attackInput) anim.SetTrigger(hashAttack);

        anim.SetFloat(hashStrafe, direction.x);
        anim.SetFloat(hashForward, direction.z);
        anim.SetFloat(hashRun, runInput);

        float movementSpeed = moveSpeed;
        if (runInput < 1.0f) movementSpeed *= 0.5f;
        Vector3 movement = direction.normalized * movementSpeed * Time.fixedDeltaTime;

        anim.SetFloat(hashMoveSpeed, movementSpeed);

        Quaternion destRot = cam.transform.localRotation;
        destRot.x = 0;
        destRot.z = 0;

        rigid.transform.rotation = destRot;
        //if (zInput >= 0.1)
        //{
        //    Quaternion destRot = cam.transform.localRotation;
        //    destRot.x = 0;
        //    destRot.z = 0;

        //    rigid.rotation = destRot;
        //}

        rigid.transform.Translate(movement);
    }
    private void Input_init()
    {

    }

    public override void LevelUP()
    {
        base.LevelUP();
        //원거리 직업은 공격 사거리도 늘어나게?
        m_fAttackRange = statusSetting.GetAttackRange();

        playerUICtr.DisplayInfo(m_nLevel, m_fMaxHP, m_fMaxMP, m_fCurSTR, m_fAttackRange);
    }

    protected override IEnumerator Attack()
    {
        //현재 사용할 스킬
        SkillStatus curSkill = circualrQueue.Front();

        //보유 마나보다 스킬 마나가 크다면 공격 중단.
        //if (m_fCurMP < curSkill.GetSkillManaAmount()) yield break; 공격 중단 없음

        //wandTrail.enabled = true; // 무기 잔상 활성화
        //Debug.Assert(destTr, "destTr is NULL !!!");



        m_bisAttack = true;
        //transform.LookAt(destTr);

        anim.SetTrigger(curSkill.GetAnimHash());
        //skillMgr.UseSkill(curSkill, destTr, this, ref m_fCurMP);
        playerUICtr.SetMPbar(m_fCurMP, m_fMaxMP);

        yield return new WaitForSeconds(1);
        m_bisAttack = false;
        circualrQueue.DeQueue(); //스킬 순서 당겨 주기
    }

    public override void Buff(float _str)
    {
        base.Buff(_str);
        playerUICtr.SetHPbar(m_fCurHP, m_fMaxHP); //HP Bar UI 수정
    }

    public override void OnDamage(float _str)
    {
        this.m_fCurHP -= _str;
        //Debug.Log($"{this.name} 가 {_str} 대미지를 입었다. 현재 체력 {m_nCurHP}");

        //if (m_fCurHP <= 0) this.objState = ObjectState.DEAD; //사망 없음

        if (m_fCurHP <= 0) m_fCurHP = 0;
        playerUICtr.SetHPbar(m_fCurHP, m_fMaxHP); //HP Bar UI 수정
    }

    protected override void Die()
    {
        Debug.Log("Player Die");
        StopAllCoroutines();
        anim.SetTrigger(hashDead);
        m_bisDead = true;

        OnPlayerDie(); //delegate event Play
        GameManager.instance.PlayerDie();
    }

    #region 몬스터 추적 관리
    //몬스터 생성시, 플레이어가 추적할 몬스터 리스트에 추가
    public void AddMonsterObjs(int _curMonidx, Transform _mon)
    {
        KeyValuePair<int, Transform> pair = new KeyValuePair<int, Transform>(_curMonidx, _mon);

        monsterObjs_list.Add(pair);

    }

    //몬스터 사망시, 플레이가 추적할 몬스터 리스트에서 삭제
    public void SubMonsterObjs(int _monidx, Transform _mon)
    {
        KeyValuePair<int, Transform> pair = new KeyValuePair<int, Transform>(_monidx, _mon);

        int idx = monsterObjs_list.IndexOf(pair);
        monsterObjs_list.RemoveAt(idx);
    }
    #endregion

    public float GetSTR() { return m_fCurSTR;}
    public float GetHP()  { return m_fCurHP; }

    public void SetEXP(int _exp)
    {
        this.m_nCurExp += _exp;
        if(m_nCurExp >= m_nMaxExp)
        {
            LevelUP();
        }
        playerUICtr.SetEXPbar(m_nCurExp, m_nMaxExp);
    }
    
}
