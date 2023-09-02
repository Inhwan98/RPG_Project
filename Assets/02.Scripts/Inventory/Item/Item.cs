using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item
{
    private ItemData m_data;

    public Item(ItemData _data) => this.m_data = _data;

    public ItemData GetData() => m_data;

}