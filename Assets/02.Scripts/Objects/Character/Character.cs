using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InHwan.CircularQueue;

abstract public class Character : ObjectBase
{
    [Header("SkILL Related")]
    [SerializeField] protected int skillMaxAmount = 3; //보유할 최대 스킬 개수
    [SerializeField] protected List<SkillStatus> skill_List;

    //스킬을 자동 실행할 원형 큐
    protected CircularQueue<SkillStatus> circualrQueue;
    protected SkillManager skillMgr;
    private GameObject[] levelUPEffect; //레벨업 이펙트

    protected override void Awake()
    {
        base.Awake();
        m_nMaxExp = m_nLevel * 20;
        levelUPEffect = Resources.LoadAll<GameObject>("Prefab/LevelUP");
    }

    protected override void Start()
    {
        skillMgr = SkillManager.instance;

        #region 기본스킬 세팅
        skill_List = skillMgr.BasicSkillSet();
        circualrQueue = new CircularQueue<SkillStatus>(skillMaxAmount);
        //기초 세팅 될 스킬을 미리 원형 큐에 넣어 둔다.
        foreach (var a in skill_List)
        {
            circualrQueue.EnQueue(a);
        }
        #endregion

        base.Start();
    }


    public override void Buff(float _str)
    {
        this.m_fCurHP += _str;
        
        if (m_fCurHP >= m_fMaxHP)// 체력이 맥스 이상이면
        {
            m_fCurHP = m_fMaxHP;
        }
    }

    public virtual void LevelUP()
    {
        //Status Set

        LevelUPEffect();

        statusSetting.LevelUP();

        m_nLevel  = statusSetting.GetLevel();
        m_nCurExp = statusSetting.GetExp();
        m_fMaxHP  = statusSetting.GetMaxHP();
        m_fMaxMP  = statusSetting.GetMaxMP();
        m_fCurSTR = statusSetting.GetSTR();

        m_fCurHP  = m_fMaxHP;
        m_fCurMP  = m_fMaxMP;
        m_nMaxExp = m_nLevel * 20;

        skillMgr.CharacterLevelUP(m_fCurSTR); //스킬의 공격력도 업데이트 해준다.
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

    protected override void Die() { }
}
