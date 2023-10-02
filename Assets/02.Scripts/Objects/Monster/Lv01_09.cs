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

    /// <summary> 몬스터의 드롭아이템 세팅 </summary>
    public void SettingDropItem()
    {
        //아이템 ID가 담긴 배열을 반복하며
        //해당 ID의 아이템들을 드롭아이템으로 가진다.
        foreach(int itemID in m_nDropItemArray)
        {
            ItemData itemdata = _resourcesData.GetItem(itemID);

            if(itemdata is PortionItemData portionItemData)
            {
                //포션의 양 랜덤하게 설정
                int nPortionAmount = RandNum(m_nPortionDrop_MinAmount, m_nPortionDrop_MaxAmount);
                
                AddDropItem(itemdata, nPortionAmount);
            }
            else
            {
                //아이템의 드랍확률에 따라 드랍시킨다.
                AddDropItem(itemdata, 1, m_nItemDrop_percentage);
            }
        }
    }
}
