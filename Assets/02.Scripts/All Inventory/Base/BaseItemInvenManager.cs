using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public abstract class BaseItemInvenManager : BaseInvenManager
{
    public abstract ItemData GetItemData(int index);
    /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
    
    public abstract Item GetItem(int index);

    public abstract int AddItem(ItemData itemData, int amount = 1);
    /// <summary> 해당 슬롯의 아이템 사용 </summary>
    public abstract void Use(int index);
}