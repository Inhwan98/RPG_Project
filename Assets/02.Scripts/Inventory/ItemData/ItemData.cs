 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _id;

    [Newtonsoft.Json.JsonProperty]
    private string _name; //아이템 이름

    [Multiline]
    [Newtonsoft.Json.JsonProperty]
    private string _tooltip; //아이템 설명

    [Newtonsoft.Json.JsonProperty]
    private string _IconPath;

    private Sprite _iconSprite; //아이템 아이콘
    //[SerializeField] private GameObject _dropItemPrefab; //바닥에 떨어질 때 생성할 프리팹

    public int GetID() { return _id; }
    public string GetName() { return _name; }
    public string GetToolTip() { return _tooltip; }
    public Sprite GetIconSprite() { return _iconSprite; }

    public void SetIcon()
    {
        _iconSprite = Resources.Load<Sprite>(_IconPath);
    }

    ///</summary> 타입에 맞는 새로운 아이템 생성 </summary>
    public abstract Item CreateItem();
}
