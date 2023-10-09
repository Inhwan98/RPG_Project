using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : Monster
{

    public void StartTimeLine()
    {
        _nav.enabled = false;
        this.enabled = false;
    }

    public void EndTimeLine()
    {
        _nav.enabled = true;
        this.enabled = true;
    }

}

