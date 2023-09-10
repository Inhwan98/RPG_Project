using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int _capacity;     //아이템 수용 한도

    [SerializeField, Range(8, 64)]
    private int _initalCapacity = 32; //초기 수용한도

    [SerializeField, Range(8, 64)]
    private int _maxCapacity = 64;     // 최대 수용 한도(아이템 배열 크기)

    [SerializeField]
    private InventoryUI _inventoryUI;

    /// <summary> 아이템 목록 </summary>
    private Item[] _items;

    /// <summary> 업데이트 할 인덱스 목록 </summary>
    private HashSet<int> _indexSetForUpdate = new HashSet<int>();

    private PlayerController _playerCtr;

    /// <summary> 아이템 데이터 타입별 정렬 가중치 </summary>
    private readonly static Dictionary<Type, int> _sortWeightDict = new Dictionary<Type, int>
        {
            { typeof(PortionItemData), 10000 },
            { typeof(WeaponItemData),  20000 },
            { typeof(ArmorItemData),   30000 },
        };

    private class ItemComparer : IComparer<Item>
    {
        public int Compare(Item a, Item b)
        {
            return (a.GetData().GetID() + _sortWeightDict[a.GetData().GetType()])
                 - (b.GetData().GetID() + _sortWeightDict[b.GetData().GetType()]);
        }
    }
    private static readonly ItemComparer _itemComparer = new ItemComparer();

    /// <summary> 아이템 수용 한도 </summary>
    public int GetCapacity() => _capacity;

    private void Awake()
    {
        Init_InvenItems();

        _capacity = _initalCapacity;
        _inventoryUI.SetInventoryReference(this);
    }

    private void Start()
    {
        UpdateAccessibleStatesAll();
        
    }

    private void OnDestroy()
    {
        SaveInven();
    }

    /// <summary> 인덱스가 수용 범위 내에 있는지 검사 </summary>
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < _capacity;
    }

    /// <summary> 앞에서부터 비어있는 슬롯 인덱스 탐색 </summary>
    private int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < _capacity; i++)
        {
            if (_items[i] == null)
                return i;
        }

        return -1;
    }

    /// <summary> 모든 슬롯 UI에 접근 가능 여부 업데이트 </summary>
    private void UpdateAccessibleStatesAll()
    {
        _inventoryUI.SetAccessibleSlotRange(_capacity);
    }

    /// <summary> 해당 슬롯이 아이템을 갖고 있는지 여부 </summary>
    public bool HasItem(int index)
    {
        return IsValidIndex(index) && _items[index] != null;
    }

    /// <summary> 해당 슬롯이 셀 수 있는 아이템인지 여부 </summary>
    public bool IsCountableItem(int index)
    {
        return HasItem(index) && _items[index] is CountableItem;
    }

    /// <summary>
    /// 해당 슬롯의 현재 아이템 개수 리턴
    /// <para/> - 잘못된 인덱스 : -1 리턴
    /// <para/> - 빈 슬롯 : 0 리턴
    /// <para/> - 셀 수 없는 아이템 : 1 리턴
    /// </summary>
    public int GetCurrentAmount(int index)
    {
        if (!IsValidIndex(index)) return -1;
        if (_items[index] == null) return 0;

        CountableItem ci = _items[index] as CountableItem;
        if (ci == null)
            return 1;

        return ci.GetAmount();
    }

    /// <summary> 해당 슬롯의 아이템 정보 리턴 </summary>
    public ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (_items[index] == null) return null;

        return _items[index].GetData();
    }

    /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (_items[index] == null) return "";

        return _items[index].GetData().GetName();
    }


    public void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        Item itemA = _items[indexA];
        Item itemB = _items[indexB];

        // 1. 셀 수 있는 아이템이고, 동일한 아이템일 경우
        // indexA -> indexB로 개수 합치기
        if (itemA != null && itemB != null &&
            itemA.GetData().GetID() == itemB.GetData().GetID() &&
            itemA is CountableItem ciA && itemB is CountableItem ciB)
        {
            int maxAmount = ciB.GetMaxAmount();
            int sum = ciA.GetAmount() + ciB.GetAmount();

            if (sum <= maxAmount)
            {
                ciA.SetAmount(0);
                ciB.SetAmount(sum);
            }
            else
            {
                ciA.SetAmount(sum - maxAmount);
                ciB.SetAmount(maxAmount);
            }
        }
        // 2. 일반적인 경우 : 슬롯 교체
        else
        {
            _items[indexA] = itemB;
            _items[indexB] = itemA;
        }

        // 두 슬롯 정보 갱신
        UpdateSlot(indexA, indexB);
    }

    /// <summary> 해당하는 인덱스의 슬롯들의 상태 및 UI 갱신 </summary>
    private void UpdateSlot(params int[] indices)
    {
        foreach (var i in indices)
        {
            UpdateSlot(i);
        }
    }

    /// <summary> 해당하는 인덱스의 슬롯 상태 및 UI 갱신 </summary>
    public void UpdateSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        Item item = _items[index];

        // 1. 아이템이 슬롯에 존재하는 경우
        if (item != null)
        {
            //아이콘 등록
            _inventoryUI.SetItemIcon(index, item.GetData().GetIconSprite());

            // 1-1. 셀 수 있는 아이템
            if (item is CountableItem ci)
            {
                // 1-1-1. 수량이 0인 경우, 아이템 제거
                if (ci.IsEmpty())
                {
                    _items[index] = null;
                    RemoveIcon();
                    return;
                }
                // 1-1-2. 수량 텍스트 표시
                else
                {
                    _inventoryUI.SetItemAmountText(index, ci.GetAmount());
                }
            }
            // 1-2. 셀 수 없는 아이템인 경우 수량 텍스트 제거
            else
            {
                _inventoryUI.HideItemAmountText(index);
            }
        }
        //2. 빈 슬롯인 경우 : 아이콘 제거
        else
        {
            RemoveIcon();
        }

        // 로컬 : 아이콘 제거하기
        void RemoveIcon()
        {
            _inventoryUI.RemoveItem(index);
            _inventoryUI.HideItemAmountText(index);
        }
    }

    

    ///<summary> 인벤토리에 아이템 추가
    /// <para/> 넣는 데 실패한 아이템 개수 리턴
    /// <para/> 리턴이 0이면 넣는데 모두 성공했다는 의미
    /// </summary>
    public int Add(ItemData itemData, int amount = 1)
    {
        if (itemData == null) return 0;

        int index;

        //1. 수량이 있는 아이템
        if (itemData is CountableItemData ciData)
        {
            bool findNextCountable = true;
            index = -1;

            while (amount > 0)
            {
                // 1-1. 이미 해당 아이템이 인벤토리 내에 존재하고, 개수 여유 있는지 검사
                if (findNextCountable)
                {
                    index = FindCountableItemSlotIndex(ciData, index + 1);

                    // 개수 여유있는 기존재 슬롯이 더이상 없다고 판단될 경우, 빈 슬롯부터 탐색 시작
                    if (index == -1)
                    {
                        findNextCountable = false;
                    }
                    // 기존재 슬롯을 찾은 경우, 양 증가시키고 초과량 존재 시 amount에 초기화
                    else
                    {
                        CountableItem ci = _items[index] as CountableItem;
                        amount = ci.AddAmountAndGetExcess(amount);

                        UpdateSlot(index);
                    }
                }
                // 1-2. 빈 슬롯 탐색
                else
                {
                    index = FindEmptySlotIndex(index + 1);

                    // 빈 슬롯조차 없는 경우 종료
                    if (index == -1)
                    {
                        break;
                    }
                    // 빈 슬롯 발견 시, 슬롯에 아이템 추가 및 남는량 계산
                    else
                    {
                        //새로운 아이템 생성
                        CountableItem ci = ciData.CreateItem() as CountableItem;
                        ci.SetAmount(amount);

                        // 슬롯에 추가
                        _items[index] = ci;

                        // 남은 개수 계산
                        amount = (amount > ciData.GetMaxAmount()) ? (amount - ciData.GetMaxAmount()) : 0;
                        UpdateSlot(index);
                    }
                }
            }
        }
        // 2. 수량이 없는 아이템
        else
        {
            // 2-1. 1개만 넎는 경우, 간단히 수행
            if (amount == 1)
            {
                index = FindEmptySlotIndex();
                if (index != -1)
                {
                    //아이템을 생성하여 슬롯에 추가
                    _items[index] = itemData.CreateItem();
                    amount = 0;

                    UpdateSlot(index);
                }
            }

            // 2-2. 2개 이상의 수량 없는 아이템을 동시에 추가하는 경우
            index = -1;
            for (; amount > 0; amount--)
            {
                //아이템 넣은 인덱스의 다음 인덱스부터 슬롯 탐색
                index = FindEmptySlotIndex(index + 1);

                // 다 넣지 못한 경우 루프 종료
                if (index == -1)
                {
                    break;
                }

                //아이템을 생성하여 슬롯에 추가
                _items[index] = itemData.CreateItem();

                UpdateSlot(index);
            }
        }

        return amount;
    }

    ///<summary> 해당 슬롯의 아이템 제거 </summary>
    public void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        _items[index] = null;
        UpdateSlot(index);
    }

    /// <summary> 앞에서부터 개수 여유가 있는 Countable 아이템의 슬롯 인덱스 탐색 </summary>
    private int FindCountableItemSlotIndex(CountableItemData target, int startIndex = 0)
    {
        for (int i = startIndex; i < _capacity; i++)
        {
            var current = _items[i];
            if (current == null)
                continue;

            // 아이템 종류 일치, 개수 여유 확인
            if (current.GetData().GetID() == target.GetID() && current is CountableItem ci)
            {
                
                if (!ci.GetIsMax())
                    return i;
            }
        }

        return -1;
    }

    /// <summary> 셀 수 있는 아이템의 수량 나누기(A -> B 슬롯으로) </summary>
    public void SeparateAmount(int indexA, int indexB, int amount)
    {
        // amount : 나눌 목표 수량

        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        Item _itemA = _items[indexA];
        Item _itemB = _items[indexB];

        CountableItem _ciA = _itemA as CountableItem;

        // 조건 : A 슬롯 - 셀 수 있는 아이템 / B 슬롯 - Null
        // 조건에 맞는 경우, 복제하여 슬롯 B에 추가
        if (_ciA != null && _itemB == null)
        {
            _items[indexB] = _ciA.SeperateAndClone(amount);

            UpdateSlot(indexA, indexB);
        }
    }

    /// <summary> 해당 슬롯의 아이템 사용 </summary>
    public void Use(int index)
    {
        if (_items[index] == null) return;

        // 사용 가능한 아이템인 경우
        if (_items[index] is IUsableItem uItem)
        {
            //아이템 사용
            bool succeeded = uItem.Use();

            if(_items[index] is PortionItem pitem)
            {
                PortionItemData potionData = pitem.GetData() as PortionItemData;
                IPortionState portionState = potionData.GetPortionState();

                float value = potionData.GetValue();

                if (portionState == IPortionState.HP) _playerCtr.RecoveryHP(value);
                else if (portionState == IPortionState.MP) _playerCtr.RecoveryMP(value);
            }

            if (succeeded)
            {
                UpdateSlot(index);
            }
        }
    }

    public void TrimAll()
    {
        _indexSetForUpdate.Clear();

        int i = -1;
        while (_items[++i] != null) ;
        int j = i;

        while (true)
        {
            while (++j < _capacity && _items[j] == null) ;

            if (j == _capacity)
                break;

            _indexSetForUpdate.Add(i);
            _indexSetForUpdate.Add(j);

            _items[i] = _items[j];
            _items[j] = null;
            i++;
        }

        foreach (var index in _indexSetForUpdate)
        {
            UpdateSlot(index);
        }
    }

    /// <summary> 빈 슬롯 없이 채우면서 아이템 종류별로 정렬하기 </summary>
    public void SortAll()
    {
        // 1. Trim
        int i = -1;
        while (_items[++i] != null) ;
        int j = i;

        while (true)
        {
            while (++j < _capacity && _items[j] == null) ;

            if (j == _capacity)
                break;

            _items[i] = _items[j];
            _items[j] = null;
            i++;
        }

        // 2. sort
        Array.Sort(_items, 0, i, _itemComparer);

        // 3.Update

        UpdateAllSlot();
    }


    /// <summary> 모든 슬롯들의 상태를 UI에 갱신 </summary>
    private void UpdateAllSlot()
    {
        for (int i = 0; i < _capacity; i++)
        {
            UpdateSlot(i);
        }
    }

    public void SetPlayerCtr(PlayerController player)
    {
        _playerCtr = player;
    }

    /// <summary> Inventory 활성화 유무 (마우스 커서도 같이 활성화) </summary>
    public void InventoryActive(bool value)
    {
        _inventoryUI.gameObject.SetActive(value);
        _playerCtr.SetUseInven(value);             //플레이어의 움직임 제어
        _playerCtr.GetCameraCtr().UseInven(value); //카메라의 회전 제어

        if (value) GameManager.instance.VisibleCursor();
        else
            GameManager.instance.InvisibleCursor();
    }

    [ContextMenu("SaveInven")]
    public void SaveInven()
    {
        SaveSys.SaveInvenItem(_items);
    }

    /// <summary> Load Data 없을시 초기화 </summary>
    public void Init_InvenItems()
    {
        _items = SaveSys.LoadInvenitem();
        if (_items == null) _items = new Item[_maxCapacity];
    }
}