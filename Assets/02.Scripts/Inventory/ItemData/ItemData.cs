 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemData
{
    [SerializeField] private int _id;
    [SerializeField] private string _name; //������ �̸�
    [Multiline]
    [SerializeField] private string _tooltip; //������ ����
    [SerializeField] private string _IconPath;

    [SerializeField] private Sprite _iconSprite; //������ ������
    //[SerializeField] private GameObject _dropItemPrefab; //�ٴڿ� ������ �� ������ ������

    public int GetID() { return _id; }
    public string GetName() { return _name; }
    public string GetTooltip() { return _tooltip; }
    public Sprite GetIconSprite() { return _iconSprite; }

    public void SetIcon()
    {
        _iconSprite = Resources.Load<Sprite>(_IconPath);
    }

    ///</summary> Ÿ�Կ� �´� ���ο� ������ ���� </summary>
    public abstract Item CreateItem();
}
