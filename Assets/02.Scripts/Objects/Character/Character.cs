using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InHwan.CircularQueue;

public abstract class Character : ObjectBase
{
    [Header("SkILL Related")]
    [SerializeField] protected SkillData[] _skill_Datas = new SkillData[6];
    
    [SerializeField] protected int m_nMaxExp;
    [SerializeField] protected int m_nCurExp;

    protected string m_sClassName = "워리어";

    public int GetMaxExp()       => m_nMaxExp;
    public int GetCurExp()       => m_nCurExp;
    public string GetClassName() => m_sClassName;

    /// <summary> 모든 스킬 덮어 씌우기 </summary>
    public void SetPlayerSkill(SkillData[] skill_datas) => _skill_Datas = skill_datas;
    /// <summary> 플레이어 스킬 데이터에 할당 </summary>
    public void SetPlayerSkill(int idx, SkillData skillData) => _skill_Datas[idx] = skillData;

    //스킬을 자동 실행할 원형 큐
    // protected CircularQueue<SkillStatus> circualrQueue;
    protected SkillManager _skillMgr;
    protected Skill_InvenUI _skInvenUI;

    private GameObject[] _levelUPEffect; //레벨업 이펙트

    protected override void Awake()
    {
        base.Awake();
       
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

        base.Start(); //_resourcesData 할당
        _levelUPEffect = _resourcesData.GetLevelUPEffect();

        ChangeDamageTextColor(Color.red);
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

        _skillMgr.SetSkillPower(m_nCurSTR);
        _skillMgr.SetSkillDataDamage(); //스킬의 공격력도 업데이트 해준다.
    }

    private void LevelUPEffect()
    {
        int _size = _levelUPEffect.Length;
        GameObject[] _levelUP = new GameObject[_size];
        for (int i = 0; i < _size; i++)
        {
            _levelUP[i] = Instantiate<GameObject>(_levelUPEffect[i], transform.position, _levelUPEffect[i].transform.rotation, this.transform);
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
