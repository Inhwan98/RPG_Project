using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv01_09 : Monster
{
    protected override void Awake()
    {
        m_nPortionDrop_MinAmount = 1;
        m_nPortionDrop_MaxAmount = 9;
        m_nItemDrop_percentage = 80;
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        SettingDropItem();
    }

    public int RandNum(int minAmount, int maxAmount)
    {
        int a = Random.Range(minAmount, maxAmount);
        return a;
    }

    public void SettingDropItem()
    {
        int hpPortionAmount = RandNum(m_nPortionDrop_MinAmount, m_nPortionDrop_MaxAmount);
        int mpPortionAmount = RandNum(m_nPortionDrop_MinAmount, m_nPortionDrop_MaxAmount);

        var _resourcesData = GameManager.instance.GetResourcesData();
        AddDropItem(_resourcesData.GetHPportion(), hpPortionAmount);
        AddDropItem(_resourcesData.GetMPportion(), mpPortionAmount);
        AddDropItem(_resourcesData.GetMidleAromor(), 1, m_nItemDrop_percentage);
    }
}