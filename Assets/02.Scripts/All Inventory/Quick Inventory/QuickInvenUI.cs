using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickInvenUI : InvenUIBase
{
    /******************************************************
     *                       Fields
     ******************************************************/
    [SerializeField] private GameObject _itemInvenGo;
    private GraphicRaycaster _itemInvenGr;
    private QuickInvenManager _quickInvenMgr;



    /****************************************
    *              Get Set Methods
    ****************************************/

    #region Set Methods
    /// <summary> 인벤토리 참조 등록 (인벤토리에서 직접 호출) </summary>
    public void SetInventoryReference(QuickInvenManager quickInvenManager)
    {
        _quickInvenMgr = quickInvenManager;
    }
    #endregion

    protected override void Start()
    {
        
    }



    /******************************************************
    *                      Methods
    ******************************************************/
    #region override Methods
    protected override void Init()
    {
        int slotSize = _slotUIList.Count;

        for (int i = 0; i < slotSize; i++)
        {
            _slotUIList[i].SetSlotIndex(i);
        }

        _itemInvenGo.TryGetComponent(out _itemInvenGr);
        if (_itemInvenGr == null)
            _itemInvenGr = _itemInvenGo.AddComponent<GraphicRaycaster>();
        base.Init();
    }

    /// <summary> 마우스 클릭 이벤트 </summary>
    protected override void OnPointerDown()
    {
        base.OnPointerDown();
        if (Input.GetMouseButtonDown(1))
        {
            QuickSlotUI slot = RaycastAndGetFirstComponent<QuickSlotUI>();

            if (slot != null && slot.GetHasItem() && slot.GetIsAccessible())
            {
                TryUseItem(slot.GetIndex());
            }
        }
    }
    protected override void EndDrag()
    {
        SlotUIBase endDragSlot = RaycastAndGetFirstComponent<SlotUIBase>();

        if (endDragSlot != null && endDragSlot.GetIsAccessible())
        {
            if(_beginDragSlot is ItemInvenSlotUI && endDragSlot is QuickSlotUI)
            {
                //아이템인벤 => 퀵슬롯인벤
                TryMoveFromItemInvenToQuick(_beginDragSlot, endDragSlot);
            }
            else if(_beginDragSlot is QuickSlotUI && endDragSlot is ItemInvenSlotUI)
            {
                //퀵슬롯 => 아이템 인벤
                TryMoveFromQuickToItemInven(_beginDragSlot, endDragSlot);
            }
            else if(_beginDragSlot is QuickSlotUI && endDragSlot is QuickSlotUI)
            {
                TrySwapItems(_beginDragSlot, endDragSlot);
            }
            

            // 툴팁 갱신
            UpdateTooltipUI(endDragSlot);
            return;
        }

        // 버리기(커서가 UI 레이캐스트 타겟 위에 있지 않은 경우
        if (!IsOverUI())
        {
         
        }
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

    /// <summary> UI 및 인벤토리에서 아이템 제거 </summary>
    protected override void TryRemoveItem(int index)
    {
        _quickInvenMgr.Remove(index);
    }
    /// <summary> 두 슬롯의 아이템 교환 </summary>
    protected override void TrySwapItems(SlotUIBase from, SlotUIBase to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to);
        _quickInvenMgr.Swap(from.GetIndex(), to.GetIndex());
    }
    /// <summary> 툴팁 UI의 슬롯 데이터 갱신 </summary>
    protected override void UpdateTooltipUI(SlotUIBase slot)
    {
        if (!slot.GetIsAccessible() || !slot.GetHasItem())
            return;

        // 툴팁 정보 갱신
        _itemTooltip.SetItemInfo(_quickInvenMgr.GetItemData(slot.GetIndex()));

        // 툴팁 위치 조정
        _itemTooltip.SetRectPosition(slot.GetSlotRect());
    }
    #endregion

    #region private Methods

    /// <summary> 아이템 사용 </summary>
    private void TryUseItem(int index)
    {
        _quickInvenMgr.Use(index);
    }

    /// <summary> 퀵 -> 아이템 인벤 </summary>
    private void TryMoveFromQuickToItemInven(SlotUIBase from, SlotUIBase to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to);
        _quickInvenMgr.MoveFromQuickToItemInven(from.GetIndex(), to.GetIndex());
    }

    private void TryMoveFromItemInvenToQuick(SlotUIBase from, SlotUIBase to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to);
        _quickInvenMgr.MoveFromItemInvenToQuick(from.GetIndex(), to.GetIndex());
    }


    #endregion
    #region public Methods
    /// <summary> 해당 슬롯의 아이템 개수 텍스트 지정 </summary>
    public void SetItemAmountText(int index, int amount)
    {
        //amount가 1 이하일 경우 텍스트 미표시
        if (_slotUIList[index] is QuickSlotUI slotUI)
        {
            slotUI.SetItemAmount(amount);
        }

    }
    /// <summary> 해당 슬롯의 아이템 개수 텍스트 지정 </summary>
    public void HideItemAmountText(int index)
    {
        if (_slotUIList[index] is QuickSlotUI slotUI)
        {
            slotUI.SetItemAmount(1);
        }
    }

    #endregion

}
