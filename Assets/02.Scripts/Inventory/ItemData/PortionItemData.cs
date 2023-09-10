using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IPortionState
{
    HP,
    MP
}

/// <summary> 소비 아이템 정보 </summary>
public class PortionItemData : CountableItemData
{
    ///</summary> 효과랑(회복량 등) </summary>
    [Newtonsoft.Json.JsonProperty]
    private float _value;

    [Newtonsoft.Json.JsonProperty]
    private IPortionState _portionState;

    public float GetValue() { return _value; }
    public IPortionState GetPortionState() { return _portionState; }

    public override Item CreateItem()
    {
        return new PortionItem(this);
    }
}