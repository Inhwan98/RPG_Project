using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesData
{ 
    private ItemData _HPportion;
    private ItemData _MPportion;
    private ItemData _MidleAromor;

    public ResourcesData()
    {
        Init();
    }

    public void Init()
    {
        //_HPportion = Resources.Load<ItemData>("Data/Item/Portion/Item_Portion_HP");
        //_MPportion = Resources.Load<ItemData>("Data/Item/Portion/Item_Portion_MP");
        //_MidleAromor = Resources.Load<ItemData>("Data/Item/Armor/Item_Armor_Middle");

        _HPportion = SaveSys.LoadItem<PortionItemData>("Item_Portion_HP");
        _MPportion = SaveSys.LoadItem<PortionItemData>("Item_Portion_MP");
        _MidleAromor = SaveSys.LoadItem<ArmorItemData>("Item_Armor_Middle");

    }

    public ItemData GetHPportion()   { return _HPportion; }
    public ItemData GetMPportion()   { return _MPportion; }
    public ItemData GetMidleAromor() { return _MidleAromor; }
}