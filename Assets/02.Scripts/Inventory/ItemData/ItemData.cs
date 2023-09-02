 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private string _name; //������ �̸�
    [Multiline]
    [SerializeField] private string _tooltip; //������ ����
    [SerializeField] private Sprite _iconSprite; //������ ������
    [SerializeField] private GameObject _dropItemPrefab; //�ٴڿ� ������ �� ������ ������

    public int GetID() { return _id; }
    public string GetName() { return _name; }
    public string GetTooltip() { return _tooltip; }
    public Sprite GetIconSprite() { return _iconSprite; }

    ///</summary> Ÿ�Կ� �´� ���ο� ������ ���� </summary>
    public abstract Item CreateItem();

}
