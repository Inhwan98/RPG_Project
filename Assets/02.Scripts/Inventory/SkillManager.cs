using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private SkillTreeUI _skillTreeUI;
   
    /// <summary> 아이템 목록 </summary>
    private SkillData[] _skills;
    private List<SkillData> _playerHaveSkills = new List<SkillData>();

    private int _maxSkillSize = 4;

    private float power_STR; // 유저의 힘을 할당받을 것
    private PlayerController _playerCtr;


    private void Awake()
    {
        _skillTreeUI.SetSkillTreeReference(this);

        Init_Skills();
    }

    private void Start()
    {
        //SetSkillDataDamage();
        HandOverPlayerSkills();

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
        if (_skills[idx] == null) return null;

        return _skills[idx];
    }

    /// <summary> 인덱스가 수용 범위 내에 있는지 검사 </summary>
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < _maxSkillSize;
    }

    /// <summary> 앞에서부터 비어있는 슬롯 인덱스 탐색 </summary>
    private int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < _maxSkillSize; i++)
        {
            if (_skills[i] == null)
                return i;
        }

        return -1;
    }

    /// <summary> 모든 슬롯 UI에 접근 가능 여부 업데이트 </summary>
    private void UpdateAccessibleStatesAll()
    {
        _skillTreeUI.SetAccessibleSlotRange(_maxSkillSize);
    }

    /// <summary> 해당 슬롯이 스킬을 갖고 있는지 여부 </summary>
    public bool HasSkill(int index)
    {
        return IsValidIndex(index) && _skills[index] != null;
    }


    /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (_skills[index] == null) return "";

        return _skills[index].GetSKillName();
    }


    public void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        SkillData skillA = _skills[indexA];
        SkillData skillB = _skills[indexB];

        _skills[indexA] = skillB;
        _skills[indexB] = skillA;

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

        SkillData skData = _skills[index];


        // 1. 아이템이 슬롯에 존재하는 경우
        if (skData != null)
        {
            //아이콘 등록
            _skillTreeUI.SetItemIcon(index, skData.GetSkill_Sprite());

        }
        //2. 빈 슬롯인 경우 : 아이콘 제거
        else
        {
            RemoveIcon();
        }

        // 로컬 : 아이콘 제거하기
        void RemoveIcon()
        {
            _skillTreeUI.RemoveItem(index);
        }
    }



    ///<summary> 인벤토리에 아이템 추가
    /// <para/> 넣는 데 실패한 아이템 개수 리턴
    /// <para/> 리턴이 0이면 넣는데 모두 성공했다는 의미
    /// </summary>
    public int Add(SkillData skillData, int amount = 1)
    {
        if (skillData == null) return 0;

        int index;


        // 2-1. 1개만 넎는 경우, 간단히 수행
        if (amount == 1)
        {
            index = FindEmptySlotIndex();
            if (index != -1)
            {
                //아이템을 생성하여 슬롯에 추가
                _skills[index] = skillData;
                amount = 0;

                UpdateSlot(index);
            }
        }

        // 2-2. 2개 이상의 수량 없는 아이템을 동시에 추가하는 경우
        index = -1;
        for (; amount > 0; amount--)
        {
            //아이템 넣은 인덱스의 다음 인덱스부터 슬롯 탐색
            index = FindEmptySlotIndex(index + 1);

            // 다 넣지 못한 경우 루프 종료
            if (index == -1)
            {
                break;
            }

            //아이템을 생성하여 슬롯에 추가
            _skills[index] = skillData;

            UpdateSlot(index);
        }


        return amount;
    }

    ///<summary> 해당 슬롯의 아이템 제거 </summary>
    public void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        _skills[index] = null;
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
        _skillTreeUI.gameObject.SetActive(value);
        //_playerCtr.SetUseInven(value);             //플레이어의 움직임 제어
        _playerCtr.GetCameraCtr().UseWindow(value); //카메라의 회전 제어

        if (value) GameManager.instance.VisibleCursor();
        else
            GameManager.instance.InvisibleCursor();
    }



    /// <summary> Load Data 없을시 초기화 </summary>
    public void Init_Skills()
    { 
        _skills = SaveSys.LoadSkillSet("AllSkillSetData.Json");

        if (_skills == null)
        {
            _skills = new SkillData[_maxSkillSize];
            return;
        }
        else
        {
            for (int i = 0; i < _skills.Length; i++)
            {
                if (_skills[i] == null) continue;

                _skills[i].Init();
                _skills[i].SetSkillDamage(power_STR);
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
        foreach (SkillData skill in _skills) skill.SetSkillDamage(power_STR);
    }

    /// <summary> 플레이어에게 스킬 초기화 해서 전달. </summary>
    private void HandOverPlayerSkills()
    {
        foreach(var a in _skills)
        {
            if (a == null) continue;
            if(a.GetIsAcquired())
            {
                _playerHaveSkills.Add(a);
            }
        }

        _playerCtr.SetPlayerSkills(_playerHaveSkills.ToArray());
    }
}
