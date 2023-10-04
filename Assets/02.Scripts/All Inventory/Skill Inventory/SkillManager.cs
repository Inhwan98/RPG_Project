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


    /// <summary> PlayerController Awake()에서 호출 </summary>
    public Skill_InvenUI GetSkInvenUI() => _skInvenUI;

    public void SetPlayerCtr(PlayerController player) => _playerCtr = player;
    public void SetSkillPower(float value) => power_STR = value;
    
    private void Awake()
    {
        _skInvenUI = FindObjectOfType<Skill_InvenUI>();
        _skInvenUI.SetSkillTreeReference(this);

        //Init_Skills(); //GameManager에서 실행
        
    }

    private void Start()
    {
        SetSkillDataDamage();
        SetLevelText();
        HandOverPlayerSkills();

        UpdateAllSlot();
        UpdateAccessibleStatesAll();
        UpdateAccessibleAcquiredState();
    }

    /// <summary> 스킬 인벤의 데이터</summary>
    public SkillData GetInvenSkillData(int idx)
    {
        if (!IsValidIndex(idx)) return null;
        if (_skillArray[idx] == null) return null;

        return _skillArray[idx];
    }

    /// <summary> 플레이어가 습득한 스킬 데이터</summary>
    public SkillData GetPlayerSkillData(int idx)
    {
        if (!IsValidIndex(idx)) return null;
        if (_playerSkillArray[idx] == null) return null;

        return _playerSkillArray[idx];
    }


    /// <summary> 스킬 인벤의 모든 데이터</summary>
    public SkillData[] GetInvenAllSkillData()
    {
        if (_skillArray == null)
        {
            Debug.LogError("Skill Data NULL!!!");
            return null;
        }
        return _skillArray;
    }


    /// <summary> 플레이어가 습득한 모든 스킬 데이터</summary>
    public SkillData[] GetPlayerAllSkillData()
    {
        if (_playerSkillArray == null) return null;

        return _playerSkillArray;
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

    /// <summary> 스킬인벤 "+" 버튼 작용 - 해당 인덱스 습득 여부 확인후 활성화 </summary>
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
        // 스킬을 배우지 않았다면 리턴
        if (!_skillArray[index].GetIsAcquired()) return;
        
        _skillArray[index].LevelUP();
        _skillArray[index].SetSkillDamage(power_STR);
        SetLevelText(index);
    }

    public void SetLevelText(int index)
    {
        _skInvenUI.SetLevelText(_skillArray[index], index);
    }

    public void SetLevelText()
    {
        _skInvenUI.SetLevelText(_skillArray);
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

    /// <summary>
    /// Drop and drop으로 플레이어가 습득할 스킬Slot으로 이동 시킨다.
    /// SKILL Inven => Player SKILL Inven
    /// </summary>
    public void FromInvenToPlayer(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        SkillData skillA = _skillArray[indexA];

        _playerSkillArray[indexB] = skillA;

        // 두 슬롯 정보 갱신
        UpdatePlayerSlot(indexB);
    }

    /// <summary>
    /// swap은 skill Inven에서는 x
    /// Player Skill Inven에서만 이루어진다.
    /// </summary>
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
            UpdatePlayerSlot(i);
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

    /// <summary> Player Skill Slot을 이미지 업데이트 한다. </summary>
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





    /// <summary> Inventory 활성화 유무 (마우스 커서도 같이 활성화) </summary>
    public void SetSkillWindowActive(bool value)
    {
        _playerCtr.SetUseItemInven(value); //플레이어의 움직임 제어
        _skInvenUI.gameObject.SetActive(value);
        
        _playerCtr.GetCameraCtr().UseWindow(value); //카메라의 회전 제어

        if (value) GameManager.instance.VisibleCursor();
        else
            GameManager.instance.InvisibleCursor();
    }



    /// <summary> Load Data 없을시 초기화 </summary>
    public void Init_Skills(SkillData[] skillArray, SkillData[] playerSkillArray)
    {
        _skillArray = skillArray;


        _playerSkillArray = playerSkillArray;


        for (int i = 0; i < _skillArray.Length; i++)
        {
            if (_skillArray[i] == null) continue;
            _skillArray[i].Init();
        }
        
        //플레이어 스킬 슬롯에 아무것도 없다면 새로 초기화
        if(_playerSkillArray.Length == 0)
        {
            _playerSkillArray = new SkillData[_maxSkillSize];
        }
        else
        {
            //플레이어 스킬 슬롯에 스킬이 존재한다면
            for (int i = 0; i < _skillArray.Length; i++)
            {
                for (int j = 0; j < _playerSkillArray.Length; j++)
                {
                    if (_playerSkillArray[j] == null) continue;

                    if (_skillArray[i].GetID() == _playerSkillArray[j].GetID())
                    {
                        _playerSkillArray[j] = _skillArray[i];
                    }
                }
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
        power_STR = _playerCtr.GetCurStr();
        for(int i = 0; i < _maxSkillSize; i++)
        {
            _skillArray[i].SetSkillDamage(power_STR);

            if (_playerSkillArray[i] == null) continue;
            _playerSkillArray[i].SetSkillDamage(power_STR);
        }
        
        //foreach (SkillData skill in _skillArray)
        //{
        //    if (skill == null) continue;
        //    skill.SetSkillDamage(power_STR);
        //}
    }

    public void StartSkillCoolTime(int skill_Idx, SkillData curSkill)
    {
        StartCoroutine(_skInvenUI.StartSkillCoolTime(skill_Idx, curSkill));
    }

    public void SkillUsedWarring(int skill_Idx)
    {
        StartCoroutine(_skInvenUI.SkillUsedWarring(skill_Idx));
    }



    /// <summary> 플레이어에게 스킬 초기화 해서 전달. </summary>
    private void HandOverPlayerSkills()
    {
        _playerCtr.SetPlayerSkill(_playerSkillArray);
    }



}
