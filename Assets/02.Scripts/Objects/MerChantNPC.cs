using System;
using System.Collections;
using UnityEngine;

public class MerChantNPC : NPC
{
    /****************************************
     *              Fields
     ****************************************/

    private MerChantInventoryManager _merChantInvenMgr;
    /// <summary> 상인이 가지고 있을 아이템 ID 목록 </summary>
    private int[] _itemIDArray;

    /****************************************
     *             Unity Event
     ****************************************/
    #region Unity Event
    protected override void Start()
    {
        _merChantInvenMgr = GameManager.instance.GetMerChantInvenMgr();
        base.Start();
    }
    #endregion

    /****************************************
     *             Methods
     ****************************************/

    #region override Methods
    protected override void NPCNameTextInit()
    {
        base.NPCNameTextInit();
        _npcNameTextMesh.transform.GetChild(0).GetComponent<TextMesh>().color = Color.yellow;
    }
    /// <summary> NPC가 로드될때, 상인 ItemID 목록도 같이 로드 </summary>
    protected override void LoadData()
    {
        base.LoadData();

        _itemIDArray = _npcData._itemIDArray;
    }
    #endregion

    #region pulbic Methods
    /// <summary> 상인이 활성화 될 때, 상인이 가진 아이템을 새로 추가 해준 뒤 활성화 한다. </summary>
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
    /// <summary> 상인이 비활성화 될 때, 아이템 목록을 비워 준다.</summary>
    public void MerChantInvenOff()
    {
        _merChantInvenMgr.SetWindowActive(false);
        _merChantInvenMgr.AllRemove();
    }
    #endregion

}
