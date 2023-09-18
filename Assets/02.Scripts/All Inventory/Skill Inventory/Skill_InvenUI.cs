using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Skill_InvenUI : InvenUIBase
{
    [SerializeField] private GameObject _playerSkillGo;
    protected GraphicRaycaster _playerGr;

    [SerializeField]
    protected List<SlotUIBase> _playerSlotUIList = new List<SlotUIBase>(); // 아이템 슬롯 리스트

    /// <summary> 연결된 인벤토리 </summary>
    private SkillManager _skillManager;

    

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < _slotUIList.Count; i++)
        {
            _slotUIList[i].SetSlotIndex(i);
            _playerSlotUIList[i].SetSlotIndex(i);
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
        if (!slot.GetIsAccessible() || !slot.GetHasItem())
            return;

        // 툴팁 정보 갱신
        _itemTooltip.SetItemInfo(_skillManager.GetSKillData(slot.GetIndex()));

        // 툴팁 위치 조정
        _itemTooltip.SetRectPosition(slot.GetSlotRect());
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
    public void SetPlayerItemIcon(int index, Sprite icon)
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
            // 수량 나누기 조건
            // 1) 마우스 클릭 떼는 순간 좌측 Ctrl 또는 Shift 키 유지
            // 2) begin : 셀 수 있는 아이템 / end : 비어있는 슬롯
            // 3) begin 아이템의 수량 > 1

           TrySwapItems(_beginDragSlot, endDragSlot);
            // 툴팁 갱신
            UpdateTooltipUI(endDragSlot);
            return;
        }

        // 버리기(커서가 UI 레이캐스트 타겟 위에 있지 않은 경우
        //if (!IsOverUI())
        //{
        //    // 확인 팝업 띄우고 콜백 위임
        //    int index = _beginDragSlot.GetIndex();

        //    TryRemoveItem(index);
        //}
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


 
}
