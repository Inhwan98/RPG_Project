using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IPortionState
{
    HP,
    MP
}

/// <summary> 소비 아이템 정보 </summary>
[CreateAssetMenu(fileName = "Item_Portion_", menuName = "Inventory System/Item Data/Portion", order = 3)]
public class PortionItemData : CountableItemData
{
    ///</summary> 효과랑(회복량 등) </summary>
    [SerializeField] private float _value;
    [SerializeField] private IPortionState _portionState;

    public float GetValue() { return _value; }
    public IPortionState GetPortionState() { return _portionState; }

    public override Item CreateItem()
    {
        return new PortionItem(this);
    }
}