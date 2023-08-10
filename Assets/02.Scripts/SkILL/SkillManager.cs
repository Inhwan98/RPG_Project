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

    public void UseSkill(SkillStatus _sk, ObjectBase _selfCtr, ref float _objectMP)
    {
        _objectMP -= _sk.GetSkillManaAmount(); //사용 Object에서 스킬 마나만큼 차감
        float _fAttackRange = _sk.GetAttackRange();

        
        switch (_sk.GetSkillType())
        {
            case SkillType.SINGLE:

                //GameObject _hitEffect = Instantiate(_sk.GetEffectObj(), _objCtr.transform.position, Quaternion.identity, _objCtr.transform);
                //Destroy(_hitEffect, 1f);
                break;

            case SkillType.ARROUND_TARGET:
               

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