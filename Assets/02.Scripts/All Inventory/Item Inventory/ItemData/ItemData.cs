 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
        [상속 구조]

        ItemData(abstract)
            - CountableItemData(abstract)
                - PortionItemData
            - EquipmentItemData(abstract)
                - WeaponItemData
                - ArmorItemData

    */


[System.Serializable]
public abstract class ItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _nId;

    [Newtonsoft.Json.JsonProperty]
    private string _sName; //아이템 이름

    [Newtonsoft.Json.JsonProperty]
    private int _nPrice; //아이템 가격

    [Newtonsoft.Json.JsonProperty]
    private int _nUsedLevel; //제한 레벨

    [Multiline]
    [Newtonsoft.Json.JsonProperty]
    private string _sTooltip; //아이템 설명

    [Newtonsoft.Json.JsonProperty]
    private string _sIconPath;


    private Sprite _iconSprite; //아이템 아이콘
    //private GameObject _dropItemPrefab; //착용할 프리팹

    private int _returnPrice;

    public int GetID() { return _nId; }
    public int GetPrice() { return _nPrice; }
    public int GetUsedLevel() { return _nUsedLevel; }
    public string GetName() { return _sName; }
    public string GetToolTip() { return _sTooltip; }
    public Sprite GetIconSprite() { return _iconSprite; }

    /// <summary>
    /// 되팔기 가격은 원래 가격의 70% 이다.
    /// </summary>
    public int GetReturnPrice()
    {
        _returnPrice = Mathf.RoundToInt(_nPrice * 0.7f);  // 가장 가까운 정수로 반올림하여 int로 변환

        return _returnPrice;
    }

    /// <summary> _sIconPath에 대한 Resources Load </summary>
    public virtual void Init()
    {
        _iconSprite = Resources.Load<Sprite>(_sIconPath);
    }

    ///</summary> 타입에 맞는 새로운 아이템 생성 </summary>
    public abstract Item CreateItem();
}
