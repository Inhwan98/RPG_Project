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
        _HPportion = Resources.Load<ItemData>("Item_Portion_HP");
        _MPportion = Resources.Load<ItemData>("Item_Portion_MP");
        _MidleAromor = Resources.Load<ItemData>("Item_Armor_Middle");
    }

    public ItemData GetHPportion()   { return _HPportion; }
    public ItemData GetMPportion()   { return _MPportion; }
    public ItemData GetMidleAromor() { return _MidleAromor; }
}