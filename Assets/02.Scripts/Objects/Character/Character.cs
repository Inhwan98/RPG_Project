using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InHwan.CircularQueue;

public abstract class Character : ObjectBase
{
    /*********************************************
     *                  Fields
     *********************************************/
    #region option Fields
    [SerializeField]
    protected Transform _weaponTr;
    #endregion
    #region protected Fields
    protected GameObject[] _levelUPEffect; //레벨업 이펙트
    protected GameObject _equipWeaponGo;
    protected SkillData[] _skill_Datas = new SkillData[6];

    protected Skill_InvenManager _skillMgr;
    protected Skill_InvenUI _skInvenUI;

    protected int m_nMaxExp;
    protected int m_nCurExp;
    protected string m_sClassName = "워리어";
    #endregion

    /*********************************************
    *               Get, Set Methods
    *********************************************/
    #region Get Methods
    public int GetMaxExp()       => m_nMaxExp;
    public int GetCurExp()       => m_nCurExp;
    public string GetClassName() => m_sClassName;
    public SkillData[] GetCharacterSillList() => _skill_Datas;
    #endregion
    #region Set Methods
    /// <summary> 모든 스킬 덮어 씌우기 </summary>
    public void SetPlayerSkill(SkillData[] skill_datas) => _skill_Datas = skill_datas;
    /// <summary> 플레이어 스킬 데이터에 할당 </summary>
    public void SetPlayerSkill(int idx, SkillData skillData) => _skill_Datas[idx] = skillData;
    #endregion


    /*********************************************
    *                 Unity Evenet
    *********************************************/

    #region Unity Evenet
    protected override void Start()
    {
        base.Start(); //_resourcesData 할당
        _levelUPEffect = _resourcesData.GetLevelUPEffect();

        ChangeDamageTextColor(Color.red);
    }

    #endregion

    /*********************************************
    *                  Methods
    *********************************************/
    #region override Methods
    protected override void Init_MiniMapIcon()
    {
        base.Init_MiniMapIcon();
        SpriteRenderer charIcon = minimapIcon.GetComponent<SpriteRenderer>();
        charIcon.color = Color.green;
    }
    #endregion

    #region abstract Methods
    /// <summary> 스킬 공격 관련 메서드 </summary>
    protected abstract IEnumerator SkillAttack(int skillNum);
    #endregion

    /// <summary> 레벨업시 등장하는 이펙트 </summary>
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

    #region public Methods
    /// <summary> 레벨업 이펙트 및 스킬 데미지 세팅</summary>
    public virtual void LevelUP()
    {
        LevelUPEffect();

        _skillMgr.SetSkillPower(GetTotalSTR());
        _skillMgr.SetSkillDataDamage(); //스킬의 공격력도 업데이트 해준다.
    }

    /// <summary> 플레이어가 레벨이 같거나 높다면 참 </summary>
    public bool IsCompareWithPlayer(int level)
    {
        return m_nLevel >= level;
    }
    #endregion
}
