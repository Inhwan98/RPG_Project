using System.Text;
using UnityEngine;
using UnityEngine.UI;


public class MerChantItemUI : InvenUIBase
{
    /****************************************
     *              Fields
     ****************************************/
    #region option Fields
    [SerializeField] private GameObject _itemInvenGo;
    [SerializeField] private ItemInventoryPopupUI _popup;            // 팝업 UI 관리 객체
    [SerializeField] private Button _exitButton;
    #endregion
    #region private Fields
    /// <summary> 연결된 인벤토리 </summary>
    private MerChantInventoryManager _merChantInvenMgr;
    private PlayerController _playerCtr;
    private GraphicRaycaster _itemInvenGr;
    private Button[] _buyButtonArray; // 판매버튼
    StringBuilder sb = new StringBuilder();
    #endregion


    /****************************************
    *              Get Set Methods
    ****************************************/

    #region Set Methods
    /// <summary> 인벤토리 참조 등록 (인벤토리에서 직접 호출) </summary>
    public void SetInventoryReference(MerChantInventoryManager merChantInven)
    {
        _merChantInvenMgr = merChantInven;
    }
    #endregion


    /****************************************
    *                 Methods
    ****************************************/

    #region private Fields

    #region 구매 / 판매 관련
    /// <summary> 아이템 구매 관련 함수 </summary>
    private void BuyItem(int index)
    {
        int possibleBuyAmount; //구매 가능한 양
        string itemName = _merChantInvenMgr.GetItemName(index);
        bool isPossibleBuy = _merChantInvenMgr.IsPossibleToBuy(index, out possibleBuyAmount);
        bool isCountableItem = _merChantInvenMgr.IsCountableItem(index); //셀 수 있는 아이템인가
        var itemData = _merChantInvenMgr.GetItemData(index);

        //구매 가능한게 아니라면 리턴
        if (!isPossibleBuy) return;

        if (isCountableItem)
        {
            _popup.OpenAmountInputPopup(amt => _merChantInvenMgr.BuyItem(itemData, amt),
                                               possibleBuyAmount, itemName);
        }
        else
            _merChantInvenMgr.BuyItem(itemData);
    }
    /// <summary> 아이템 판매 관련 함수 </summary>
    private void Sell_Item(int index)
    {
        //판매되는 아이템 정보를 merChantInvenMgr => ItemInvenManger 에서 가져온다.
        string itemName = _merChantInvenMgr.GetSell_ItemName(index);
        bool isCountableItem = _merChantInvenMgr.GetSell_IsCountableItem(index);
        int itemAmount = _merChantInvenMgr.GetSell_ItemAmount(index);
        // 확인 팝업 띄우고 콜백 위임

        if(isCountableItem)
        {
            sb.Append($"{itemName} {itemAmount}개");
            _popup.OpenConfirmationPopup(() => _merChantInvenMgr.Sell_item(index, itemAmount), sb.ToString());
            sb.Clear();
        }
        else
        {
            _popup.OpenConfirmationPopup(() => _merChantInvenMgr.Sell_item(index), itemName);
        }

    }
    #endregion

    #endregion
    #region protected Fields
    protected override void Init()
    {
        int slotSize = _slotUIList.Count;

        //판매버튼
        _buyButtonArray = new Button[slotSize];
        for (int i = 0; i < slotSize; i++)
        {
            _slotUIList[i].SetSlotIndex(i);

            var skillSlot = _slotUIList[i] as MerChantSlotUI;
            int index = skillSlot.GetIndex();

            _buyButtonArray[i] = skillSlot.GetBuyButton();
            _buyButtonArray[i].onClick.AddListener(() => BuyItem(index));
        }

        _exitButton.onClick.AddListener(() => _merChantInvenMgr.SetWindowActive(false));

        _itemInvenGo.TryGetComponent(out _itemInvenGr);
        if (_itemInvenGr == null)
            _itemInvenGr = _itemInvenGo.AddComponent<GraphicRaycaster>();
        base.Init();
    }
    /// <summary> 툴팁 UI의 슬롯 데이터 갱신 </summary>
    protected override void UpdateTooltipUI(SlotUIBase slot)
    {
        if (!slot.GetIsAccessible() || !slot.GetHasItem())
            return;

        // 툴팁 정보 갱신
        _itemTooltip.SetItemInfo(_merChantInvenMgr.GetItemData(slot.GetIndex()));

        // 툴팁 위치 조정
        _itemTooltip.SetRectPosition(slot.GetSlotRect());
    }
    /// <summary> 슬롯 우클릭시 판매, 구입 이벤트 </summary>
    protected override void OnPointerDown() 
    {
        base.OnPointerDown();
        if (Input.GetMouseButtonDown(1))
        {
            SlotUIBase slot = RaycastAndGetFirstComponent<SlotUIBase>();

            if (slot != null && slot.GetHasItem() && slot.GetIsAccessible())
            {
                if(slot is MerChantSlotUI)
                {
                    BuyItem(slot.GetIndex());
                }
                else if(slot is ItemInvenSlotUI)
                {
                    Sell_Item(slot.GetIndex());
                }
            }
        }
    }
    protected override void ShowOrHideItemToolTip()
    {
        // 마우스가 유효한 아이템 아이콘 위에 올라와 있다면 툴팁 보여주기
        bool isValid =                                                 //09.19
            _pointerOverSlot != null && _pointerOverSlot.GetHasItem() //&& _pointerOverSlot.GetIsAccessible()
            && (_pointerOverSlot != _beginDragSlot) && _pointerOverSlot is MerChantSlotUI; //드래그 시작한 슬롯이면 보여주지 않기

        if (isValid)
        {
            UpdateTooltipUI(_pointerOverSlot);
            _itemTooltip.Show();
        }
        else
            _itemTooltip.Hide();
    }
    protected override void EndDrag()
    {
        SlotUIBase endDragSlot = RaycastAndGetFirstComponent<SlotUIBase>();

        if (endDragSlot != null && endDragSlot.GetIsAccessible())
        {
            //구입
            if (_beginDragSlot is MerChantSlotUI && endDragSlot is ItemInvenSlotUI)
            {
                int index = _beginDragSlot.GetIndex();
                BuyItem(index);

                //TryMoveItems(_beginDragSlot, endDragSlot);
                // 툴팁 갱신

            }
            //판매
            else if (_beginDragSlot is ItemInvenSlotUI && endDragSlot is MerChantSlotUI)
            {
                int index = _beginDragSlot.GetIndex();
                Sell_Item(index);
            }
        }
    }
    /// <summary> 마우스 레이케스트 관리 및 레이목록에 들어온 첫번째 컴포넌트 반환 </summary>
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
        _merChantInvenMgr.Remove(index);
    }
    /// <summary> 두 슬롯의 아이템 교환 </summary>
    protected override void TrySwapItems(SlotUIBase from, SlotUIBase to)
    {
        if (from == to) return;
        from.SwapOnMoveIcon(to);
        _merChantInvenMgr.Swap(from.GetIndex(), to.GetIndex());
    }
    #endregion
    #region public Fields
    /// <summary> 상점 Slot 개수만큼 상점 초기화 </summary>
    public void MerChantUI_Init()
    {
        int slotSize = _slotUIList.Count;
        for (int i = 0; i < slotSize; i++)
        {

            var skillSlot = _slotUIList[i] as MerChantSlotUI;
            int index = skillSlot.GetIndex();
            //슬롯에 해당하는 아이템 데이터를 바탕으로 판매상인의 UI 초기화
            ItemData data = _merChantInvenMgr.GetItemData(index);
            skillSlot.MerChantSlotUI_Init(data);
        }
    }
    /// <summary> 해당 슬롯의 아이템 개수 텍스트 지정 </summary>
    public void HideItemAmountText(int index)
    {
        if (_slotUIList[index] is ItemInvenSlotUI slotUI)
        {
            slotUI.SetItemAmount(1);
        }
    }
    #endregion
}
