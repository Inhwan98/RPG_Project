using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerEquipUI : InvenUIBase
{
    private PlayerStatManager _playerUIMgr;
    [SerializeField] private GameObject _itemInvenGo;
    private GraphicRaycaster _itemInvenGr;

    public void SetPlayerUIMgr(PlayerStatManager playerUIManager) => this._playerUIMgr = playerUIManager;

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
            PlayerEquipSlotUI slot = RaycastAndGetFirstComponent<PlayerEquipSlotUI>();

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
            if (_beginDragSlot is PlayerEquipSlotUI && endDragSlot is PlayerEquipSlotUI) return;

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

        if(from is PlayerEquipSlotUI)
        {
            _playerUIMgr.Swap(from.GetIndex(), to.GetIndex());
        }
        else
        {
            _playerUIMgr.ReverseSwap(from.GetIndex(), to.GetIndex());
        }  
    }

    protected override void ShowOrHideItemToolTip()
    {
        // 마우스가 유효한 아이템 아이콘 위에 올라와 있다면 툴팁 보여주기
        bool isValid =                                                
            _pointerOverSlot != null && _pointerOverSlot.GetHasItem() 
            && (_pointerOverSlot != _beginDragSlot) && _pointerOverSlot is PlayerEquipSlotUI; //장비슬롯이 아니면 보여주지 않기

        if (isValid)
        {
            UpdateTooltipUI(_pointerOverSlot);
            _itemTooltip.Show();
        }
        else
            _itemTooltip.Hide();
    }


    protected override void UpdateTooltipUI(SlotUIBase slot)
    {
        if (!slot.GetIsAccessible() || !slot.GetHasItem())
            return;

        // 툴팁 정보 갱신
        _itemTooltip.SetItemInfo(_playerUIMgr.GetItemData(slot.GetIndex()));

        // 툴팁 위치 조정
        _itemTooltip.SetRectPosition(slot.GetSlotRect());
    }

}
