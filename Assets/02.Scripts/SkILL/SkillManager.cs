using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance = null;

    [SerializeField] private float power_STR; // 유저의 힘을 할당받을 것
    [SerializeField] protected LayerMask targetLayer; // 몬스터의 레이어
    [SerializeField] private List<SkillStatus> Skills = new List<SkillStatus>(); // 게임 내 모든 스킬

    private PlayerController playerCtr;

    [Header("Damage UI")]
    private GameObject damageTextObj;
    

    private void Awake()
    {
        #region SingTone
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        #endregion

        foreach (SkillStatus skill in Skills) skill.ChangeAnimHash();
    }

    private void Start()
    {
        damageTextObj = Resources.Load<GameObject>("Prefab/DamageUI");
        //스킬의 Power로 유저의 STR을 받아옴.
        //적들의 OnDamge 함수를 호출해서 인자로 넘길 것
        playerCtr = PlayerController.instance;
        power_STR = playerCtr.GetSTR();

    }

    //기초 스킬 세팅을 넘겨주기 위한 준비
    public List<SkillStatus> BasicSkillSet()
    {
        return Skills;
    }

    public void UseSkill(SkillStatus _sk, Transform _targetTr, ObjectBase _selfCtr, ref float _objectMP)
    {
        _objectMP -= _sk.GetSkillManaAmount(); //사용 Object에서 스킬 마나만큼 차감

        UI_Damage damageUICtr;
        GameObject _damageText;
        ObjectBase _objCtr  = _targetTr?.GetComponent<ObjectBase>();
        float _fAttackRange = _sk.GetAttackRange();

        
        switch (_sk.GetSkillType())
        {
            case SkillType.SINGLE:
                //공격력 만큼 피해
                _objCtr.OnDamage(power_STR);
                GameObject _hitEffect = Instantiate(_sk.GetEffectObj(), _objCtr.transform.position, Quaternion.identity, _objCtr.transform);

                _damageText = Instantiate(damageTextObj, _objCtr.transform.position, Quaternion.identity, _objCtr.transform);
                damageUICtr = _damageText.GetComponent<UI_Damage>();
                damageUICtr.SetDamageText(power_STR);

                Destroy(_hitEffect, 1f);
               
                break;

            case SkillType.ARROUND_TARGET:
                GameObject _arroundEffect = Instantiate(_sk.GetEffectObj(), _objCtr.transform.position, Quaternion.identity, _objCtr.transform);

                _arroundEffect.transform.localScale = Vector3.one * _sk.GetAttackRange();
                Destroy(_arroundEffect, 1f);
                Collider[] targets = Physics.OverlapSphere(_targetTr.position, _fAttackRange, targetLayer); // 공격 범위 적용
                bool isTargetDetected = targets.Length > 0;

                if (isTargetDetected)
                {
                    foreach (Collider target in targets)
                    {
                        //모든 생명체는 ObjectBase를 가지고 있을 것
                        _objCtr = target.GetComponent<ObjectBase>();

                        _objCtr.OnDamage(power_STR);

                        _damageText = Instantiate(damageTextObj, _objCtr.transform.position, Quaternion.identity, _objCtr.transform);
                        damageUICtr = _damageText.GetComponent<UI_Damage>();
                        damageUICtr.SetDamageText(power_STR);
                    }
                }
                else
                {
                    Debug.Log("감지된거 없음");
                }

                break;

            case SkillType.BUFF_SELF:
                _selfCtr.Buff(power_STR);
                GameObject _healEffect = Instantiate(_sk.GetEffectObj(), _selfCtr.transform.position + Vector3.up, Quaternion.identity, _selfCtr.transform);
                Destroy(_healEffect, 2f);
                break;
        }
    }

    public void CharacterLevelUP(float _STR)
    {
        power_STR = _STR;
    }


}