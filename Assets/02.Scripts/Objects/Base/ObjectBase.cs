 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    [상속 구조]

    ObjectBase(abstract)
        
        * 캐릭터 관련 Object
        - Character(abstract)
            - PlayerController

        * 몬스터 관련 Object
        - Monster
            - BossMonster

*/


public abstract class ObjectBase : MonoBehaviour
{
    /*************************************************
     *                  Fields
     *************************************************/

    #region option Fields
    [Header("Damage UI")]
    [SerializeField] protected Transform _damageTr;
    #endregion

    #region protected Fields
    protected GameObject minimapIcon;

    [SerializeField] protected int   m_nID;
    protected int   m_nLevel;
    protected int   m_nCurHP;
    protected int   m_nCurMP;
    protected int   m_nCurSTR;
    protected int   m_nSkillDamage;
    protected int   m_nDefence;

    protected int m_nAddDefence;
    protected int m_nAddSTR;
    protected int   m_nMinAttack = 3;
    protected int   m_nMaxAttack = 7;
    
    protected int   m_nMaxHP;
    protected int   m_nMaxMP;
    protected bool  m_bisAttack;
    protected bool  m_bisDead;

    #region DamageText 관련
    protected GameObject _damageTextObj;

    //오브젝트 풀링될 데미지 텍스트
    protected List<GameObject> _damageTextGoList = new List<GameObject>();
    protected List<TextMesh> _damageTextMeshList = new List<TextMesh>();
    protected int _maxDamageTextAmount = 12;
    #endregion

    protected ResourcesData _resourcesData;

    #region Animation Setting
    protected Animator _anim;
    protected readonly int hashDead = Animator.StringToHash("DoDead");
    #endregion

    #endregion
    
    /******************************************************
     *                  Get, Set Methods
     ******************************************************/

    #region Get Methods
    public int GetLevel() => m_nLevel;
    public int GetMaxHP() => m_nMaxHP;
    public int GetCurHP() => m_nCurHP;
    public int GetMaxMP() => m_nMaxMP;
    public int GetCurMP() => m_nCurMP;
    public int GetCurStr() => m_nCurSTR;
    /// <summary> Object Die 상태 반환 </summary>
    public bool GetIsDie() => m_bisDead;
    /// <summary> 스킬데미지 값 반환 </summary>
    public int GetSkillDamage() => m_nSkillDamage;
    public int GetID() => m_nID;
    public int GetMinAttack() => m_nMinAttack;
    public int GetMaxAttack() => m_nMaxAttack;
    public int GetDefence()   => m_nDefence;
    public int GetTotalSTR() => m_nCurSTR + m_nAddSTR;
    public int GetTotalDefence() => m_nDefence + m_nAddDefence;
    #endregion
    #region Set Methods
    public int SetID(int value) => m_nID = value;
    /// <summary> 스킬데미지 값 수정 </summary>
    public void SetSkillDamage(int _skillDamage) => m_nSkillDamage = _skillDamage;
    //추가 공격력과 추가 방어력의 관리
    public void SetAddDefence(int value) => m_nAddDefence = value;
    public void SetAddSTR(int value) => m_nAddSTR = value;
    public void SubDefence(int value) => m_nAddDefence -= value;
    public void SubSTR(int value) => m_nAddSTR -= value;
    #endregion


    /******************************************************
    *                    Unity Evenet
    ******************************************************/
    #region Unity Evenet
    protected virtual void Awake()
    {
        LoadData();
        InitObj();
        CreateDamageTextPool();
    }
    protected virtual void Start()
    {
        _resourcesData = GameManager.instance.GetResourcesData();
        Init_MiniMapIcon();
    }

    #endregion

    /******************************************************
     *                    Methods
     ******************************************************/

    #region virtual Methods
    //Init Object Setting
    protected virtual void InitObj()
    {
        _anim = GetComponent<Animator>();
    }
    protected virtual void Init_MiniMapIcon()
    {
        minimapIcon = _resourcesData.GetMinimapObjIcon();
        minimapIcon = Instantiate(minimapIcon, transform);
    }
    public virtual void Buff(int _str) { }
    #endregion
    #region protected abstract Methods
    /// <summary> 공격 관련 </summary>
    protected abstract IEnumerator Attack();
    /// <summary> 체력이 0 이하일 때 Die Event </summary>
    protected abstract void Die();
    /// <summary> 대미지를 입었을 때 함수 </summary>
    public abstract void OnDamage(int _str, bool _isKnockback = false, Transform posTr = null);
    /// <summary> 각 객체에 맞게 Data Load </summary>
    protected abstract void LoadData();
    #endregion

    #region private Methods
    private void CreateDamageTextPool()
    {
        _damageTextObj = Resources.Load<GameObject>("Prefab/DamageText");

        GameObject damageText;
        for (int i = 0; i < _maxDamageTextAmount; i++)
        {
            damageText = Instantiate(_damageTextObj, _damageTr.position, Quaternion.identity, transform);
            damageText.SetActive(false);
            _damageTextGoList.Add(damageText);
            _damageTextMeshList.Add(damageText.GetComponent<TextMesh>());
        }
    }
    /// <summary>
    /// 데미지 텍스트를 풀에서 꺼내 온다.
    /// </summary>
    /// <param name="index">TextMesh와 대응하는 index를 out </param>
    private GameObject GetDamageTextInPool(out int index)
    {
        for (int i = 0; i < _damageTextGoList.Count; i++)
        {
            if (_damageTextGoList[i].activeSelf == false)
            {
                index = i;
                return _damageTextGoList[i];
            }
        }

        //풀 안에 객체가 없을 경우 새로 생성
        GameObject damageText;
        damageText = Instantiate(_damageTextObj, _damageTr.position, Quaternion.identity, transform);
        damageText.SetActive(false);
        _damageTextGoList.Add(damageText);
        _damageTextMeshList.Add(damageText.GetComponent<TextMesh>());

        index = _damageTextGoList.Count - 1;
        return null;
    }
    #endregion
    #region protected Methods
    /// <summary>
    /// 데미지 텍스트를 활성화 시키는 함수
    /// </summary>
    /// <param name="_power"> 화면상에 나타낼 수치 </param>
    protected void ActiveDamageText(int _power)
    {
        int index;
        //데미지 텍스트  발생. 오브젝트 풀링할 것
        GameObject damageText = GetDamageTextInPool(out index);

        damageText.transform.position = _damageTr.position;
        damageText.SetActive(true);
        _damageTextMeshList[index].text = $"{_power}";
    }
    /// <summary>
    /// 데미지 텍스트의 색상을 변경해준다.
    /// </summary>
    protected void ChangeDamageTextColor(Color color)
    {
        foreach (var damageText in _damageTextMeshList)
        {
            damageText.color = color;
        }
    }
    #endregion
    #region public Methods
    public int GetAttackPower(int nPower)
    {
        int minDamage = nPower * m_nMinAttack;
        int maxDamage = nPower * m_nMaxAttack;
        int nDamage = Random.Range(minDamage, maxDamage + 1);
        return nDamage;
    }
    #endregion
}



