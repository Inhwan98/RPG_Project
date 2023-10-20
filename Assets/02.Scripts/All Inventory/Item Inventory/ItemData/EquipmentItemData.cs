using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipState
{
    HEAD,
    CHEST,
    GLOVES,
    WEAPON,
    CLOAK,
    SHOULDER,
    PANTS,
    BOOTS
}


public abstract class EquipmentItemData : ItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _maxDurability = 100;

    [Newtonsoft.Json.JsonProperty]
    private EquipState _eEquipState;

    /// <summary> 최대 내구도 </summary>
    public int GetMaxDurability() => _maxDurability;
    public EquipState GetEquipState() => _eEquipState;
}
