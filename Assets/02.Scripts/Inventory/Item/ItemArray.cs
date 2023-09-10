using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemArray
{
    [Newtonsoft.Json.JsonProperty]
    private Item[] _item;

    public ItemArray(Item[] __items)
    {
        var size = __items.Length;
        this._item = new Item[size];
        
        for(int i = 0; i < size; i++)
        {

            _item[i] = __items[i];
        }

    }
    public Item[] Getitems() => _item;

}