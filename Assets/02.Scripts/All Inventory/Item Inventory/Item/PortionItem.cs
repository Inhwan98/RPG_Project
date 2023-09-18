using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 수량 아이템 - 포션 아이템 </summary>
public class PortionItem : CountableItem, IUsableItem
{
    public PortionItem(PortionItemData data, int amount = 1) : base(data, amount) { Type = "PortionItem"; }

    public bool Use()
    {
        // 임시 : 개수 하나 감소
        m_nAmount--;

        return true;
    }

    protected override CountableItem Clone(int amount)
    {
        return new PortionItem(this.GetCountableItemData() as PortionItemData, amount);
    }
}