﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 소비 아이템 정보 </summary>
[CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion", order = 3)]
public class PortionItemData : CountableItemData
{
    ///</summary> 효과랑(회복량 등) </summary>
    [SerializeField] private float _value;

    public float GetValue() { return _value; }

    public override Item CreateItem()
    {
        return new PortionItem(this);
    }
}