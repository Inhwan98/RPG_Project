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

public class PlayerStatManager : BaseItemInvenManager
{
    /***********************************************
     *                   Fields
     ***********************************************/
    #region static Fields
    public static PlayerStatManager instance = null;
    #endregion

    #region private Fields
    [SerializeField, Range(8, 64)]
    private int _maxCapacity = 8; 

    private PlayerEquipUI _playerEquipUI;
    private PlayerInfoUI _playerInfoUI;
    private Item[] _equipItemArray;

    //GameManager Awake()���� �Ҵ�
    private ItemInventoryManager _itemInvenMgr;
    private QuickInvenManager _quickInvenMgr;
    private PlayerController _playerCtr;
    #endregion


    /***********************************************
     *           Get, Set, Update Methods
     ***********************************************/
    #region Set Methods
    /// <summary> PlayerController Awake() ���� ���� ��</summary>
    public override void SetPlayerCtr(PlayerController playerCtr) => _playerCtr = playerCtr;
    public void SetItemInvenMgr(ItemInventoryManager itemInvenMgr) => _itemInvenMgr = itemInvenMgr;
    public void SetQuickInvenMgr(QuickInvenManager quickInvenMgr) => _quickInvenMgr = quickInvenMgr;
    public void SetPlayerStatusData(PlayerStatusData playerStatusData)
    {
        _playerInfoUI.SetPlayerStatusData(playerStatusData);
    }
    #endregion

    #region Player Status / Info
    public void UpdateHPbar(float playerHP, float playerMaxHP) => _playerInfoUI.UpdateHPbar(playerHP, playerMaxHP);
    public void UpdateMPbar(float playerMP, float playerMaxMP) => _playerInfoUI.UpdateMPbar(playerMP, playerMaxMP);
    public void UpdateEXPbar(float playerEXP, float playerMaxEXP) => _playerInfoUI.UpdateEXPbar(playerEXP, playerMaxEXP);
    public void UpdateLevelText(int nLevel) => _playerInfoUI.UpdateLevelText(nLevel);
    #endregion

    /***********************************************
     *                 Unity Evnet
     ***********************************************/
    #region Unity Event
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

    private void OnDestroy()
    {
        instance = null;
        SaveEquipItem();
    }
    #endregion

    /***********************************************
     *              Override Methods
     ***********************************************/
    #region Override Methods
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
        UpdateStatusDisplay();

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
        UpdateStatusDisplay();
        _playerEquipUI.gameObject.SetActive(value);
    }
    ///<summary> �ش� ������ ������ ���� </summary>
    public override void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        if(index == (int)EquipState.WEAPON) PlayerTakeOffWeapon();
        _equipItemArray[index] = null;
        UpdateSlot(index);
    }
    ///<summary>
    /// ��񽽷� => �������κ��丮�� swap �̺�Ʈ
    /// </summary>
    public override void Swap(int equipIndex, int itemIndex)
    {
        //if (invenitem == null) return false;
        Item equipItem = _equipItemArray[equipIndex];
        Item invenItem = _itemInvenMgr.GetItem(itemIndex);

     
        //�������� ���� �Ѵٸ� ���� && �������� ���������� �´ٸ� ����
        if (_itemInvenMgr.HasItem(itemIndex) && (invenItem is EquipmentItem))
        {
            _itemInvenMgr.TrySetItem(equipItem, itemIndex);
            _equipItemArray[equipIndex] = invenItem;
        }
        //�������� ���� ���� �ʴ´ٸ� �̵�
        else
        {
            _itemInvenMgr.TrySetItem(equipItem, itemIndex);
             Remove(equipIndex);
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

    #endregion


    /***********************************************
     *                 Methods
     ***********************************************/
    #region public Methods
    ///<summary>
    /// �������κ��丮 => ��񽽷� swap �̺�Ʈ
    /// </summary>
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
    /// <summary> QuickSlot <=> equipSlot </summary>
    public void QuickSlotSwap(int itemIndex, int equipIndex)
    {
        Item equipItem = _equipItemArray[equipIndex];
        Item invenItem = _quickInvenMgr.GetItem(itemIndex);

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
            _quickInvenMgr.TrySetItem(equipItem, itemIndex);
            _equipItemArray[equipIndex] = invenItem;
        }
        //�������� ���� ���� �ʴ´ٸ� �̵�
        else
        {
            _equipItemArray[equipIndex] = invenItem;
            _quickInvenMgr.Remove(itemIndex);
        }
        //���� ��� UpdateSlot ����
        UpdateSlot(equipIndex);
        _quickInvenMgr.UpdateSlot(itemIndex);
    }

    /// <summary> Item Inven Item �߰� </summary>
    public void AddItemInven(int index)
    {
        ItemData itemdata = GetItemData(index);
        _itemInvenMgr.AddItem(itemdata);
    }

    /// <summary> ������ ��� ���� ĳ���� ���� �߰� </summary>
    public void UpdateState()
    {
        int nTotalAddDefecne = 0;
        int nTotalAddPower = 0;

        foreach (var equipItem in _equipItemArray)
        {
            if (equipItem is ArmorItem armorItem)
            {
                var armorData = armorItem.GetData() as ArmorItemData;
                int nAddDefnce = armorData.GetDefence();

                nTotalAddDefecne += nAddDefnce;

            }
            else if (equipItem is WeaponItem weaponItem)
            {
                var weaponData = weaponItem.GetData() as WeaponItemData;
                int nPower = weaponData.GetDamage();

                nTotalAddPower += nPower;
            }
        }

        _playerCtr.SetAddDefence(nTotalAddDefecne);
        _playerCtr.SetAddSTR(nTotalAddPower);
    }

    #region ConverSation ����
    public void ConversationKeyActiveOn(Vector3 pos) => _playerInfoUI.ConversationKeyActiveOn(pos);
    public void ConversationKeyActiveOff() => _playerInfoUI.ConversationKeyActiveOff();
    #endregion

    //�������ͽ� ���÷��� ������Ʈ
    public void UpdateStatusDisplay()
    {
        UpdateState();
        _playerInfoUI.UpdateStatusDisplay();
    }
    
    /// <summary> ��� ������ �ε� </summary>
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
                _equipItemArray[i].GetData().Init();

                if(i == (int)EquipState.WEAPON)
                {
                    EquipmentItemData equipItemData = _equipItemArray[i].GetData() as EquipmentItemData;
                    PlayerPutOnWeapon(equipItemData);
                }
                UpdateSlot(i);
            }
        }
    }

    /// <summary> Player ���� ���� </summary>
    public void PlayerPutOnWeapon(EquipmentItemData equipData)
    {
        _playerCtr.PutOnWeapon(equipData);
    }

    public void PlayerTakeOffWeapon()
    {
        _playerCtr.TakeOffWeapon();
    }


    public void ReturnToVillage()
    {
        string stayingVilliage = _playerCtr.GetPlayerStayingVilliage();
        LodingSceneController.LoadScene(stayingVilliage);
    }

    public void SaveEquipItem()
    {
        SaveSys.SaveInvenItem(_equipItemArray, "EquipItem.Json");
    }


    #endregion


}
