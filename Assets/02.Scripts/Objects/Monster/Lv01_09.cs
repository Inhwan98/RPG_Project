using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv01_09 : Monster
{
    Dictionary<ItemData, int> dictionary = new Dictionary<ItemData, int>();
    
    protected int _amount;

    protected override void Start()
    {
        base.Start();
    }

    public void RandNum(int inputNum)
    {
        int a = Random.Range(0, inputNum + 1);
    }
}