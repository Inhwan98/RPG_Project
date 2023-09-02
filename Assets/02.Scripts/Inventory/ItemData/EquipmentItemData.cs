using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItemData : ItemData
{
    /// <summary> 최대 내구도 </summary>
    public int GetMaxDurability() => _maxDurability;

    [SerializeField] private int _maxDurability = 100;
}