using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerEquipUI : InvenUIBase
{
    private PlayerUIManager _playerUIMgr;
    [SerializeField] private GameObject _itemInvenGo;
    private GraphicRaycaster _itemInvenGr;

    public void SetPlayerUIMgr(PlayerUIManager playerUIManager) => this._playerUIMgr = playerUIManager;

    protected override void Awake()
    {
        base.Awake();

        int slotSize = _slotUIList.Count;

        for(int i = 0; i < slotSize; i++)
        {
            _slotUIList[i].SetSlotIndex(i);
        }
    }


    protected override void Init()
    {
        _itemInvenGo.TryGetComponent(out _itemInvenGr);
        if (_itemInvenGr == null)
            _itemInvenGr = _itemInvenGo.AddComponent<GraphicRaycaster>();
        base.Init();
    }


    protected override T RaycastAndGetFirstComponent<T>()
    {
        _rrList.Clear();

        _gr.Raycast(_ped, _rrList);
        _itemInvenGr.Raycast(_ped, _rrList);

        if (_rrList.Count == 0)
            return null;
        return _rrList[0].gameObject.GetComponent<T>();
    }

    protected override void OnPointerDown()
    {
        base.OnPointerDown();
        if (Input.GetMouseButtonDown(1))
        {
            PlayerSlotUI slot = RaycastAndGetFirstComponent<PlayerSlotUI>();

            if (slot != null && slot.GetHasItem() && slot.GetIsAccessible())
            {
                int index = slot.GetIndex();

                //추가 후 삭제
                TryAddItemInven(index);
                TryRemoveItem(index);
            }
        }
    }

    protected override void EndDrag()
    {
        SlotUIBase endDragSlot = RaycastAndGetFirstComponent<SlotUIBase>();

        if (endDragSlot != null && endDragSlot.GetIsAccessible())
        {
            if (_beginDragSlot is ItemInvenSlotUI && endDragSlot is ItemInvenSlotUI) return;
            if (_beginDragSlot is PlayerSlotUI && endDragSlot is PlayerSlotUI) return;
            TrySwapItems(_beginDragSlot, endDragSlot);

            // 툴팁 갱신
            UpdateTooltipUI(endDragSlot);
            return;
        }

        // 버리기(커서가 UI 레이캐스트 타겟 위에 있지 않은 경우
        if (!IsOverUI())
        {
            int index = _beginDragSlot.GetIndex();

            //추가 후 삭제
            TryAddItemInven(index);
            TryRemoveItem(index);
        }
    }

    private void TryAddItemInven(int index) => _playerUIMgr.AddItemInven(index);
    protected override void TryRemoveItem(int index) => _playerUIMgr.Remove(index);

    /// <summary> 두 슬롯의 아이템 교환 </summary>
    protected override void TrySwapItems(SlotUIBase from, SlotUIBase to)
    {
        if (from == to) return;
        //from.SwapOnMoveIcon(to);

        if(from is PlayerSlotUI)
        {
            _playerUIMgr.Swap(from.GetIndex(), to.GetIndex());
        }
        else
        {
            _playerUIMgr.ReverseSwap(from.GetIndex(), to.GetIndex());
        }  
    }

    protected override void UpdateTooltipUI(SlotUIBase slot)
    {
        if (!slot.GetIsAccessible() || !slot.GetHasItem())
            return;

        // 툴팁 정보 갱신
        //_itemTooltip.SetItemInfo(_itemInventory.GetItemData(slot.GetIndex()));

        // 툴팁 위치 조정
        _itemTooltip.SetRectPosition(slot.GetSlotRect());
    }

}
