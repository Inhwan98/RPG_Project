using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryUI : StateInfoUI
{
    [Header("Options")]
    [Range(0, 10)]
    [SerializeField] private int _horizontalSlotCount = 8;    // ���� ���� ī��Ʈ
    [Range(0, 10)]
    [SerializeField] private int _verticalSlotCount = 8;      // ���� ���� ����
    [SerializeField] private float _slotMargin = 8f;            // �� ������ ��ȭ�¿� ����
    [SerializeField] private float _contentAreaPadding = 20.0f; // �κ��丮 ������ ���� ����

    [Range(32, 64)]
    [SerializeField] private float _slotSize = 64f;             // �� ������ ũ��

    [Space(16)]
    [SerializeField] private bool _mouseReversed = false; // ���콺 Ŭ�� ���� ����

    [Header("Connected Objects")]
    [SerializeField] private RectTransform _contentAreaRT;       // ���Ե��� ��ġ�� ����
    [SerializeField] private GameObject _slotUiPrefab;           // ������ ���� ������
    [SerializeField] private InventoryPopupUI _popup;            // �˾� UI ���� ��ü

    [SerializeField] private Button _sortButton;
    [SerializeField] private Button _exitButton;

    /// <summary> ����� �κ��丮 </summary>
    private Inventory _inventory;


    protected override void Awake()
    {
        InitSlots();
        _sortButton.onClick.AddListener(() => _inventory.SortAll());
        _exitButton.onClick.AddListener(() => _inventory.InventoryActive(false));

        base.Awake();
    }


    /// <summary> ���� UI�� ���� ������ ���� </summary>
    protected override void UpdateTooltipUI(SlotUIBase slot)
    {
        if (!slot.GetIsAccessible() || !slot.GetHasItem())
            return;

        // ���� ���� ����
        _itemTooltip.SetItemInfo(_inventory.GetItemData(slot.GetIndex()));

        // ���� ��ġ ����
        _itemTooltip.SetRectPosition(slot.GetSlotRect());
    }


    /// <summary> �κ��丮 ���� ��� (�κ��丮���� ���� ȣ��) </summary>
    public void SetInventoryReference(Inventory inventory)
    {
        _inventory = inventory;
    }


    /// <summary> ������ ��� </summary>
    private void TryUseItem(int index)
    {
        _inventory.Use(index);
    }

    protected override void OnPointerDown()
    {
        base.OnPointerDown();
        if (Input.GetMouseButtonDown(1))
        {
            ItemSlotUI slot = RaycastAndGetFirstComponent<ItemSlotUI>();

            if (slot != null && slot.GetHasItem() && slot.GetIsAccessible())
            {
                TryUseItem(slot.GetIndex());
            }
        }
    }


    protected override void EndDrag()
    {
        ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        if (endDragSlot != null && endDragSlot.GetIsAccessible())
        {
            // ���� ������ ����
            // 1) ���콺 Ŭ�� ���� ���� ���� Ctrl �Ǵ� Shift Ű ����
            // 2) begin : �� �� �ִ� ������ / end : ����ִ� ����
            // 3) begin �������� ���� > 1

            bool isSepartable =
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) &&
                (_inventory.IsCountableItem(_beginDragSlot.GetIndex()) && !_inventory.HasItem(endDragSlot.GetIndex()));

            //true : ���� ������, false : ��ȯ �Ǵ� �̵�
            bool isSeparation = false;
            int currentAmount = 0;

            // ���� ���� Ȯ��
            if (isSepartable)
            {
                currentAmount = _inventory.GetCurrentAmount(_beginDragSlot.GetIndex());
                if (currentAmount > 1)
                {
                    isSeparation = true;
                }
            }

            // 1. ���� ������
            if (isSeparation)
                TrySeparateAmount(_beginDragSlot.GetIndex(), endDragSlot.GetIndex(), currentAmount);
            // 2. ��ȯ �Ǵ� �̵�
            else
                TrySwapItems(_beginDragSlot, endDragSlot);

            // ���� ����
            UpdateTooltipUI(endDragSlot);
            return;
        }

        // ������(Ŀ���� UI ����ĳ��Ʈ Ÿ�� ���� ���� ���� ���
        if (!IsOverUI())
        {
            // Ȯ�� �˾� ���� �ݹ� ����
            int index = _beginDragSlot.GetIndex();
            string itemName = _inventory.GetItemName(index);
            int amount = _inventory.GetCurrentAmount(index);

            // �� �� �ִ� �������� ���, ���� ǥ��
            if (amount > 1)
                itemName += $" x{amount}";

            // Ȯ�� �˾� ���� �ݹ� ����
            _popup.OpenConfirmationPopup(() => TryRemoveItem(index), itemName);
        }
    }

    /// <summary> UI �� �κ��丮���� ������ ���� </summary>
    protected override void TryRemoveItem(int index)
    {
        _inventory.Remove(index);
    }

    /// <summary> �� �� �ִ� ������ ���� ������ </summary>
    private void TrySeparateAmount(int indexA, int indexB, int amount)
    {
        if (indexA == indexB)
        {
            return;
        }
        string itemName = _inventory.GetItemName(indexA);

        _popup.OpenAmountInputPopup(
            amt => _inventory.SeparateAmount(indexA, indexB, amt),
            amount, itemName
        );
    }

    /// <summary> �ش� ������ ������ ���� �ؽ�Ʈ ���� </summary>
    public void SetItemAmountText(int index, int amount)
    {
        //amount�� 1 ������ ��� �ؽ�Ʈ ��ǥ��
        if(_slotUIList[index] is ItemSlotUI slotUI)
        {
            slotUI.SetItemAmount(amount);
        }
        
    }

    /// <summary> �ش� ������ ������ ���� �ؽ�Ʈ ���� </summary>
    internal void HideItemAmountText(int index)
    {
        if (_slotUIList[index] is ItemSlotUI slotUI)
        {
            slotUI.SetItemAmount(1);
        }
    }



    /// <summary> �� ������ ������ ��ȯ </summary>
    protected override void TrySwapItems(SlotUIBase from, SlotUIBase to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to);
        _inventory.Swap(from.GetIndex(), to.GetIndex());
    }


    /// <summary>
    /// ������ ������ŭ ���� ���� ���� ���Ե� ���� ����
    /// </summary>
    private void InitSlots()
    {
        Debug.Log("InitSlots");
        // ���� ������ ����
        _slotUiPrefab.TryGetComponent(out RectTransform slotRect);
        slotRect.sizeDelta = new Vector2(_slotSize, _slotSize);

        _slotUiPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if (itemSlot == null)
        {
            _slotUiPrefab.AddComponent<ItemSlotUI>();
        }

        _slotUiPrefab.SetActive(false);


        // --
        Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
        Vector2 curPos = beginPos;

        _slotUIList = new List<SlotUIBase>(_verticalSlotCount * _horizontalSlotCount);

        //���Ե� ���� ����
        for (int j = 0; j < _verticalSlotCount; j++)
        {
            for (int i = 0; i < _horizontalSlotCount; i++)
            {
                int slotIndex = (_horizontalSlotCount * j) + i;

                var slotRT = CloneSlot();
                Vector2 leftTopVec = new Vector2(0f, 1f);
                slotRT.pivot = leftTopVec; // Left Top
                slotRT.anchorMin = leftTopVec;
                slotRT.anchorMax = leftTopVec;
                slotRT.anchoredPosition = curPos;

                slotRT.gameObject.SetActive(true);
                slotRT.gameObject.name = $"Item Slot [{slotIndex}]";

                var slotUI = slotRT.GetComponent<ItemSlotUI>();
                slotUI.SetSlotIndex(slotIndex);
                _slotUIList.Add(slotUI);

                //Next X
                curPos.x += (_slotMargin + _slotSize);
            }

            //Next Line
            curPos.x = beginPos.x;
            curPos.y -= (_slotMargin + _slotSize);

        }

        // ���� ������ - �������� �ƴ� ��� �ı�
        if (_slotUiPrefab.scene.rootCount != 0)
            Destroy(_slotUiPrefab);

        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(_slotUiPrefab);
            slotGo.TryGetComponent(out RectTransform rt);
            rt.SetParent(_contentAreaRT); //_contentAreaRT�� �ڽ����� slot�� �Ҵ�

            return rt;
        }
    }
}
