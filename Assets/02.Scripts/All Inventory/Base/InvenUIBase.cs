using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
    * 부모 *
    InvenUIBase(abstract) : 만들어진 모든 슬롯들을 관리 / 감독 한다. (SlotUIBase List)
    
    [기능 - 유저 인터페이스]
        - 슬롯에 마우스 올리기
        - 드래그 앤 드롭

    [상속 구조]

           - ItemInventoryUI : 플레이어의 아이템 인벤토리 UI 관리
           - MerChantItemUI  : 상인이 판매하는 아이템 인벤토리 UI 관리
           - Skill_InvenUI   : 스킬 인벤토리의 UI 관리
           - PlayerEquipUI   : 플레이어 장착장비 인벤토리 UI 관리
*/


public abstract class InvenUIBase : MonoBehaviour
{
    /***********************************************************************
     *                              Fields                                 *
     ***********************************************************************/
    #region protected Fields
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

    protected int _curSiblingIndex;
    #endregion

    /***********************************************************************
    *                               Unity Events
    ***********************************************************************/
    #region Unity Event
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
    #endregion


    /***********************************************************************
    *                                 Methods
    ***********************************************************************/

    #region private Methods
    /// <summary>  드래그 하는 도중 /// </summary>
    private void OnPointerDrag()
    {
        if (_beginDragSlot == null) return;

        if (Input.GetMouseButton(_leftClick))
        {
            //위치 이름
            _beginDragIconTransform.position =
                _beginDragIconPoint + (Input.mousePosition - _beginDragCursorPoint);
        }
    }
    /// <summary>  클릭을 뗄 경우 /// </summary>
    private void OnPointerUp()
    {
        if (Input.GetMouseButtonUp(_leftClick))
        {
            // End Drag
            if (_beginDragSlot != null)
            {
                ComeBackSiblingUI();
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
    #endregion
    #region protected Methods
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
    protected virtual void ShowOrHideItemToolTip()
    {
        // 마우스가 유효한 아이템 아이콘 위에 올라와 있다면 툴팁 보여주기
        bool isValid =                                                 //09.19
            _pointerOverSlot != null && _pointerOverSlot.GetHasItem() //&& _pointerOverSlot.GetIsAccessible()
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
            if (_beginDragSlot != null && _beginDragSlot.GetHasItem() && _beginDragSlot.GetIsAccessible())
            {
                LastSiblingUI();
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
    /// <summary> 사용 중인 UI를 제일 먼저 보이게 설정 </summary>
    protected void LastSiblingUI()
    {
        _curSiblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
    }
    /// <summary> 사용이 끝나면 원래 보이는 순서로 컴백 </summary>
    protected void ComeBackSiblingUI()
    {
        transform.SetSiblingIndex(_curSiblingIndex);
    }
    /// <summary> 마우스 Ray에 들어오는 제일 첫번째 컴포넌트 반환 </summary>
    protected abstract T RaycastAndGetFirstComponent<T>() where T : Component;
    /// <summary> 현재 이벤트 시스템 밖으로 마우스포인터가 나갔는지 판단 </summary>
    protected bool IsOverUI() => EventSystem.current.IsPointerOverGameObject();
    /// <summary> 툴팁 UI의 슬롯 데이터 갱신 </summary>
    protected abstract void UpdateTooltipUI(SlotUIBase slot);
    /// <summary> 드래그가 끝났을 때 이벤트 </summary>
    protected abstract void EndDrag();
    /// <summary> 두 슬롯의 교환 </summary>
    protected abstract void TrySwapItems(SlotUIBase from, SlotUIBase to);
    /// <summary> UI 아이템 제거 </summary>
    protected abstract void TryRemoveItem(int index);
    #endregion
    #region public Methods
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
    public void RemoveItem(int index)
    {
        _slotUIList[index].RemoveItem();
    }
    #endregion

}
