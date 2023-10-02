 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _nId;

    [Newtonsoft.Json.JsonProperty]
    private string _sName; //아이템 이름

    [Newtonsoft.Json.JsonProperty]
    private int _nUsedLevel; //제한 레벨

    [Multiline]
    [Newtonsoft.Json.JsonProperty]
    private string _sTooltip; //아이템 설명

    [Newtonsoft.Json.JsonProperty]
    private string _sIconPath;

    private Sprite _iconSprite; //아이템 아이콘
    //[SerializeField] private GameObject _dropItemPrefab; //바닥에 떨어질 때 생성할 프리팹

    public int GetID() { return _nId; }
    public int GetUsedLevel() { return _nUsedLevel; }
    public string GetName() { return _sName; }
    public string GetToolTip() { return _sTooltip; }
    public Sprite GetIconSprite() { return _iconSprite; }

    /// <summary> _sIconPath에 대한 Resources Load </summary>
    public void SetIcon()
    {
        _iconSprite = Resources.Load<Sprite>(_sIconPath);
    }

    ///</summary> 타입에 맞는 새로운 아이템 생성 </summary>
    public abstract Item CreateItem();
}
