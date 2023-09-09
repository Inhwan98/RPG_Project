using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> 장비 - 무기 아이템 </summary>
public class WeaponItemData : EquipmentItemData
{
    /// <summary> 공격력 </summary>
    public int Damage => _damage;

    [SerializeField] private int _damage = 1;

    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }
}
