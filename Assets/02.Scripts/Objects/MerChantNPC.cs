using System;
using System.Collections;
using UnityEngine;

public class MerChantNPC : NPC
{
    private MerChantInventoryManager _merChantInvenMgr;

    private int[] _itemIDArray;


    protected override void Start()
    {
        _merChantInvenMgr = GameManager.instance.GetMerChantInvenMgr();
        base.Start();
    }

    protected override void NPCNameTextInit()
    {
        base.NPCNameTextInit();
        _npcNameTextMesh.transform.GetChild(0).GetComponent<TextMesh>().color = Color.yellow;
    }

    public void MerChantInvenOn()
    {
        foreach (var itemID in _itemIDArray)
        {
            ItemData itemData = _resourcesData.GetItem(itemID);

            _merChantInvenMgr.AddItem(itemData);
        }
        _merChantInvenMgr.SetWindowActive(true);
        _merChantInvenMgr.MerChantInvenON();

    }

    public void MerChantInvenOff()
    {
        _merChantInvenMgr.SetWindowActive(false);
        _merChantInvenMgr.AllRemove();
    }

    protected override void LoadData()
    {
        base.LoadData();

        _itemIDArray = _npcData._itemIDArray;
    }

}
