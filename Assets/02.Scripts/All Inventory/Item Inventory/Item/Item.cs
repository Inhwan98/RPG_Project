using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    [상속 구조]
    Item (abstract): 기본 아이템
        - EquipmentItem (abstract): 장비 아이템
            - ArmorItem  : 방어구 아이템
            - WeaponItem : 무기 아이템

        - CountableItem (abstract): 수량이 존재하는 아이템
            - PortionItem : 포션 아이템
*/

[System.Serializable]
public abstract class Item
{
    [Newtonsoft.Json.JsonProperty]
    private ItemData m_data;

    public Item(ItemData _data)
    {
        this.m_data = _data;
    }

    public ItemData GetData() => m_data;
    public void SetData(ItemData value) => m_data = value;

}