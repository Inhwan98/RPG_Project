using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private Skill_InvenUI _skInvenUI;
   
    /// <summary> 아이템 목록 </summary>
    private SkillData[] _skillArray;
    private SkillData[] _playerSkillArray;

    private int _maxSkillSize = 6;

    private float power_STR; // 유저의 힘을 할당받을 것
    private PlayerController _playerCtr; //Player의 Awake()에서 참조시킴


    private void Awake()
    {
        _skInvenUI.SetSkillTreeReference(this);

        Init_Skills();
        
    }

    private void Start()
    {
        SetSkillDataDamage();
        HandOverPlayerSkills();

        UpdateAllSlot();
        UpdateAccessibleStatesAll();
        UpdateAccessibleAcquiredState();
    }

    private void OnDestroy()
    {
        //SaveInven();
    }


    /// <summary> 스킬 인벤의 데이터</summary>
    public SkillData GetInvenSKillData(int idx)
    {
        if (!IsValidIndex(idx)) return null;
        if (_skillArray[idx] == null) return null;

        return _skillArray[idx];
    }

    /// <summary> 스킬 인벤의 모든 데이터</summary>
    public SkillData[] GetInvenAllSKillData()
    {
        if (_skillArray == null)
        {
            Debug.LogError("Skill Data NULL!!!");
            return null;
        }
        return _skillArray;
    }


    /// <summary> 플레이어가 습득한 스킬 데이터</summary>
    public SkillData GetPlayerSKillData(int idx)
    {
        if (!IsValidIndex(idx)) return null;
        if (_playerSkillArray[idx] == null) return null;

        return _playerSkillArray[idx];
    }

    /// <summary> 인덱스가 수용 범위 내에 있는지 검사 </summary>
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < _maxSkillSize;
    }

    /// <summary> 습득하지 않은 스킬들 비활성화 </summary>
    public void UpdateAccessibleAcquiredState()
    {
        _skInvenUI.SetItemAccessibleState(_skillArray);
    }

    /// <summary> 해당 인덱스 습득 여부 확인후 활성화 </summary>
    public void UpdateAccessibleAcquiredState(int index)
    {
        //스킬 제한 레벨이 더 높다면 스킬을 습득할 수 없다.
        if (!ComparePlayerLevelToSkillLevel(index)) return;

        _skInvenUI.SetItemAccessibleState(_skillArray[index], index);
    }

    public void SkillLevelUP(int index)
    {
        //스킬 제한 레벨이 더 높다면 스킬 레벨업 x
        if (!ComparePlayerLevelToSkillLevel(index)) return;
        // 스킬 레벨 1이라면 레벨업 x
        if (_skillArray[index].GetSkillLevel() == 1) return;
        _skillArray[index].LevelUP();
    }

    /// <summary> 모든 슬롯 UI에 접근 가능 여부 업데이트 </summary>
    private void UpdateAccessibleStatesAll()
    {
        
        _skInvenUI.SetAccessibleSlotRange(_maxSkillSize);
    }

    /// <summary> 플레이어 레벨과 스킬의 습득 레벨 비교 </summary>
    private bool ComparePlayerLevelToSkillLevel(int index)
    {
        return _playerCtr.GetLevel() >= _skillArray[index].GetSkillUsedLevel();
    }

    /// <summary> 해당 슬롯이 스킬을 갖고 있는지 여부 </summary>
    public bool HasSkill(int index)
    {
        return IsValidIndex(index) && _skillArray[index] != null;
    }


    /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (_skillArray[index] == null) return "";

        return _skillArray[index].GetSKillName();
    }

    public void FromInvenToPlayer(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        SkillData skillA = _skillArray[indexA];

        _playerSkillArray[indexB] = skillA;

        // 두 슬롯 정보 갱신
        UpdatePlayerSlot(indexB);
    }


    public void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        SkillData skillA = _playerSkillArray[indexA];
        SkillData skillB = _playerSkillArray[indexB];

        _playerSkillArray[indexA] = skillB;
        _playerSkillArray[indexB] = skillA;

        // 두 슬롯 정보 갱신
        UpdatePlayerSlot(indexA, indexB);
    }

    /// <summary> 모든 슬롯들의 상태를 UI에 갱신 </summary>
    private void UpdateAllSlot()
    {
        for (int i = 0; i < _maxSkillSize; i++)
        {
            UpdateSlot(i);
        }
    }

    /// <summary> 해당하는 인덱스의 슬롯들의 상태 및 UI 갱신 </summary>
    private void UpdateSlot(params int[] indices)
    {
        foreach (var i in indices)
        {
            UpdateSlot(i);
        }
    }

    /// <summary> 해당하는 인덱스의 플레이어 슬롯 상태 및 UI 갱신 </summary>
    private void UpdatePlayerSlot(params int[] indices)
    {
        foreach (var i in indices)
        {
            UpdatePlayerSlot(i);
        }
    }

    public void UpdatePlayerSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        SkillData skData = _playerSkillArray[index];

        // 1. 아이템이 슬롯에 존재하는 경우
        if (skData != null)
        {
            //아이콘 등록
            _skInvenUI.SetPlayerSkill_ItemIcon(index, skData.GetSkill_Sprite());

        }
        //2. 빈 슬롯인 경우 : 아이콘 제거
        else
        {
            RemoveIcon();
        }

        // 로컬 : 아이콘 제거하기
        void RemoveIcon()
        {
            _skInvenUI.RemovePlayerItem(index);
        }
    }


    /// <summary> 해당하는 인덱스의 슬롯 상태 및 UI 갱신 </summary>
    public void UpdateSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        SkillData skData = _skillArray[index];

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

        _playerSkillArray[index] = null;
        UpdatePlayerSlot(index);
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
        _playerCtr.SetUseInven(value); //플레이어의 움직임 제어
        _skInvenUI.gameObject.SetActive(value);
        
        _playerCtr.GetCameraCtr().UseWindow(value); //카메라의 회전 제어

        if (value) GameManager.instance.VisibleCursor();
        else
            GameManager.instance.InvisibleCursor();
    }



    /// <summary> Load Data 없을시 초기화 </summary>
    public void Init_Skills()
    { 
        _skillArray = SaveSys.LoadSkillSet("AllSkillSetData.Json");
        _playerSkillArray = new SkillData[_maxSkillSize];

        if (_skillArray == null)
        {
            _skillArray = new SkillData[_maxSkillSize];
            return;
        }
        else
        {
            for (int i = 0; i < _skillArray.Length; i++)
            {
                if (_skillArray[i] == null) continue;

                _skillArray[i].Init();                
                //_skills[i].SetSkillDamage(power_STR);
            }
        }
    }

    public void UseSkill(SkillData _skilldata, ObjectBase _selfCtr, ref int _objectMP)
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
        foreach (SkillData skill in _skillArray)
        {
            if (skill == null) continue;
            skill.SetSkillDamage(power_STR);
        }
    }

    /// <summary> PlayerController Awake()에서 호출 </summary>
    public Skill_InvenUI GetSkInvenUI()
    {
        return _skInvenUI;
    }

    /// <summary> 플레이어에게 스킬 초기화 해서 전달. </summary>
    private void HandOverPlayerSkills()
    {
        _playerCtr.SetPlayerSkill(_playerSkillArray);
    }
}
