using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BabyDragon : Lv01_09
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
        playerCtr.AddInven(_itemDic);

        base.Die();
    }


}
