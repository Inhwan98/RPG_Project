 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectBase : MonoBehaviour
{
    [Header("Currnet Info")]
    [SerializeField] protected int   m_nID;
    [SerializeField] protected int   m_nLevel;
    [SerializeField] protected int   m_nCurHP;
    [SerializeField] protected int   m_nCurMP;
    [SerializeField] protected int   m_nCurSTR;
    [SerializeField] protected int   m_nSkillDamage;

    [SerializeField] protected int   m_nDefence;

    [Header("Damage UI")]
    [SerializeField] protected Transform _damageTr;
    protected GameObject _damageTextObj;

    protected List<GameObject> _damageTextGoList = new List<GameObject>();
    protected List<TextMesh> _damageTextMeshList = new List<TextMesh>();
    protected int _maxDamageTextAmount = 12;

    protected int   m_nMinAttack = 3;
    protected int   m_nMaxAttack = 7;
    
    protected int   m_nMaxHP;
    protected int   m_nMaxMP;
    protected bool  m_bisAttack;
    protected bool  m_bisDead;
    
    protected ResourcesData _resourcesData;

    #region Animation Setting
    protected Animator _anim;
    protected readonly int hashDead = Animator.StringToHash("DoDead");
    #endregion

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
    public int SetID(int value) => m_nID = value;

    public int GetMinAttack() => m_nMinAttack;
    public int GetMaxAttack() => m_nMaxAttack;
    public int GetDefence()   => m_nDefence;


    /// <summary> 스킬데미지 값 수정 </summary>
    public void SetSkillDamage(int _skillDamage) => m_nSkillDamage = _skillDamage;

    /// <summary> Resource Data 클래스 </summary>
    //protected ResourcesData _resourcesData;

    protected virtual void Awake()
    {
        LoadData();
        InitObj();
        CreateDamageTextPool();
    }

    protected virtual void Start()
    {
        _resourcesData = GameManager.instance.GetResourcesData();
    }

    //Init Object Setting
    private void InitObj()
    {
        _anim = GetComponent<Animator>();
    }

    protected virtual void Getinfo()
    {
        Debug.Log($"HP : {m_nCurHP}");
        Debug.Log($"STR : {m_nCurSTR}");
    }

    #region 데미지 텍스트 관련
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
    public GameObject GetDamageTextInPool(out int index)
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

    /// <summary>
    /// 데미지 텍스트를 활성화 시키는 함수
    /// </summary>
    /// <param name="_power"> 화면상에 나타낼 수치 </param>
    public void ActiveDamageText(int _power)
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

    public int GetAttackPower(int nPower)
    {
        int minDamage = nPower * m_nMinAttack;
        int maxDamage = nPower * m_nMaxAttack;
        int nDamage = Random.Range(minDamage, maxDamage + 1);
        return nDamage;
    }
    

    public virtual void Buff(int _str) { }

    /// <summary> 공격 관련 </summary>
    protected abstract IEnumerator Attack();

    /// <summary> 체력이 0 이하일 때 Die Event </summary>
    protected abstract void Die();

    /// <summary> 대미지를 입었을 때 함수 </summary>
    public abstract void OnDamage(int _str, bool _isKnockback = false, Transform posTr = null);

    /// <summary> 각 객체에 맞게 Data Load </summary>
    protected abstract void LoadData();


}



