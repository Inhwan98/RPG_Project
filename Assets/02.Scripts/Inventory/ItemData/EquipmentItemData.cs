using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItemData : ItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _maxDurability = 100;

    /// <summary> 최대 내구도 </summary>
    public int GetMaxDurability() => _maxDurability;
}
