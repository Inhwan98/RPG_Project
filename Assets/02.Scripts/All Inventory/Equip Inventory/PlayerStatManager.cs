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
    public static PlayerStatManager instance = null;
    
    private PlayerEquipUI _playerEquipUI;
    private PlayerInfoUI _playerInfoUI;

    //GameManager Awake()에서 할당
    private ItemInventoryManager _itemInvenMgr; 
    private PlayerController _playerCtr;

    Item[] _equipItemArray;

    [SerializeField, Range(8, 64)]
    private int _maxCapacity = 8; 


    /// <summary> PlayerController Awake() 에서 참조 됌</summary>
    public override void SetPlayerCtr(PlayerController playerCtr) => _playerCtr = playerCtr;
    public void SetItemInvenMgr(ItemInventoryManager itemInvenMgr) => _itemInvenMgr = itemInvenMgr;

    #region Player Status / Info
    public void UpdateHPbar(float playerHP, float playerMaxHP) => _playerInfoUI.UpdateHPbar(playerHP, playerMaxHP);
    public void UpdateMPbar(float playerMP, float playerMaxMP) => _playerInfoUI.UpdateMPbar(playerMP, playerMaxMP);
    public void UpdateEXPbar(float playerEXP, float playerMaxEXP) => _playerInfoUI.UpdateEXPbar(playerEXP, playerMaxEXP);
    public void UpdateLevelText(int nLevel) => _playerInfoUI.UpdateLevelText(nLevel);
    #endregion
    #region ConverSation 관련
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

    /// <summary> 인덱스가 수용 범위 내에 있는지 검사 </summary>
    public override bool IsValidIndex(int index) => index >= 0 && index < _equipItemArray.Length;
    /// <summary> 해당 슬롯이 아이템을 갖고 있는지 여부 </summary>
    public override bool HasItem(int index) => IsValidIndex(index) && _equipItemArray[index] != null;
    /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
    public override string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (_equipItemArray[index] == null) return "";

        return _equipItemArray[index].GetData().GetName();
    }
    /// <summary> 해당 아이템 반환 </summary>
    public override Item GetItem(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (_equipItemArray[index] == null) return null;

        return _equipItemArray[index];
    }
    /// <summary> 해당 슬롯의 아이템 정보 리턴 </summary>
    public override ItemData GetItemData(int index)
    {
        if (!IsValidIndex(index)) return null;
        if (_equipItemArray[index] == null) return null;

        return _equipItemArray[index].GetData();
    }
    /// <summary> 해당하는 인덱스의 슬롯들의 상태 및 UI 갱신 </summary>
    public override void UpdateSlot(params int[] indices)
    {
        foreach (var i in indices)
        {
            UpdateSlot(i);
        }
    }
    /// <summary> 해당하는 인덱스의 슬롯 상태 및 UI 갱신 </summary>
    public override void UpdateSlot(int index)
    {
        if (!IsValidIndex(index)) return;

        Item item = _equipItemArray[index];
        
        // 1. 아이템이 슬롯에 존재하는 경우
        if (item != null)
        {
            //아이콘 등록
            _playerEquipUI.SetItemIcon(index, item.GetData().GetIconSprite());
            
        }
        //2. 빈 슬롯인 경우 : 아이콘 제거
        else
        {
            RemoveIcon();
        }
        UpdateStatusDisplay();

        // 로컬 : 아이콘 제거하기
        void RemoveIcon()
        {
            _playerEquipUI.RemoveItem(index);
        }
    }
    /// <summary> 모든 슬롯들의 상태를 UI에 갱신 </summary>
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
    /// <summary> Inventory 활성화 유무 </summary>
    public override void SetWindowActive(bool value)
    {
        _playerCtr.SetUseSKill_Inven(value); //플레이어의 움직임 제어
        UpdateStatusDisplay();
        _playerEquipUI.gameObject.SetActive(value);
    }
    ///<summary> 해당 슬롯의 아이템 제거 </summary>
    public override void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        if(index == (int)EquipState.WEAPON) PlayerTakeOffWeapon();
        _equipItemArray[index] = null;
        UpdateSlot(index);
    }
    ///<summary>
    /// 장비슬롯 => 아이템인벤토리의 swap 이벤트
    /// </summary>
    public override void Swap(int equipIndex, int itemIndex)
    {
        //if (invenitem == null) return false;
        Item equipItem = _equipItemArray[equipIndex];
        Item invenItem = _itemInvenMgr.GetItem(itemIndex);

     
        //아이템이 존재 한다면 스왑 && 아이템이 장비아이템이 맞다면 스왑
        if (_itemInvenMgr.HasItem(itemIndex) && !(invenItem is EquipmentItem))
        {
            _itemInvenMgr.TrySetItem(equipItem, itemIndex);
            _equipItemArray[equipIndex] = invenItem;
        }
        //아이템이 존재 하지 않는다면 이동
        else
        {
            _itemInvenMgr.TrySetItem(equipItem, itemIndex);


             Remove(equipIndex);
        }
        //양쪽 모두 UpdateSlot 진행
        UpdateSlot(equipIndex);
        _itemInvenMgr.UpdateSlot(itemIndex);
    }

    ///<summary>
    /// 아이템인벤토리 => 장비슬롯 swap 이벤트
    /// </summary>
    public void ReverseSwap(int itemIndex, int equipIndex)
    {
        Item equipItem = _equipItemArray[equipIndex];
        Item invenItem = _itemInvenMgr.GetItem(itemIndex);

        if (invenItem is EquipmentItem eqItem)
        {
            bool isSameEquipSate = equipIndex == (int)eqItem.GetEequipmentData().GetEquipState();
            //장비슬롯과 전달하는 아이템의 종류가 다르면 스왑(장착) x (ex - 무기 and 방어구)
            if (!isSameEquipSate) return;
        }
        else
        {
            //장비슬롯으로 옮기려 하는 아이템이 장비가 아니면 return
            return;
        }
        
        
        //아이템이 존재 한다면 스왑
        if (HasItem(equipIndex))
        {    
            _itemInvenMgr.TrySetItem(equipItem, itemIndex);
            _equipItemArray[equipIndex] = invenItem;
        }
        //아이템이 존재 하지 않는다면 이동
        else
        {
            _equipItemArray[equipIndex] = invenItem;
            _itemInvenMgr.Remove(itemIndex);
        }
        //양쪽 모두 UpdateSlot 진행
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

    /// <summary>
    /// 장착한 장비에 따라 캐릭터 스탯 추가
    /// </summary>
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

    //스테이터스 디스플레이 업데이트
    public void UpdateStatusDisplay()
    {
        UpdateState();
        _playerInfoUI.UpdateStatusDisplay();
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
