 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
        [��� ����]

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
    private string _sName; //������ �̸�

    [Newtonsoft.Json.JsonProperty]
    private int _nPrice; //������ ����

    [Newtonsoft.Json.JsonProperty]
    private int _nUsedLevel; //���� ����

    [Multiline]
    [Newtonsoft.Json.JsonProperty]
    private string _sTooltip; //������ ����

    [Newtonsoft.Json.JsonProperty]
    private string _sIconPath;


    private Sprite _iconSprite; //������ ������
    //private GameObject _dropItemPrefab; //������ ������

    private int _returnPrice;

    public int GetID() { return _nId; }
    public int GetPrice() { return _nPrice; }
    public int GetUsedLevel() { return _nUsedLevel; }
    public string GetName() { return _sName; }
    public string GetToolTip() { return _sTooltip; }
    public Sprite GetIconSprite() { return _iconSprite; }

    /// <summary>
    /// ���ȱ� ������ ���� ������ 70% �̴�.
    /// </summary>
    public int GetReturnPrice()
    {
        _returnPrice = Mathf.RoundToInt(_nPrice * 0.7f);  // ���� ����� ������ �ݿø��Ͽ� int�� ��ȯ

        return _returnPrice;
    }

    /// <summary> _sIconPath�� ���� Resources Load </summary>
    public virtual void Init()
    {
        _iconSprite = Resources.Load<Sprite>(_sIconPath);
    }

    ///</summary> Ÿ�Կ� �´� ���ο� ������ ���� </summary>
    public abstract Item CreateItem();
}
