using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv01_09 : Monster
{
    protected override void Awake()
    {
        Init();
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Init()
    {
        m_nPortionDrop_MinAmount = 1;
        m_nPortionDrop_MaxAmount = 9;
        m_nItemDrop_percentage = 80;
    }
}

