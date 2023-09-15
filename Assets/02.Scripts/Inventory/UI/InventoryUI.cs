using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryUI : StateInfoUI
{
    [Header("Options")]
    [Range(0, 10)]
    [SerializeField] private int _horizontalSlotCount = 8;    // 슬롯 개수 카운트
    [Range(0, 10)]
    [SerializeField] private int _verticalSlotCount = 8;      // 슬롯 세로 개수
    [SerializeField] private float _slotMargin = 8f;            // 한 슬롯의 상화좌우 여백
    [SerializeField] private float _contentAreaPadding = 20.0f; // 인벤토리 영역의 내부 여백

    [Range(32, 64)]
    [SerializeField] private float _slotSize = 64f;             // 각 슬롯의 크기

    [Space(16)]
    [SerializeField] private bool _mouseReversed = false; // 마우스 클릭 반전 여부

    [Header("Connected Objects")]
    [SerializeField] private RectTransform _contentAreaRT;       // 슬롯들이 위치할 영역
    [SerializeField] private GameObject _slotUiPrefab;           // 슬롯의 원본 프리팹
    [SerializeField] private InventoryPopupUI _popup;            // 팝업 UI 관리 객체

    [SerializeField] private Button _sortButton;
    [SerializeField] private Button _exitButton;

    /// <summary> 연결된 인벤토리 </summary>
    private Inventory _inventory;


    protected override void Awake()
    {
        InitSlots();
        _sortButton.onClick.AddListener(() => _inventory.SortAll());
        _exitButton.onClick.AddListener(() => _inventory.InventoryActive(false));

        base.Awake();
    }


    /// <summary> 툴팁 UI의 슬롯 데이터 갱신 </summary>
    protected override void UpdateTooltipUI(SlotUIBase slot)
    {
        if (!slot.GetIsAccessible() || !slot.GetHasItem())
            return;

        // 툴팁 정보 갱신
        _itemTooltip.SetItemInfo(_inventory.GetItemData(slot.GetIndex()));

        // 툴팁 위치 조정
        _itemTooltip.SetRectPosition(slot.GetSlotRect());
    }


    /// <summary> 인벤토리 참조 등록 (인벤토리에서 직접 호출) </summary>
    public void SetInventoryReference(Inventory inventory)
    {
        _inventory = inventory;
    }


    /// <summary> 아이템 사용 </summary>
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
            // 수량 나누기 조건
            // 1) 마우스 클릭 떼는 순간 좌측 Ctrl 또는 Shift 키 유지
            // 2) begin : 셀 수 있는 아이템 / end : 비어있는 슬롯
            // 3) begin 아이템의 수량 > 1

            bool isSepartable =
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) &&
                (_inventory.IsCountableItem(_beginDragSlot.GetIndex()) && !_inventory.HasItem(endDragSlot.GetIndex()));

            //true : 수량 나누기, false : 교환 또는 이동
            bool isSeparation = false;
            int currentAmount = 0;

            // 현재 개수 확인
            if (isSepartable)
            {
                currentAmount = _inventory.GetCurrentAmount(_beginDragSlot.GetIndex());
                if (currentAmount > 1)
                {
                    isSeparation = true;
                }
            }

            // 1. 개수 나누기
            if (isSeparation)
                TrySeparateAmount(_beginDragSlot.GetIndex(), endDragSlot.GetIndex(), currentAmount);
            // 2. 교환 또는 이동
            else
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
            string itemName = _inventory.GetItemName(index);
            int amount = _inventory.GetCurrentAmount(index);

            // 셀 수 있는 아이템의 경우, 수량 표시
            if (amount > 1)
                itemName += $" x{amount}";

            // 확인 팝업 띄우고 콜백 위임
            _popup.OpenConfirmationPopup(() => TryRemoveItem(index), itemName);
        }
    }

    /// <summary> UI 및 인벤토리에서 아이템 제거 </summary>
    protected override void TryRemoveItem(int index)
    {
        _inventory.Remove(index);
    }

    /// <summary> 셀 수 있는 아이템 개수 나누기 </summary>
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

    /// <summary> 해당 슬롯의 아이템 개수 텍스트 지정 </summary>
    public void SetItemAmountText(int index, int amount)
    {
        //amount가 1 이하일 경우 텍스트 미표시
        if(_slotUIList[index] is ItemSlotUI slotUI)
        {
            slotUI.SetItemAmount(amount);
        }
        
    }

    /// <summary> 해당 슬롯의 아이템 개수 텍스트 지정 </summary>
    internal void HideItemAmountText(int index)
    {
        if (_slotUIList[index] is ItemSlotUI slotUI)
        {
            slotUI.SetItemAmount(1);
        }
    }



    /// <summary> 두 슬롯의 아이템 교환 </summary>
    protected override void TrySwapItems(SlotUIBase from, SlotUIBase to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to);
        _inventory.Swap(from.GetIndex(), to.GetIndex());
    }


    /// <summary>
    /// 지정된 개수만큼 슬롯 영역 내에 슬롯들 동적 생성
    /// </summary>
    private void InitSlots()
    {
        // 슬롯 프리팹 설정
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

        //슬롯들 동적 생성
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

        // 슬롯 프리팹 - 프리팹이 아닌 경우 파괴
        if (_slotUiPrefab.scene.rootCount != 0)
            Destroy(_slotUiPrefab);

        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(_slotUiPrefab);
            slotGo.TryGetComponent(out RectTransform rt);
            rt.SetParent(_contentAreaRT); //_contentAreaRT의 자식으로 slot을 할당

            return rt;
        }
    }
}
