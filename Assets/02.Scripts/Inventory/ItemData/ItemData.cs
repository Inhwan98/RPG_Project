 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private string _name; //아이템 이름
    [Multiline]
    [SerializeField] private string _tooltip; //아이템 설명
    [SerializeField] private Sprite _iconSprite; //아이템 아이콘
    [SerializeField] private GameObject _dropItemPrefab; //바닥에 떨어질 때 생성할 프리팹

    public int GetID() { return _id; }
    public string GetName() { return _name; }
    public string GetTooltip() { return _tooltip; }
    public Sprite GetIconSprite() { return _iconSprite; }

    ///</summary> 타입에 맞는 새로운 아이템 생성 </summary>
    public abstract Item CreateItem();

}
