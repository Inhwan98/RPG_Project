using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InHwan.CircularQueue;

public abstract class Character : ObjectBase
{
    [Header("SkILL Related")]
    [SerializeField] protected int skillMaxAmount = 3; //보유할 최대 스킬 개수
    [SerializeField] protected SkillData[] _skill_Datas = new SkillData[6];

    //스킬을 자동 실행할 원형 큐
    // protected CircularQueue<SkillStatus> circualrQueue;
    protected SkillManager _skillMgr;
    protected Skill_InvenUI _skInvenUI;

    private GameObject[] levelUPEffect; //레벨업 이펙트

    protected override void Awake()
    {
        base.Awake();
        m_nMaxExp = m_nLevel * 20;
        levelUPEffect = Resources.LoadAll<GameObject>("Prefab/LevelUP");
    }

    protected override void Start()
    {
        #region 자동스킬 사용 세팅
        //circualrQueue = new CircularQueue<SkillStatus>(skillMaxAmount);
        //기초 세팅 될 스킬을 미리 원형 큐에 넣어 둔다.
        //foreach (var a in skill_List)
        //{
        //    circualrQueue.EnQueue(a);
        //}
        #endregion

        base.Start();
    }


    public override void Buff(int _str)
    {
        this.m_nCurHP += _str;
        
        if (m_nCurHP >= m_nMaxHP)// 체력이 맥스 이상이면
        {
            m_nCurHP = m_nMaxHP;
        }
    }

    /// <summary> 레벨업 이펙트 및 스킬 데미지 세팅</summary>
    public virtual void LevelUP()
    {
        LevelUPEffect();

        //09.24 스킬매니져 참조 필요해 보임.
        //게임메니져로 전부 관리하면 가시성이 있을까?
        //끝. 예비군 3일 후 복귀 예정
        _skillMgr.SetSkillPower(m_nCurSTR);
        _skillMgr.SetSkillDataDamage(); //스킬의 공격력도 업데이트 해준다.
    }

    private void LevelUPEffect()
    {
        int _size = levelUPEffect.Length;
        GameObject[] _levelUP = new GameObject[_size];
        for (int i = 0; i < _size; i++)
        {
            _levelUP[i] = Instantiate<GameObject>(levelUPEffect[i], transform.position, levelUPEffect[i].transform.rotation, this.transform);
        }

        foreach (var v in _levelUP) Destroy(v, 4.0f);
    }

    public SkillData[] GetCharacterSillList()
    {
        return _skill_Datas;
    }


    protected override void Die() { }
    protected abstract IEnumerator SkillAttack(int skillNum);
}
