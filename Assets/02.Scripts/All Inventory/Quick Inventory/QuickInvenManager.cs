using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickInvenManager : BaseItemInvenManager
{
    /*****************************************************
     *                       Fields
     *****************************************************/
    #region private Fields
    private QuickInvenUI _quickInvenUI;
    private ItemInventoryManager _itemInvenMgr;
    private PlayerController _playerCtr;
    private Item[] _items;

    private int _maxCapacity = 2;     // �ִ� ���� �ѵ�(������ �迭 ũ��)
    private int _capacity;     //������ ���� �ѵ�

    #endregion

    /*****************************************************
     *                    Get, Set Methods
     *****************************************************/
    public override void SetPlayerCtr(PlayerController player) => _playerCtr = player;
    public void SetItemInvenMgr(ItemInventoryManager intemInvenMgr) => _itemInvenMgr = intemInvenMgr;

    private void Awake()
    {
        Init_InvenItems();
    }


    void Start()
    {
        UpdateAccessibleStatesAll();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /***********************************************************************
    *                               Override Methods 
    ***********************************************************************/
    #region Public override Methods
    /// <summary> �ε����� ���� ���� ���� �ִ��� �˻� </summary>
    public override bool IsValidIndex(int index) => index >= 0 && index < _capacity;
    /// <summary> �ش� ������ �������� ���� �ִ��� ���� </summary>
    public override bool HasItem(int index) => IsValidIndex(index) && _items[index] != null;
    /// <summary> �ش� ������ ������ ���� ���� </summary>
    public override ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (_items[index] == null) return null;

        return _items[index].GetData();
    }
    /// <summary> �ش� ������ ������ �̸� ���� </summary>
    public override string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (_items[index] == null) return "";

        return _items[index].GetData().GetName();
    }
    /// <summary> �ش� ������ ��ȯ </summary>
    public override Item GetItem(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (_items[index] == null) return null;

        return _items[index];
    }
    /// <summary> �ش��ϴ� �ε����� ���Ե��� ���� �� UI ���� </summary>
    public override void UpdateSlot(params int[] indices)
    {
        foreach (var i in indices)
        {
            UpdateSlot(i);
        }
    }
    /// <summary> �ش��ϴ� �ε����� ���� ���� �� UI ���� </summary>
    public override void UpdateSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        Item item = _items[index];

        // 1. �������� ���Կ� �����ϴ� ���
        if (item != null)
        {

            //������ ���
            _quickInvenUI.SetItemIcon(index, item.GetData().GetIconSprite());

            // 1-1. �� �� �ִ� ������
            if (item is CountableItem ci)
            {
                // 1-1-1. ������ 0�� ���, ������ ����
                if (ci.IsEmpty())
                {
                    _items[index] = null;
                    RemoveIcon();
                    return;
                }
                // 1-1-2. ���� �ؽ�Ʈ ǥ��
                else
                {
                    _quickInvenUI.SetItemAmountText(index, ci.GetAmount());
                }
            }
            // 1-2. �� �� ���� �������� ��� ���� �ؽ�Ʈ ����
            else
            {
                _quickInvenUI.HideItemAmountText(index);
            }
        }
        //2. �� ������ ��� : ������ ����
        else
        {
            RemoveIcon();
        }

        // ���� : ������ �����ϱ�
        void RemoveIcon()
        {
            _quickInvenUI.RemoveItem(index);
            _quickInvenUI.HideItemAmountText(index);
        }
    }
    /// <summary> ��� ���Ե��� ���¸� UI�� ���� </summary>
    public override void UpdateAllSlot()
    {
        for (int i = 0; i < _capacity; i++)
        {
            UpdateSlot(i);
        }
    }
    ///<summary>
    /// �κ��丮�� ������ �߰�<br></br>
    /// �ִ� �� ������ ������ ���� ����<br></br>
    /// ������ 0�̸� �ִµ� ��� �����ߴٴ� �ǹ�
    /// </summary>
    public override int AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null) return 0;

        int index;

        //1. ������ �ִ� ������
        if (itemData is CountableItemData ciData)
        {
            bool findNextCountable = true;
            index = -1;

            while (amount > 0)
            {
                // 1-1. �̹� �ش� �������� �κ��丮 ���� �����ϰ�, ���� ���� �ִ��� �˻�
                if (findNextCountable)
                {
                    index = FindCountableItemSlotIndex(ciData, index + 1);

                    // ���� �����ִ� ������ ������ ���̻� ���ٰ� �Ǵܵ� ���, �� ���Ժ��� Ž�� ����
                    if (index == -1)
                    {
                        findNextCountable = false;
                    }
                    // ������ ������ ã�� ���, �� ������Ű�� �ʰ��� ���� �� amount�� �ʱ�ȭ
                    else
                    {
                        CountableItem ci = _items[index] as CountableItem;
                        amount = ci.AddAmountAndGetExcess(amount);

                        UpdateSlot(index);
                    }
                }
                // 1-2. �� ���� Ž��
                else
                {
                    index = FindEmptySlotIndex(index + 1);

                    // �� �������� ���� ��� ����
                    if (index == -1)
                    {
                        break;
                    }
                    // �� ���� �߰� ��, ���Կ� ������ �߰� �� ���·� ���
                    else
                    {
                        //���ο� ������ ����
                        CountableItem ci = ciData.CreateItem() as CountableItem;
                        ci.SetAmount(amount);

                        // ���Կ� �߰�
                        _items[index] = ci;

                        // ���� ���� ���
                        amount = (amount > ciData.GetMaxAmount()) ? (amount - ciData.GetMaxAmount()) : 0;
                        UpdateSlot(index);
                    }
                }
            }
        }
        // 2. ������ ���� ������
        else
        {
            // 2-1. 1���� ���� ���, ������ ����
            if (amount == 1)
            {
                index = FindEmptySlotIndex();
                if (index != -1)
                {
                    //�������� �����Ͽ� ���Կ� �߰�
                    _items[index] = itemData.CreateItem();
                    amount = 0;

                    UpdateSlot(index);
                }
            }

            // 2-2. 2�� �̻��� ���� ���� �������� ���ÿ� �߰��ϴ� ���
            index = -1;
            for (; amount > 0; amount--)
            {
                //������ ���� �ε����� ���� �ε������� ���� Ž��
                index = FindEmptySlotIndex(index + 1);

                // �� ���� ���� ��� ���� ����
                if (index == -1)
                {
                    break;
                }

                //�������� �����Ͽ� ���Կ� �߰�
                _items[index] = itemData.CreateItem();

                UpdateSlot(index);
            }
        }

        return amount;
    }
    /// <summary> Inventory Ȱ��ȭ ���� </summary>
    public override void SetWindowActive(bool value)
    {
        _playerCtr.SetUseItemInven(value); //�÷��̾��� ������ ����
        _quickInvenUI.gameObject.SetActive(value);
    }
    ///<summary> �ش� ������ ������ ���� </summary>
    public override void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        _items[index] = null;
        UpdateSlot(index);
    }
    public override void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        Item itemA = _items[indexA];
        Item itemB = _items[indexB];

        // 1. �� �� �ִ� �������̰�, ������ �������� ���
        // indexA -> indexB�� ���� ��ġ��
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
        // 2. �Ϲ����� ��� : ���� ��ü
        else
        {
            _items[indexA] = itemB;
            _items[indexB] = itemA;
        }

        // �� ���� ���� ����
        UpdateSlot(indexA, indexB);
    }
    /// <summary> �ش� ������ ������ ��� </summary>
    public override void Use(int index)
    {
        if (_items[index] == null) return;

        // ��� ������ �������� ���
        if (_items[index] is IUsableItem uItem)
        {
            //������ ���
            bool succeeded = uItem.Use();

            if (_items[index] is PortionItem pitem)
            {
                PortionItemData potionData = pitem.GetData() as PortionItemData;
                IPortionState portionState = potionData.GetPortionState();

                int value = potionData.GetValue();

                if (portionState == IPortionState.HP) _playerCtr.RecoveryHP(value);
                else if (portionState == IPortionState.MP) _playerCtr.RecoveryMP(value);
            }

            if (succeeded)
            {
                UpdateSlot(index);
            }
        }
        else if (_items[index] is EquipmentItem eqItem)
        {
            // 1. =>�÷��̾� �÷��̾� ���� ���
            // 2. �÷��̾� => �÷��̾� UI ������Ʈ
            EquipmentItemData equipData = eqItem.GetEequipmentData();
            equipData.Init();
            int equipIndex = (int)equipData.GetEquipState();

            //������ ���� ����ȭ
            if ((EquipState)equipIndex == EquipState.WEAPON)
            {
                _playerCtr.PutOnWeapon(equipData);
            }

        }

    }
    /// <summary> ��� ���� UI�� ���� ���� ���� ������Ʈ </summary>
    public override void UpdateAccessibleStatesAll()
    {
        _quickInvenUI.SetAccessibleSlotRange(_capacity);
    }
    #endregion

    /***********************************************************************
    *                                 Methods 
    ***********************************************************************/
    #region Public Methods
    /// <summary>
    /// �ش� ������ ���� ������ ���� ����
    /// <para/> - �߸��� �ε��� : -1 ����
    /// <para/> - �� ���� : 0 ����
    /// <para/> - �� �� ���� ������ : 1 ����
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
    /// <summary>
    /// ������ �κ��丮 index ��°�� item�� ����
    /// </summary>
    /// <param name="item"> ������ </param>
    /// <param name="index"> ��� </param>
    public bool TrySetItem(Item item, int index)
    {
        if (!IsValidIndex(index)) return false;
        if (item == null) return false;

        _items[index] = item;
        return true;
    }
    /// <summary> �� �� �ִ� �������� ���� ������(A -> B ��������) </summary>
    public void SeparateAmount(int indexA, int indexB, int amount)
    {
        // amount : ���� ��ǥ ����

        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        Item _itemA = _items[indexA];
        Item _itemB = _items[indexB];

        CountableItem _ciA = _itemA as CountableItem;

        // ���� : A ���� - �� �� �ִ� ������ / B ���� - Null
        // ���ǿ� �´� ���, �����Ͽ� ���� B�� �߰�
        if (_ciA != null && _itemB == null)
        {
            _items[indexB] = _ciA.SeperateAndClone(amount);

            UpdateSlot(indexA, indexB);
        }
    }
    /// <summary> �ش� ������ �� �� �ִ� ���������� ���� </summary>
    public bool IsCountableItem(int index) => HasItem(index) && _items[index] is CountableItem;

    /// <summary> Quick => Item Inven  Move </summary>
    public void MoveFromQuickToItemInven(int quickIndex, int iteminvenIndex)
    {
        if (!IsValidIndex(quickIndex)) return;
        if (!_itemInvenMgr.IsValidIndex(iteminvenIndex)) return;

        Item quickItem = _items[quickIndex];
        Item invenItem = _itemInvenMgr.GetItem(iteminvenIndex);

        // 1. �� �� �ִ� �������̰�, ������ �������� ���
        // indexA -> indexB�� ���� ��ġ��
        if (quickItem != null && invenItem != null &&
            quickItem.GetData().GetID() == invenItem.GetData().GetID() &&
            quickItem is CountableItem ciA && invenItem is CountableItem ciB)
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
        // 2. �Ϲ����� ��� : ���� ��ü
        else
        {
            _items[quickIndex] = invenItem;
            _itemInvenMgr.TrySetItem(quickItem, iteminvenIndex);
        }

        // �� ���� ���� ����
        UpdateSlot(quickIndex);
        _itemInvenMgr.UpdateSlot(iteminvenIndex);
    }

    /// <summary> Item Inven  Move => Quick </summary>
    public void MoveFromItemInvenToQuick(int iteminvenIndex, int quickIndex)
    {
        if (!IsValidIndex(quickIndex)) return;
        if (!_itemInvenMgr.IsValidIndex(iteminvenIndex)) return;

        Item quickItem = _items[quickIndex];
        Item invenItem = _itemInvenMgr.GetItem(iteminvenIndex);

        // 1. �� �� �ִ� �������̰�, ������ �������� ���
        // indexA -> indexB�� ���� ��ġ��
        if (quickItem != null && invenItem != null &&
            quickItem.GetData().GetID() == invenItem.GetData().GetID() &&
            quickItem is CountableItem ciA && invenItem is CountableItem ciB)
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
        // 2. �Ϲ����� ��� : ���� ��ü
        else
        {
            _items[quickIndex] = invenItem;
            _itemInvenMgr.TrySetItem(quickItem, iteminvenIndex);
        }

        // �� ���� ���� ����
        UpdateSlot(quickIndex);
        _itemInvenMgr.UpdateSlot(iteminvenIndex);
    }
    #endregion


    #region Private Methods
    /// <summary> �տ������� ����ִ� ���� �ε��� Ž�� </summary>
    private int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < _capacity; i++)
        {
            if (_items[i] == null)
                return i;
        }

        return -1;
    }
    /// <summary> �տ������� ���� ������ �ִ� Countable �������� ���� �ε��� Ž�� </summary>
    private int FindCountableItemSlotIndex(CountableItemData target, int startIndex = 0)
    {
        for (int i = startIndex; i < _capacity; i++)
        {
            var current = _items[i];
            if (current == null)
                continue;

            // ������ ���� ��ġ, ���� ���� Ȯ��
            if (current.GetData().GetID() == target.GetID() && current is CountableItem ci)
            {

                if (!ci.GetIsMax())
                    return i;
            }
        }

        return -1;
    }

    /// <summary> Load Data ������ �ʱ�ȭ </summary>
    private void Init_InvenItems()
    {
        _quickInvenUI = FindObjectOfType<QuickInvenUI>();
        _capacity = _maxCapacity;
        _quickInvenUI.SetInventoryReference(this);

        _items = SaveSys.LoadInvenitem("QuickItem.Json");

        if (_items == null)
        {
            _items = new Item[_maxCapacity];
            return;
        }
        else
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null) continue;
                _items[i].GetData().Init();

                UpdateSlot(i);
            }
        }
    }
    #endregion


}
