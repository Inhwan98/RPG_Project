using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class InvenUIBase : MonoBehaviour
{
    [Header("Connected Objects")]
    [SerializeField] protected ItemTooltipUI _itemTooltip;         // 아이템 정보를 보여줄 툴팁 UI

    //아이템 슬롯 드롭 앤 드랍
    [SerializeField]
    protected List<SlotUIBase> _slotUIList = new List<SlotUIBase>(); // 아이템 슬롯 리스트

 
    protected GraphicRaycaster _gr;
    protected PointerEventData _ped;
    protected List<RaycastResult> _rrList;

    protected SlotUIBase _beginDragSlot;         //현재 드래그를 시작한 슬롯
    protected Transform _beginDragIconTransform; //해당 슬롯의 아이콘 트랜스폼

    protected SlotUIBase _beginUseSlot;         //현재 사용할 슬롯

    protected int _leftClick = 0;
    protected int _rightClick = 1;

    protected Vector3 _beginDragIconPoint;   //드래그 시작 시 슬롯의 위치
    protected Vector3 _beginDragCursorPoint; //드래그 시작 시 커서의 위치
    protected int _beginDragSlotSiblingIndex;

    protected SlotUIBase _pointerOverSlot; // 현재 포인터가 위치한 곳의 슬롯


    protected virtual void Awake()
    {
        Init();
    }

    protected virtual void Start()
    {
        this.gameObject.SetActive(false);
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

    protected virtual void Init()
    {
        TryGetComponent(out _gr);
        if (_gr == null)
            _gr = gameObject.AddComponent<GraphicRaycaster>();

        //Graphic RayCaster
        _ped = new PointerEventData(EventSystem.current);
        _rrList = new List<RaycastResult>(10);

    }

    /// <summary> 아이템 정보 툴팁 보여주거나 감추기 </summary>
    private void ShowOrHideItemToolTip()
    {
        // 마우스가 유효한 아이템 아이콘 위에 올라와 있다면 툴팁 보여주기
        bool isValid =
            _pointerOverSlot != null && _pointerOverSlot.GetHasItem() && _pointerOverSlot.GetIsAccessible()
            && (_pointerOverSlot != _beginDragSlot); //드래그 시작한 슬롯이면 보여주지 않기

        if (isValid)
        {
            UpdateTooltipUI(_pointerOverSlot);
            _itemTooltip.Show();
        }
        else
            _itemTooltip.Hide();
    }

    protected virtual void OnPointerDown()
    {
        // Left Click : begin Drag
        if (Input.GetMouseButtonDown(0))
        {
            _beginDragSlot = RaycastAndGetFirstComponent<SlotUIBase>();

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
                EndDrag();

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
        var curSlot = _pointerOverSlot = RaycastAndGetFirstComponent<SlotUIBase>();

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

    /// <summary> 접근 가능한 슬롯 범위 설정 </summary>
    public virtual void SetAccessibleSlotRange(int accessibleSlotCount)
    {
        for (int i = 0; i < _slotUIList.Count; i++)
        {
            _slotUIList[i].SetSlotAccessibleState(i < accessibleSlotCount);
        }
    }

    /// <summary> 슬롯에 아이템 아이콘 등록 </summary>
    public void SetItemIcon(int index, Sprite icon)
    {
        _slotUIList[index].SetItem(icon);
    }

   

    /// <summary> 슬롯에서 아이템 아이콘 제거, 개수 텍스트 숨기기 </summary>
    internal void RemoveItem(int index)
    {
        _slotUIList[index].RemoveItem();
    }

    protected abstract T RaycastAndGetFirstComponent<T>() where T : Component;

    protected bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();

    /// <summary> 툴팁 UI의 슬롯 데이터 갱신 </summary>
    protected abstract void UpdateTooltipUI(SlotUIBase slot);

    protected abstract void EndDrag();

    /// <summary> 두 슬롯의 교환 </summary>
    protected abstract void TrySwapItems(SlotUIBase from, SlotUIBase to);


    /// <summary> UI 아이템 제거 </summary>
    protected abstract void TryRemoveItem(int index);  

}
