using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillTreeUI : StateInfoUI
{

    /// <summary> 연결된 인벤토리 </summary>
    private SkillManager _skillManager;

    protected override void Start()
    {
        base.Start();
        
        for(int i = 0; i < _slotUIList.Count; i++)
        {
            _slotUIList[i].SetSlotIndex(i);
        }
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
        if (!IsOverUI())
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


 
}
