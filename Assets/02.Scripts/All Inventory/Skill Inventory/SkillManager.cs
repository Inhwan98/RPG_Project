using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private Skill_InvenUI _skInvenUI;
   
    /// <summary> 아이템 목록 </summary>
    private SkillData[] _skillarray;
    private SkillData[] _playerSkillarray;

    private int _maxSkillSize = 6;

    private float power_STR; // 유저의 힘을 할당받을 것
    private PlayerController _playerCtr;


    private void Awake()
    {
        _skInvenUI.SetSkillTreeReference(this);

        Init_Skills();
    }

    private void Start()
    {
        SetSkillDataDamage();
        //HandOverPlayerSkills();

        UpdateAllSlot();
        UpdateAccessibleStatesAll();

    }

    private void OnDestroy()
    {
        //SaveInven();
    }

    public SkillData GetSKillData(int idx)
    {
        if (!IsValidIndex(idx)) return null;
        if (_skillarray[idx] == null) return null;

        return _skillarray[idx];
    }

    /// <summary> 인덱스가 수용 범위 내에 있는지 검사 </summary>
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < _maxSkillSize;
    }


    /// <summary> 모든 슬롯 UI에 접근 가능 여부 업데이트 </summary>
    private void UpdateAccessibleStatesAll()
    {
        _skInvenUI.SetAccessibleSlotRange(_maxSkillSize);
    }

    /// <summary> 해당 슬롯이 스킬을 갖고 있는지 여부 </summary>
    public bool HasSkill(int index)
    {
        return IsValidIndex(index) && _skillarray[index] != null;
    }


    /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (_skillarray[index] == null) return "";

        return _skillarray[index].GetSKillName();
    }


    public void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        SkillData skillA = _playerSkillarray[indexA];
        SkillData skillB = _playerSkillarray[indexB];

        _playerSkillarray[indexA] = skillB;
        _playerSkillarray[indexB] = skillA;

        // 두 슬롯 정보 갱신
        UpdateSlot(indexA, indexB);
    }

    /// <summary> 해당하는 인덱스의 슬롯들의 상태 및 UI 갱신 </summary>
    private void UpdateSlot(params int[] indices)
    {
        foreach (var i in indices)
        {
            UpdateSlot(i);
        }
    }

    /// <summary> 해당하는 인덱스의 슬롯 상태 및 UI 갱신 </summary>
    public void UpdateSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        SkillData skData = _playerSkillarray[index];

        // 1. 아이템이 슬롯에 존재하는 경우
        if (skData != null)
        {
            //아이콘 등록
            _skInvenUI.SetItemIcon(index, skData.GetSkill_Sprite());

        }
        //2. 빈 슬롯인 경우 : 아이콘 제거
        else
        {
            RemoveIcon();
        }

        // 로컬 : 아이콘 제거하기
        void RemoveIcon()
        {
            _skInvenUI.RemoveItem(index);
        }
    }


    ///<summary> 해당 슬롯의 아이템 제거 </summary>
    public void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        _playerSkillarray[index] = null;
        UpdateSlot(index);
    }


    /// <summary> 모든 슬롯들의 상태를 UI에 갱신 </summary>
    private void UpdateAllSlot()
    {
        for (int i = 0; i < _maxSkillSize; i++)
        {
            UpdateSlot(i);
        }
    }

    public void SetPlayerCtr(PlayerController player)
    {
        _playerCtr = player;
    }

    public void SetSkillPower(float value)
    {
        power_STR = value;
    }

    /// <summary> Inventory 활성화 유무 (마우스 커서도 같이 활성화) </summary>
    public void SkillWindowActive(bool value)
    {
        _skInvenUI.gameObject.SetActive(value);
        //_playerCtr.SetUseInven(value);             //플레이어의 움직임 제어
        _playerCtr.GetCameraCtr().UseWindow(value); //카메라의 회전 제어

        if (value) GameManager.instance.VisibleCursor();
        else
            GameManager.instance.InvisibleCursor();
    }



    /// <summary> Load Data 없을시 초기화 </summary>
    public void Init_Skills()
    { 
        _skillarray = SaveSys.LoadSkillSet("AllSkillSetData.Json");
        _playerSkillarray = new SkillData[_maxSkillSize];

        if (_skillarray == null)
        {
            _skillarray = new SkillData[_maxSkillSize];
            return;
        }
        else
        {
            for (int i = 0; i < _skillarray.Length; i++)
            {
                if (_skillarray[i] == null) continue;

                _skillarray[i].Init();                
                //_skills[i].SetSkillDamage(power_STR);
            }
        }
    }

    public void UseSkill(SkillData _skilldata, ObjectBase _selfCtr, ref float _objectMP)
    {
        _objectMP -= _skilldata.GetSkillManaAmount(); //사용 Object에서 스킬 마나만큼 차감
        //float _fAttackRange = _sk.GetAttackRange(); //공격 범위
        _playerCtr.SetSkillDamage(_skilldata.GetSkillDamage()); //현재 스킬의 데미지를 플레이어에게 전달.

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

    /// <summary> 스킬의 데미지 설정 : 플레이어 비례 </summary>
    public void SetSkillDataDamage()
    {
        foreach (SkillData skill in _skillarray)
        {
            if (skill == null) continue;
            skill.SetSkillDamage(power_STR);
        }
    }
    /// <summary> 플레이어에게 스킬 초기화 해서 전달. </summary>
    //private void HandOverPlayerSkills()
    //{
    //    foreach (var a in _skills)
    //    {
    //        if (a == null) continue;
    //        if (a.GetIsAcquired())
    //        {
    //            _playerHaveSkills.Add(a);
    //        }
    //    }

    //    _playerCtr.SetPlayerSkills(_playerHaveSkills.ToArray());
    //}
}
