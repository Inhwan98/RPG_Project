 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _id;

    [Newtonsoft.Json.JsonProperty]
    private string _name; //������ �̸�

    [Newtonsoft.Json.JsonProperty]
    private int _usedLevel; //���� ����

    [Multiline]
    [Newtonsoft.Json.JsonProperty]
    private string _tooltip; //������ ����

    [Newtonsoft.Json.JsonProperty]
    private string _IconPath;

    private Sprite _iconSprite; //������ ������
    //[SerializeField] private GameObject _dropItemPrefab; //�ٴڿ� ������ �� ������ ������

    public int GetID() { return _id; }
    public int GetUsedLevel() { return _usedLevel; }
    public string GetName() { return _name; }
    public string GetToolTip() { return _tooltip; }
    public Sprite GetIconSprite() { return _iconSprite; }

    public void SetIcon()
    {
        _iconSprite = Resources.Load<Sprite>(_IconPath);
    }

    ///</summary> Ÿ�Կ� �´� ���ο� ������ ���� </summary>
    public abstract Item CreateItem();
}
