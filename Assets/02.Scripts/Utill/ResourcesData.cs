using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourcesData
{
    private ItemData _HPportion;
    private ItemData _MPportion;
    private ItemData _MidleAromor;

    private TMP_Text _questHeader;
    private TMP_Text _questContext;

    public ResourcesData()
    {
        Init();
    }

    public void Init()
    {
        _HPportion = SaveSys.LoadItem<PortionItemData>("Item_Portion_HP.Json");
        _MPportion = SaveSys.LoadItem<PortionItemData>("Item_Portion_MP.Json");
        _MidleAromor = SaveSys.LoadItem<ArmorItemData>("Item_Armor_Middle.Json");

        _questHeader = Resources.Load<TMP_Text>("Prefab/UI/Header");
        _questContext = Resources.Load<TMP_Text>("Prefab/UI/Context");
    }

    public ItemData GetHPportion()   { return _HPportion; }
    public ItemData GetMPportion()   { return _MPportion; }
    public ItemData GetMidleAromor() { return _MidleAromor; }

    public TMP_Text GetQuestHeader() => _questHeader;
    public TMP_Text GetQuestConText() => _questContext;
}