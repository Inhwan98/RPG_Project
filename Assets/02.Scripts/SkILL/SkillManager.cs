using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance = null;

    [SerializeField] private float power_STR; // 유저의 힘을 할당받을 것
    [SerializeField] protected LayerMask targetLayer; // 몬스터의 레이어
    [SerializeField] private List<SkillData> Skills = new List<SkillData>(); // 게임 내 모든 스킬

    private PlayerController playerCtr;

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
        //스킬의 animtion Parameter 해쉬화
        Skills = SaveSys.LoadSkillSet();
        foreach (SkillData skill in Skills) skill.Init();
           
    }

    private void Start()
    {
        //스킬의 Power로 유저의 STR을 받아옴.
        //적들의 OnDamge 함수를 호출해서 인자로 넘길 것
        playerCtr = PlayerController.instance;
        power_STR = playerCtr.GetCurStr();
        //스킬 데미지를 플레이어의 STR, 스킬의 % 수치에 비례해서 설정
        //각 스킬은 고유의 % 데미지를 가지고 있음
        foreach (SkillData skill in Skills) skill.SetSkillDamage(power_STR);
    }

    private void Update()
    {
        
    }

    //기초 스킬 세팅을 넘겨주기 위한 준비
    public List<SkillData> BasicSkillSet()
    {
        return Skills;
    }

    public void UseSkill(SkillData _sk, ObjectBase _selfCtr, ref float _objectMP)
    {
        _objectMP -= _sk.GetSkillManaAmount(); //사용 Object에서 스킬 마나만큼 차감
        //float _fAttackRange = _sk.GetAttackRange(); //공격 범위
        playerCtr.SetSkillDamage(_sk.GetSkillDamage()); //현재 스킬의 데미지를 플레이어에게 전달.

        #region 스킬타입
        //switch (_sk.GetSkillType())
        //{
        //    case SkillType.SINGLE:

        //        //GameObject _hitEffect = Instantiate(_sk.GetEffectObj(), _objCtr.transform.position, Quaternion.identity, _objCtr.transform);
        //        //Destroy(_hitEffect, 1f);
        //        break;

        //    case SkillType.ARROUND_TARGET:


        //        break;

        //    case SkillType.BUFF_SELF:
        //        _selfCtr.Buff(power_STR);
        //        GameObject _healEffect = Instantiate(_sk.GetEffectObj(), _selfCtr.transform.position + Vector3.up, Quaternion.identity, _selfCtr.transform);
        //        Destroy(_healEffect, 2f);
        //        break;
        //}
        #endregion
    }

    public void CharacterLevelUP(float _STR)
    {
        //캐릭터가 레벨업하면 스킬 데미지도 재설정
        power_STR = _STR;
        foreach (SkillData skill in Skills) skill.SetSkillDamage(power_STR);
    }

}