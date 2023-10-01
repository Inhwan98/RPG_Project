using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 장비 - 무기 아이템 </summary>
public class WeaponItemData : EquipmentItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _nDamage = 1;

    /// <summary> 공격력 </summary>
    public int GetDamage() => _nDamage;

    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }
}
