using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary> 수량을 셀 수 있는 아이템 </summary>

public abstract class CountableItem : Item
{
    [Newtonsoft.Json.JsonProperty]
    private CountableItemData m_countableData;

    [Newtonsoft.Json.JsonProperty]
    protected int m_nAmount; // 현재 아이템 개수

    [Newtonsoft.Json.JsonProperty]
    private bool m_bIsMax; // 수량이 가득 찼는지 여부

    public int GetAmount() => m_nAmount;
    public void SetAmount(int nAmount) => m_nAmount = nAmount;

    /// <summary> 하나의 슬롯이 가질 수 있는 최대 개수 (기본 99) </summary>
    public int GetMaxAmount() => m_countableData.GetMaxAmount();

    public CountableItemData GetCountableItemData() => m_countableData;
    public void SetCountableItemData(CountableItemData countableData) => m_countableData = countableData;

    /// <summary>수량이 가득 찼는지 여부 (기본 99) </summary>
    public bool GetIsMax() => m_nAmount >= GetMaxAmount();

    /// <summary> 개수가 없는지 여부 </summary>
    public bool IsEmpty() => m_nAmount <= 0;

    public CountableItem(CountableItemData data, int amount = 1) : base(data)
    {
        Type = "CountableItem";
        m_countableData = data;
        SetAmount(amount);
    }

    /// <summary> 개수 지정(범위 제한) </summary>
    public void SetClampAmount(int amount)
    {
        m_nAmount = Mathf.Clamp(amount, 0, this.GetMaxAmount());
    }

    /// <summary> 개수 추가 및 최대치 초과량 반환(초과량 없을 경우 0) </summary>
    public int AddAmountAndGetExcess(int amount)
    {
        int nextAmount = m_nAmount + amount;
        SetClampAmount(nextAmount);

        return (nextAmount > GetMaxAmount()) ? (nextAmount - GetMaxAmount()) : 0;
    }

    public CountableItem SeperateAndClone(int nAmount)
    {
        //수량이 한개 이하일 경우, 복제 불가
        if (m_nAmount <= 1) return null;

        if (nAmount > m_nAmount - 1)
            nAmount = m_nAmount - 1;

        m_nAmount -= nAmount;
        return Clone(nAmount);
    }

    protected abstract CountableItem Clone(int amount);
}
