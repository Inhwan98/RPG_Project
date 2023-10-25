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
    /// <summary> �κ��丮 ���� ��� (�κ��丮���� ���� ȣ��) </summary>
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

    /// <summary> ���콺 Ŭ�� �̺�Ʈ </summary>
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
                //�������κ� => �������κ�
                TryMoveFromItemInvenToQuick(_beginDragSlot, endDragSlot);
            }
            else if(_beginDragSlot is QuickSlotUI && endDragSlot is ItemInvenSlotUI)
            {
                //������ => ������ �κ�
                TryMoveFromQuickToItemInven(_beginDragSlot, endDragSlot);
            }
            else if(_beginDragSlot is QuickSlotUI && endDragSlot is QuickSlotUI)
            {
                TrySwapItems(_beginDragSlot, endDragSlot);
            }
            

            // ���� ����
            UpdateTooltipUI(endDragSlot);
            return;
        }

        // ������(Ŀ���� UI ����ĳ��Ʈ Ÿ�� ���� ���� ���� ���
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

    /// <summary> UI �� �κ��丮���� ������ ���� </summary>
    protected override void TryRemoveItem(int index)
    {
        _quickInvenMgr.Remove(index);
    }
    /// <summary> �� ������ ������ ��ȯ </summary>
    protected override void TrySwapItems(SlotUIBase from, SlotUIBase to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to);
        _quickInvenMgr.Swap(from.GetIndex(), to.GetIndex());
    }
    /// <summary> ���� UI�� ���� ������ ���� </summary>
    protected override void UpdateTooltipUI(SlotUIBase slot)
    {
        if (!slot.GetIsAccessible() || !slot.GetHasItem())
            return;

        // ���� ���� ����
        _itemTooltip.SetItemInfo(_quickInvenMgr.GetItemData(slot.GetIndex()));

        // ���� ��ġ ����
        _itemTooltip.SetRectPosition(slot.GetSlotRect());
    }
    #endregion

    #region private Methods

    /// <summary> ������ ��� </summary>
    private void TryUseItem(int index)
    {
        _quickInvenMgr.Use(index);
    }

    /// <summary> �� -> ������ �κ� </summary>
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
    /// <summary> �ش� ������ ������ ���� �ؽ�Ʈ ���� </summary>
    public void SetItemAmountText(int index, int amount)
    {
        //amount�� 1 ������ ��� �ؽ�Ʈ ��ǥ��
        if (_slotUIList[index] is QuickSlotUI slotUI)
        {
            slotUI.SetItemAmount(amount);
        }

    }
    /// <summary> �ش� ������ ������ ���� �ؽ�Ʈ ���� </summary>
    public void HideItemAmountText(int index)
    {
        if (_slotUIList[index] is QuickSlotUI slotUI)
        {
            slotUI.SetItemAmount(1);
        }
    }

    #endregion

}
