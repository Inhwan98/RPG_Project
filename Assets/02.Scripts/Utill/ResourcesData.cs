using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourcesData
{
    private ItemData[] _portionDatas;
    private ItemData[] _aromorDatas;

    private TMP_Text _questHeader;
    private TMP_Text _questContext;

    public ResourcesData()
    {
        Init();
    }

    public void Init()
    {
        //_HPportion = SaveSys.LoadItem<PortionItemData>("Item_Portion_HP.Json");
        //_MPportion = SaveSys.LoadItem<PortionItemData>("Item_Portion_MP.Json");
        //_MidleAromor = SaveSys.LoadItem<ArmorItemData>("Item_Armor_Middle.Json");
        _portionDatas = SaveSys.LoadAllData().PortionItemDB;
        _aromorDatas = SaveSys.LoadAllData().ArmorItemDB;

        _questHeader = Resources.Load<TMP_Text>("Prefab/UI/Header");
        _questContext = Resources.Load<TMP_Text>("Prefab/UI/Context");
    }

    //public ItemData GetHPportion()   { return _HPportion; }
    //public ItemData GetMPportion()   { return _MPportion; }
    //public ItemData GetMidleAromor() { return _MidleAromor; }

    public List<ItemData> GetItemList(int[] _nIDArray)
    {
        int index;

        List<ItemData> itemDatas = new List<ItemData>();

        foreach(int nID in _nIDArray)
        {
            if (nID >= 1000 && nID < 2000)
            {
                index = nID - 1000;
                itemDatas.Add(_portionDatas[index]);
            }
            else if (nID >= 2000 && nID < 3000)
            {
                index = nID - 2000;
                itemDatas.Add(_aromorDatas[index]);
            }
        }

        return itemDatas;
    }

    public ItemData GetItem(int nID)
    {
        int index = 0;

        if (nID >= 1000 && nID < 2000)
        {
            index = nID - 1000;
            _portionDatas[index].SetIcon();
            return _portionDatas[index];
        }
        else if(nID >= 2000 && nID < 3000)
        {
            index = nID - 2000;
            _aromorDatas[index].SetIcon();
            return _aromorDatas[index];
        }

        return null;
    }

    public TMP_Text GetQuestHeader() => _questHeader;
    public TMP_Text GetQuestConText() => _questContext;
}