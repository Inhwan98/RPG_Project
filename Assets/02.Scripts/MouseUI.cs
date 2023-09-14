using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseUI : MonoBehaviour
{
    private GraphicRaycaster _gr;
    private PointerEventData _ped;
    private List<RaycastResult> _rrList;

    private ItemSlotUI _beginDragSlot;         //현재 드래그를 시작한 슬롯
    private Transform _beginDragIconTransform; //해당 슬롯의 아이콘 트랜스폼

    private Vector3 _beginDragIconPoint;   //드래그 시작 시 슬롯의 위치
    private Vector3 _beginDragCursorPoint; //드래그 시작 시 커서의 위치
    private int _beginDragSlotSiblingIndex;

    private ItemSlotUI _pointerOverSlot; // 현재 포인터가 위치한 곳의 슬롯


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

            //아이템을 갖고 있는 슬롯만 해당
            if (_beginDragSlot != null && _beginDragSlot.GetHasItem())
            {
                //위치 기억, 참조 등록
                _beginDragIconTransform = _beginDragSlot.GetIconRect().transform;
                _beginDragIconPoint = _beginDragIconTransform.position;
                _beginDragCursorPoint = Input.mousePosition;

                //맨 위에 보이기
                _beginDragSlotSiblingIndex = _beginDragSlot.transform.GetSiblingIndex();
                _beginDragSlot.transform.SetAsLastSibling();

                // 해당 슬롯의 하이라이트 이미지를 아이콘보다 뒤에 위치시키기
                _beginDragSlot.SetHighlightOnTop(false);
            }
            else
            {
                _beginDragSlot = null;

            }
        }

    }

    /// <summary>  드래그 하는 도중 /// </summary>
    private void OnPointerDrag()
    {
        if (_beginDragSlot == null) return;

        if (Input.GetMouseButton(0))
        {
            //위치 이름
            _beginDragIconTransform.position =
                _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
        }
    }

    //private void EndDrag()
    //{
    //    ItemSlotUI endDragSlot = RaycastAndGetFirstComponent<ItemSlotUI>();

    //    if (endDragSlot != null && endDragSlot.GetIsAccessible())
    //    {
    //        // 수량 나누기 조건
    //        // 1) 마우스 클릭 떼는 순간 좌측 Ctrl 또는 Shift 키 유지
    //        // 2) begin : 셀 수 있는 아이템 / end : 비어있는 슬롯
    //        // 3) begin 아이템의 수량 > 1

    //        bool isSepartable =
    //            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift)) &&
    //            (_inventory.IsCountableItem(_beginDragSlot.GetIndex()) && !_inventory.HasItem(endDragSlot.GetIndex()));

    //        //true : 수량 나누기, false : 교환 또는 이동
    //        bool isSeparation = false;
    //        int currentAmount = 0;

    //        // 현재 개수 확인
    //        if (isSepartable)
    //        {
    //            currentAmount = _inventory.GetCurrentAmount(_beginDragSlot.GetIndex());
    //            if (currentAmount > 1)
    //            {
    //                isSeparation = true;
    //            }
    //        }

    //        // 1. 개수 나누기
    //        if (isSeparation)
    //            TrySeparateAmount(_beginDragSlot.GetIndex(), endDragSlot.GetIndex(), currentAmount);
    //        // 2. 교환 또는 이동
    //        else
    //            TrySwapItems(_beginDragSlot, endDragSlot);

    //        // 툴팁 갱신
    //        UpdateTooltipUI(endDragSlot);
    //        return;
    //    }

    //    // 버리기(커서가 UI 레이캐스트 타겟 위에 있지 않은 경우
    //    if (!IsOverUI())
    //    {
    //        // 확인 팝업 띄우고 콜백 위임
    //        int index = _beginDragSlot.GetIndex();
    //        string itemName = _inventory.GetItemName(index);
    //        int amount = _inventory.GetCurrentAmount(index);

    //        // 셀 수 있는 아이템의 경우, 수량 표시
    //        if (amount > 1)
    //            itemName += $" x{amount}";

    //        // 확인 팝업 띄우고 콜백 위임
    //        _popup.OpenConfirmationPopup(() => TryRemoveItem(index), itemName);
    //    }
    //}

    /// <summary>  클릭을 뗄 경우 /// </summary>
    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // End Drag
            if (_beginDragSlot != null)
            {
                // 위치 복원
                _beginDragIconTransform.position = _beginDragIconPoint;

                // UI 순서 복원
                _beginDragSlot.transform.SetSiblingIndex(_beginDragSlotSiblingIndex);

                //드래그 완료 처리

                //EndDrag();

                //참조 제거
                _beginDragSlot = null;
                _beginDragIconTransform = null;
            }
        }
    }





    /// <summary> 슬롯에 포인터가 올라가는 경우, 슬롯에서 포인터가 빠져나가는 경우 </summary>
    private void OnPointerEnterAndExit()
    {
        // 이전 프레임의 슬롯
        var prevSlot = _pointerOverSlot;

        // 현재 프레임의 슬롯
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

    private bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    /// <summary> 두 슬롯의 아이템 교환 </summary>
    private void TrySwapItems(ItemSlotUI from, ItemSlotUI to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to);
        //_inventory.Swap(from.GetIndex(), to.GetIndex());
    }

    /// <summary> 슬롯에 아이템 아이콘 등록 </summary>
    public void SetItemIcon(int index, Sprite icon)
    {
        ///_slotUIList[index].SetItem(icon);
    }


}