 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _nId;

    [Newtonsoft.Json.JsonProperty]
    private string _sName; //������ �̸�

    [Newtonsoft.Json.JsonProperty]
    private int _nUsedLevel; //���� ����

    [Multiline]
    [Newtonsoft.Json.JsonProperty]
    private string _sTooltip; //������ ����

    [Newtonsoft.Json.JsonProperty]
    private string _sIconPath;

    private Sprite _iconSprite; //������ ������
    //[SerializeField] private GameObject _dropItemPrefab; //�ٴڿ� ������ �� ������ ������

    public int GetID() { return _nId; }
    public int GetUsedLevel() { return _nUsedLevel; }
    public string GetName() { return _sName; }
    public string GetToolTip() { return _sTooltip; }
    public Sprite GetIconSprite() { return _iconSprite; }

    /// <summary> _sIconPath�� ���� Resources Load </summary>
    public void SetIcon()
    {
        _iconSprite = Resources.Load<Sprite>(_sIconPath);
    }

    ///</summary> Ÿ�Կ� �´� ���ο� ������ ���� </summary>
    public abstract Item CreateItem();
}
