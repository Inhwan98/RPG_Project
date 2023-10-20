using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// *** EquipItem Index ***
/// 
///       HEAD     = 0;
///       CHEST    = 1;
///       GLOVES   = 2;
///       WEAPON   = 3;
///       CLOAK    = 4;
///       SHOULDER = 5;
///       PANTS    = 6;
///       BOOTS    = 7;
///       

public class PlayerUIManager : BaseItemInvenManager
{
    public static PlayerUIManager instance = null;
    
    private PlayerEquipUI _playerEquipUI;
    private PlayerInfoUI _playerInfoUI;

    //GameManager Awake()���� �Ҵ�
    private ItemInventoryManager _itemInvenMgr; 
    private PlayerController _playerCtr;


    Item[] _equipItemArray;

    [SerializeField, Range(8, 64)]
    private int _maxCapacity = 8; 


    /// <summary> PlayerController Awake() ���� ���� ��</summary>
    public override void SetPlayerCtr(PlayerController playerCtr) => _playerCtr = playerCtr;
    public void SetItemInvenMgr(ItemInventoryManager itemInvenMgr) => _itemInvenMgr = itemInvenMgr;

    #region Player Status / Info
    public void SetHPbar(float playerHP, float playerMaxHP) => _playerInfoUI.UpdateHPbar(playerHP, playerMaxHP);
    public void SetMPbar(float playerMP, float playerMaxMP) => _playerInfoUI.UpdateMPbar(playerMP, playerMaxMP);
    public void SetEXPbar(float playerEXP, float playerMaxEXP) => _playerInfoUI.UpdateEXPbar(playerEXP, playerMaxEXP);
    #endregion
    #region ConverSation ����
    public void ConversationKeyActiveOn(Vector3 pos) => _playerInfoUI.ConversationKeyActiveOn(pos);
    public void ConversationKeyActiveOff() => _playerInfoUI.ConversationKeyActiveOff();
    #endregion

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;

        _playerEquipUI = FindObjectOfType<PlayerEquipUI>();
        _playerEquipUI.SetPlayerUIMgr(this);

        _playerInfoUI = FindObjectOfType<PlayerInfoUI>();
        _playerInfoUI.SetPlayerUIMgr(this);

        LoadEquipItem();
    }

    /// <summary> �ε����� ���� ���� ���� �ִ��� �˻� </summary>
    public override bool IsValidIndex(int index) => index >= 0 && index < _equipItemArray.Length;
    /// <summary> �ش� ������ �������� ���� �ִ��� ���� </summary>
    public override bool HasItem(int index) => IsValidIndex(index) && _equipItemArray[index] != null;
    /// <summary> �ش� ������ ������ �̸� ���� </summary>
    public override string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (_equipItemArray[index] == null) return "";

        return _equipItemArray[index].GetData().GetName();
    }
    /// <summary> �ش� ������ ��ȯ </summary>
    public override Item GetItem(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (_equipItemArray[index] == null) return null;

        return _equipItemArray[index];
    }
    /// <summary> �ش� ������ ������ ���� ���� </summary>
    public override ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (_equipItemArray[index] == null) return null;

        return _equipItemArray[index].GetData();
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

        Item item = _equipItemArray[index];
        
        // 1. �������� ���Կ� �����ϴ� ���
        if (item != null)
        {
            //������ ���
            _playerEquipUI.SetItemIcon(index, item.GetData().GetIconSprite());
            
        }
        //2. �� ������ ��� : ������ ����
        else
        {
            RemoveIcon();
        }

        // ���� : ������ �����ϱ�
        void RemoveIcon()
        {
            _playerEquipUI.RemoveItem(index);
        }
    }
    /// <summary> ��� ���Ե��� ���¸� UI�� ���� </summary>
    public override void UpdateAllSlot()
    {
        for (int i = 0; i < _maxCapacity; i++)
        {
            UpdateSlot(i);
        }
    }
    public override int AddItem(ItemData itemData, int amount = 1)
    {
        throw new System.NotImplementedException();
    }
    /// <summary> Inventory Ȱ��ȭ ���� </summary>
    public override void SetWindowActive(bool value)
    {
        _playerCtr.SetUseSKill_Inven(value); //�÷��̾��� ������ ����
        _playerInfoUI.StatusDisplay();
        _playerEquipUI.gameObject.SetActive(value);
    }
    ///<summary> �ش� ������ ������ ���� </summary>
    public override void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        _equipItemArray[index] = null;
        UpdateSlot(index);
    }
    ///<summary>
    /// �÷��̾� ������ ��� ����
    /// </summary>
    public override void Swap(int equipIndex, int itemIndex)
    {
        //if (invenitem == null) return false;
        Item equipItem = _equipItemArray[equipIndex];
        Item invenItem = _itemInvenMgr.GetItem(itemIndex);

        if (!(invenItem is EquipmentItem)) return; 
        
        //�������� ���� �Ѵٸ� ����
        if (_itemInvenMgr.HasItem(itemIndex))
        {
            _itemInvenMgr.TrySetItem(equipItem, itemIndex);
            _equipItemArray[equipIndex] = invenItem;
        }
        //�������� ���� ���� �ʴ´ٸ� �̵�
        else
        {
            if (equipItem == null) Debug.Log("is null");
            _itemInvenMgr.TrySetItem(equipItem, itemIndex);


             Remove(equipIndex);
        }
        //���� ��� UpdateSlot ����
        UpdateSlot(equipIndex);
        _itemInvenMgr.UpdateSlot(itemIndex);
    }

    public void ReverseSwap(int itemIndex, int equipIndex)
    {
        Item equipItem = _equipItemArray[equipIndex];
        Item invenItem = _itemInvenMgr.GetItem(itemIndex);

        if (invenItem is EquipmentItem eqItem)
        {
            bool isSameEquipSate = equipIndex == (int)eqItem.GetEequipmentData().GetEquipState();
            //��񽽷԰� �����ϴ� �������� ������ �ٸ��� ����(����) x (ex - ���� and ��)
            if (!isSameEquipSate) return;
        }
        else
        {
            //��񽽷����� �ű�� �ϴ� �������� ��� �ƴϸ� return
            return;
        }
        
        
        //�������� ���� �Ѵٸ� ����
        if (HasItem(equipIndex))
        {    
            _itemInvenMgr.TrySetItem(equipItem, itemIndex);
            _equipItemArray[equipIndex] = invenItem;
        }
        //�������� ���� ���� �ʴ´ٸ� �̵�
        else
        {
            _equipItemArray[equipIndex] = invenItem;
            _itemInvenMgr.Remove(itemIndex);
        }
        //���� ��� UpdateSlot ����
        UpdateSlot(equipIndex);
        _itemInvenMgr.UpdateSlot(itemIndex);
    }

    public override void Use(int index)
    {
        throw new System.NotImplementedException();
    }
    public override void UpdateAccessibleStatesAll()
    {

    }

    public void AddItemInven(int index)
    {
        ItemData itemdata = GetItemData(index);
        _itemInvenMgr.AddItem(itemdata);
    }

    public void LoadEquipItem()
    {
        _equipItemArray = SaveSys.LoadInvenitem("EquipItem.Json");

        if (_equipItemArray == null)
        {
            _equipItemArray = new Item[_maxCapacity];
            return;
        }
        else
        {
            for (int i = 0; i < _equipItemArray.Length; i++)
            {
                if (_equipItemArray[i] == null) continue;
                _equipItemArray[i].GetData().SetIcon();
                UpdateSlot(i);
            }
        }
    }

    public void SaveEquipItem()
    {
        SaveSys.SaveInvenItem(_equipItemArray, "EquipItem.Json");
    }

    public void SetPlayerStatusData(PlayerStatusData playerStatusData)
    {
        _playerInfoUI.SetPlayerStatusData(playerStatusData);
    }

    private void OnDestroy()
    {
        instance = null;
        SaveEquipItem();
    }
}
