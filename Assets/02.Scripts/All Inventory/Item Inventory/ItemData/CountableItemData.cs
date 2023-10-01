using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CountableItemData : ItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _nMaxAmount = 99;

    public int GetMaxAmount() { return _nMaxAmount; }

}
