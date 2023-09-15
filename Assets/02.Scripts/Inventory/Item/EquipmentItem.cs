using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> 장비 아이템</summary>
public abstract class EquipmentItem : Item
{
    [Newtonsoft.Json.JsonProperty]
    private int _durability; // 내구도

    [Newtonsoft.Json.JsonProperty]
    private EquipmentItemData equipmentData;


    public void SetEequipmentData(EquipmentItemData value) => equipmentData = value;
    public EquipmentItemData GetEequipmentData() => equipmentData;

    /// <summary> 현재 내구도 </summary>
    public int GetDurability() => _durability;
    public void SetDurability(int value)
    {
        if (value < 0) value = 0;
        if (value > equipmentData.GetMaxDurability())
            value = equipmentData.GetMaxDurability();

        _durability = value;
    }

    public EquipmentItem(EquipmentItemData data) : base(data)
    {
        if (data == null) return;

        Type = "EquipmentItem";
        equipmentData = data;

        _durability = data.GetMaxDurability();
        Debug.Log("data Not null");
    }

    // Item Data 외의 필드값에 대한 매개변수를 갖는 생성자는 추가로 제공하지 않음
    // 자식들에서 모두 추가해줘야 하므로 유지보수면에서 불편
}