using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Lv01_09 : Monster
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
        var _resourcesData = GameManager.instance.GetResourcesData();

        foreach(var a in m_nDropItemArray)
        {
            var item = _resourcesData.GetItem(a);

            if(item is PortionItemData portionItem)
            {
                int nPortionAmount = RandNum(m_nPortionDrop_MinAmount, m_nPortionDrop_MaxAmount);

                AddDropItem(item, nPortionAmount);
            }
            else
            {
                AddDropItem(item, 1, m_nItemDrop_percentage);
            }
        }

        //AddDropItem(_resourcesData.GetHPportion(), hpPortionAmount);
        //AddDropItem(_resourcesData.GetMPportion(), mpPortionAmount);
        //AddDropItem(_resourcesData.GetMidleAromor(), 1, m_nItemDrop_percentage);
    }
}