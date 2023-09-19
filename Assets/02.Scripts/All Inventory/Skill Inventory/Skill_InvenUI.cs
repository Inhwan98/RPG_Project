using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System;

public class Skill_InvenUI : InvenUIBase
{
    [SerializeField] private GameObject _playerSkillGo;
    private GraphicRaycaster _playerGr;

    [SerializeField]
    private List<PlayerSkillSlotUI> _playerSlotUIList = new List<PlayerSkillSlotUI>(); // 아이템 슬롯 리스트

    private Button[] plusButton;

    /// <summary> 연결된 인벤토리 </summary>
    private SkillManager _skillManager;


    protected override void Awake()
    {
        base.Awake();

        int slotSize = _slotUIList.Count;

        plusButton = new Button[slotSize];

        for (int i = 0; i < slotSize; i++)
        {
            _slotUIList[i].SetSlotIndex(i);
            _playerSlotUIList[i].SetSlotIndex(i);

            var skillSlot = _slotUIList[i] as InvenSkillSlotUI;
            int index = skillSlot.GetIndex();

            //스킬슬롯의 버튼 변수 할당 (같은 고유 인덱스를 가진다)
            plusButton[i] = skillSlot.GetPlusButton();
            plusButton[i].onClick.AddListener(() => _skillManager.UpdateAccessibleAcquiredState(index));
            plusButton[i].onClick.AddListener(() => _skillManager.SkillLevelUP(index));

        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Init()
    {
        _playerSkillGo.TryGetComponent(out _playerGr);
        if (_playerGr == null)
            _playerSkillGo.AddComponent<GraphicRaycaster>();
        base.Init();
    }

    public void RemovePlayerItem(int index)
    {
        _playerSlotUIList[index].RemoveItem();
    }

    protected override T RaycastAndGetFirstComponent<T>()
    {
        _rrList.Clear();

        _playerGr.Raycast(_ped, _rrList);
        _gr.Raycast(_ped, _rrList);

        if (_rrList.Count == 0)
            return null;
        return _rrList[0].gameObject.GetComponent<T>();
    }

    /// <summary> 툴팁 UI의 슬롯 데이터 갱신 </summary>
    protected override void UpdateTooltipUI(SlotUIBase slot)
    {
        //09.19
        //if (!slot.GetIsAccessible() || !slot.GetHasItem())
        //    return;

        if (!slot.GetHasItem())
            return;

        // 툴팁 정보 갱신
        if (slot is InvenSkillSlotUI) //스킬인벤에 있는 데이터라면
            _itemTooltip.SetItemInfo(_skillManager.GetInvenSKillData(slot.GetIndex()));
        else ////플레이어 스킬인벤에 있는 데이터라면
            _itemTooltip.SetItemInfo(_skillManager.GetPlayerSKillData(slot.GetIndex()));

        // 툴팁 위치 조정
        _itemTooltip.SetRectPosition(slot.GetSlotRect());
    }

    public void SetItemAccessibleState(SkillData[] datas)
    {
        
        for(int i = 0; i < _slotUIList.Count; i++)
        {
            bool isAcquired = datas[i].GetIsAcquired();

            if (_slotUIList[i] is SKillSlotUI skSlotUIk)
            {
                skSlotUIk.SetItemAccessibleState(isAcquired);
            }
        }
    }

    public void SetItemAccessibleState(SkillData data, int index)
    {
        data.SetIsAcquired(true);
        bool isAcquired = data.GetIsAcquired();

        if (_slotUIList[index] is SKillSlotUI skSlotUIk)
        {
            skSlotUIk.SetItemAccessibleState(isAcquired);
        }
    }


    public override void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < _slotUIList.Count; i++)
        {
            _playerSlotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }

        base.SetAccessibleSlotRange(accessibleSlotCount);
    }

    /// <summary> 슬롯에 플레이어 스킬 아이콘 등록 </summary>
    public void SetPlayerSkill_ItemIcon(int index, Sprite icon)
    {
        _playerSlotUIList[index].SetItem(icon);
    }

    /// <summary> 스킬매니져 참조 등록 (스킬매니져 직접 호출) </summary>
    public void SetSkillTreeReference(SkillManager skillMgr)
    {
        _skillManager = skillMgr;
    }

    protected override void EndDrag()
    {
        SlotUIBase endDragSlot = RaycastAndGetFirstComponent<SlotUIBase>();
        if (endDragSlot != null && endDragSlot.GetIsAccessible())
        {
            if (_beginDragSlot is InvenSkillSlotUI && endDragSlot is PlayerSkillSlotUI)
            {
                TryMoveItems(_beginDragSlot, endDragSlot);
                // 툴팁 갱신
                UpdateTooltipUI(endDragSlot);
                return;
            }
            else if(_beginDragSlot is PlayerSkillSlotUI && endDragSlot is PlayerSkillSlotUI)
            {
                TrySwapItems(_beginDragSlot, endDragSlot);
                // 툴팁 갱신
                UpdateTooltipUI(endDragSlot);
                return;
            }
        }

        //버리기(커서가 UI 레이캐스트 타겟 위에 있지 않은 경우
        //플레이어 스킬UI 인 경우
        if (_beginDragSlot is PlayerSkillSlotUI && !IsOverUI())
        {
            // 확인 팝업 띄우고 콜백 위임
            int index = _beginDragSlot.GetIndex();

            TryRemoveItem(index);
        }
    }

    /// <summary> UI 및 인벤토리에서 아이템 제거 </summary>
    protected override void TryRemoveItem(int index)
    {
        _skillManager.Remove(index);
    }

    /// <summary> 두 슬롯의 아이템 교환 </summary>
    protected override void TrySwapItems(SlotUIBase from, SlotUIBase to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to);
        _skillManager.Swap(from.GetIndex(), to.GetIndex());
    }

    /// <summary> 스킬인벤 -> 플레이어 스킬인벤 이동 </summary>
    public void TryMoveItems(SlotUIBase from, SlotUIBase to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to, true);
        _skillManager.FromInvenToPlayer(from.GetIndex(), to.GetIndex());
    }


    // *************************************
    // ******   스킬 사용시 이벤트      ****
    // ****** PlayerController에서 사용 ****
    // **************************************

    /// <summary> 쿨타임 진행 때 사용시 적색 불 들어온다 </summary>
    public IEnumerator SkillUsedWarring(int _idx)
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.05f);

        Image curIcon = _playerSlotUIList[_idx].GetIconImage();

        for (int i = 0; i < 4; i++)
        {
            if (i % 2 == 0) curIcon.color = Color.red;
            
            else curIcon.color = Color.white;
            yield return waitTime;
        }

        yield break;
    }


    /// <summary> 스킬의 쿨타임을 시각적인 이미지, 텍스트로 나타낸다 </summary>
    public IEnumerator StartSkillCoolTime(int _idx, SkillData _curSkill)
    {
        _curSkill.SetInUse(true); // 현재 스킬은 사용 중으로 bool 값 상태

        Image coolTimeImage = _playerSlotUIList[_idx].GetCoolTimeImage();
        TMP_Text coolTimeText = _playerSlotUIList[_idx].GetCoolTimeText();

        coolTimeImage.gameObject.SetActive(true);
        coolTimeImage.fillAmount = 1;

        float fcoolTime     = _curSkill.GetCoolDown();
        float fcoolTimeText = _curSkill.GetCoolDown();
        float t = 0.0f;

        while (0.0f < fcoolTimeText)
        {
            fcoolTimeText -= Time.deltaTime;
            t = Time.deltaTime / fcoolTime;
            coolTimeImage.fillAmount -= t;

            coolTimeText.text = $"{(int)fcoolTimeText}";
            yield return null;
        }
        //쿨타임이 끝나면 Fill 이미지와 CoolTime Text 비활성화
        //스킬 사용 가능 상태로 변환
        coolTimeImage.gameObject.SetActive(false);
        _curSkill.SetInUse(false);
    }

}
