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
    [SerializeField]
    private int _nValue;

    [Newtonsoft.Json.JsonProperty]
    [SerializeField]
    private IPortionState _ePortionState;

    public int GetValue() { return _nValue; }
    public IPortionState GetPortionState() { return _ePortionState; }

    public override Item CreateItem()
    {
        return new PortionItem(this);
    }
}