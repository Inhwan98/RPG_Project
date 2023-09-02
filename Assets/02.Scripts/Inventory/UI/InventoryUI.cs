using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventoryUI : MonoBehaviour
{
    [Space(16)]
    [SerializeField] private bool _mouseReversed = false; // ���콺 Ŭ�� ���� ����

    [Header("Connected Objects")]
    [SerializeField] private RectTransform _contentAreaRT;       // ���Ե��� ��ġ�� ����
    [SerializeField] private GameObject _slotUiPrefab;           // ������ ���� ������
    [SerializeField] private ItemTooltipUI _itemTooltip;         // ������ ������ ������ ���� UI
    [SerializeField] private InventoryPopupUI _popup;            // �˾� UI ���� ��ü

    [SerializeField] private Button _sortButton;

    /// <summary> ����� �κ��丮 </summary>
    private Inventory _inventory;

    //������ ���� ��� �� ���
    private List<ItemSlotUI> _slotUIList = new List<ItemSlotUI>(); // ������ ���� ����Ʈ
    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;

    private ItemSlotUI _beginDragSlot;         //���� �巡�׸� ������ ����
    private Transform _beginDragIconTransform; //�ش� ������ ������ Ʈ������

    private ItemSlotUI _beginUseSlot;         //���� ����� ����

    private int _leftClick = 0;
    private int _rightClick = 1;

    private Vector3 _beginDragIconPoint;   //�巡�� ���� �� ������ ��ġ
    private Vector3 _beginDragCursorPoint; //�巡�� ���� �� Ŀ���� ��ġ
    private int _beginDragSlotSiblingIndex;

    private ItemSlotUI _pointerOverSlot; // ���� �����Ͱ� ��ġ�� ���� ����




    void Awake()
    {
        Init();
        _sortButton.onClick.AddListener(() => _inventory.SortAll());
    }

    private void Update()
    {
        _ped.position = Input.mousePosition;

        OnPointerEnterAndExit();

        ShowOrHideItemToolTip();

        OnPointerDown();
        OnPointerDrag();
        OnPointerUp();
    }


    private void Init()
    {
        TryGetComponent(out _gr);
        if (_gr == null)
            _gr = gameObject.AddComponent<GraphicRaycaster>();

        //Graphic RayCaster
        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10);

    }

    /// <summary> ������ ���� ���� �����ְų� ���߱� </summary>
    private void ShowOrHideItemToolTip()
    {
        // ���콺�� ��ȿ�� ������ ������ ���� �ö�� �ִٸ� ���� �����ֱ�
        bool isValid =
            _pointerOverSlot != null && _pointerOverSlot.GetHasItem() && _pointerOverSlot.GetIsAccessible()
            && (_pointerOverSlot != _beginDragSlot); //�巡�� ������ �����̸� �������� �ʱ�

        if (isValid)
        {
            UpdateTooltipUI(_pointerOverSlot);
            _itemTooltip.Show();
        }
        else
            _itemTooltip.Hide();
    }

    /// <summary> ���� UI�� ���� ������ ���� </summary>
    private void UpdateTooltipUI(ItemSlotUI slot)
    {
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

    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _rrList.Clear();

        _gr.Raycast(_ped, _rrList);

        if (_rrList.Count == 0)
            return null;
        return _rrList[0].gameObject.GetComponent<T>();
    }

    private void OnPointerDown()
    {
        // Left Click : begin Drag
        if (Input.GetMouseButtonDown(0))
        {
            _beginDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

            //�������� ���� �ִ� ���Ը� �ش�
            if (_beginDragSlot != null && _beginDragSlot.GetHasItem())
            {
                //��ġ ���, ���� ���
                _beginDragIconTransform = _beginDragSlot.GetIconRect().transform;
                _beginDragIconPoint = _beginDragIconTransform.position;
                _beginDragCursorPoint = Input.mousePosition;

                //�� ���� ���̱�
                _beginDragSlotSiblingIndex = _beginDragSlot.transform.GetSiblingIndex();
                _beginDragSlot.transform.SetAsLastSibling();

                // �ش� ������ ���̶���Ʈ �̹����� �����ܺ��� �ڿ� ��ġ��Ű��
                _beginDragSlot.SetHighlightOnTop(false);
            }
            else
            {
                _beginDragSlot = null;

            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ItemSlotUI slot = RaycastAndGetFirstComponent<ItemSlotUI>();

            if (slot != null && slot.GetHasItem() && slot.GetIsAccessible())
            {
                TryUseItem(slot.GetIndex());
            }
        }
    }

    /// <summary>  �巡�� �ϴ� ���� /// </summary>
    private void OnPointerDrag()
    {
        if (_beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            //��ġ �̸�
            _beginDragIconTransform.position =
                _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
        }
    }

    /// <summary>  Ŭ���� �� ��� /// </summary>
    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // End Drag
            if (_beginDragSlot != null)
            {
                // ��ġ ����
                _beginDragIconTransform.position = _beginDragIconPoint;

                // UI ���� ����
                _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                //�巡�� �Ϸ� ó��
                EndDrag();

                //���� ����
                _beginDragSlot = null;
                _beginDragIconTransform = null;
            }
        }
    }

    /// <summary> ���Կ� �����Ͱ� �ö󰡴� ���, ���Կ��� �����Ͱ� ���������� ��� </summary>
    private void OnPointerEnterAndExit()
    {
        // ���� �������� ����
        var prevSlot = _pointerOverSlot;

        // ���� �������� ����
        var curSlot = _pointerOverSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

        if (prevSlot == null)
        {
            // Enter
            if (curSlot != null)
            {
                OnCurrentEnter();
            }
        }
        else
        {
            // Exit
            if (curSlot == null)
            {
                OnPrevExit();
            }

            //Change
            else if (prevSlot != curSlot)
            {
                OnPrevExit();
                OnCurrentEnter();
            }
        }

        // ===== Local Methods =====
        void OnCurrentEnter()
        {
            curSlot.Highlight(true);
        }

        void OnPrevExit()
        {
            prevSlot.Highlight(false);
        }

    }

    /// <summary> ���콺 Ŭ�� �¿� ������Ű�� (true : ����) </summary>
    public void InvertMouse(bool value)
    {
        _leftClick = value ? 1 : 0;
        _rightClick = value ? 0 : 1;

        _mouseReversed = value;
    }

    /// <summary> ������ ��� </summary>
    private void TryUseItem(int index)
    {
        EditorLog($"UI - Try Use Item : Slot [{index}]");

        _inventory.Use(index);
    }

    /// <summary> ���Կ� ������ ������ ��� </summary>
    public void SetItemIcon(int index, Sprite icon)
    {
        EditorLog($"Set Item Icon : Slot [{index}]");

        _slotUIList[index].SetItem(icon);
    }

    /// <summary> �ش� ������ ������ ���� �ؽ�Ʈ ���� </summary>
    public void SetItemAmountText(int index, int amount)
    {
        EditorLog($"Set Item Amount Text : Slot [{index}], Amount [{amount}]");

        // NOTE : amount�� 1 ������ ��� �ؽ�Ʈ ��ǥ��
        _slotUIList[index].SetItemAmount(amount);
    }

    /// <summary> ���Կ��� ������ ������ ����, ���� �ؽ�Ʈ ����� </summary>
    internal void RemoveItem(int index)
    {
        _slotUIList[index].RemoveItem();
    }

    /// <summary> �ش� ������ ������ ���� �ؽ�Ʈ ���� </summary>
    internal void HideItemAmountText(int index)
    {
        _slotUIList[index].SetItemAmount(1);
    }



    private void EndDrag()
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
    private void TryRemoveItem(int index)
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

    private bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    /// <summary> �� ������ ������ ��ȯ </summary>
    private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to);
        _inventory.Swap(from.GetIndex(), to.GetIndex());
    }

    /// <summary> ���� ������ ���� ���� ���� </summary>
    public void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < _slotUIList.Count; i++)
        {
            _slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }
    }

    /***********************************************************************
       *                               Editor Only Debug
       ***********************************************************************/
    #region .

    [Header("Editor Options")]
    [SerializeField] private bool _showDebug = true;
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void EditorLog(object message)
    {
        if (!_showDebug) return;
        UnityEngine.Debug.Log($"[InventoryUI] {message}");
    }
}
    #endregion

  

