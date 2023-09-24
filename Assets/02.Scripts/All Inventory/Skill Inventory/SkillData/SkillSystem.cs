using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem
{
    private SkillData[] skilldatas;

    public SkillSystem()
    {
        skilldatas = SaveSys.LoadAllData().SkillDB;

        foreach(var a in skilldatas)
        {
            a.Init();
        }
    }



}