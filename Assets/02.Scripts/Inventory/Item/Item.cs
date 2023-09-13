using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    [Newtonsoft.Json.JsonProperty]
    protected string Type;

    [Newtonsoft.Json.JsonProperty]
    private ItemData m_data;

    public Item(ItemData _data)
    {
        Type = "Item";
        this.m_data = _data;
    }

    public ItemData GetData() => m_data;
    public void SetData(ItemData value) => m_data = value;

}