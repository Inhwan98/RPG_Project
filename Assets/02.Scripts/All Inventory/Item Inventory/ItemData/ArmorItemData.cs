using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> 장비 - 방어구 아이템 </summary>
public class ArmorItemData : EquipmentItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _nDefence = 1;

    /// <summary> 방어력 </summary>
    public int GetDefence() => _nDefence;


    public override Item CreateItem()
    {
        return new ArmorItem(this);
    }
}

