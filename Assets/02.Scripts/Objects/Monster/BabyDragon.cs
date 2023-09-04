using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BabyDragon : Monster
{
    

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        //Getinfo();
    }

    protected override void Die()
    {
        List<ItemData> itemDatas = new List<ItemData>();
        itemDatas.Add(_resourcesData.GetHPportion());
        playerCtr.AddInven(itemDatas);
        

        base.Die();
    }


}
