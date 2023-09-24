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
        _playerCtr.AddInven(_itemDic);

        base.Die();
    }

    protected override void LoadData()
    {
        //몬스터는 100번대 부터 시작한다. 0번째 인덱스 부터 시작하려면 m_ID - 100 
        //현재 몬스터에 해당하는 INDEX를 찾아간다.
        _monsterData = SaveSys.LoadAllData().MonsterDB[m_nID - 100];

        if (_monsterData == null)
        {
            Debug.LogError("MonData is NULL!!");
            return;
        }



        m_nLevel = _monsterData.nLevel;
        m_nMaxHP = _monsterData.nMaxHP;
        m_nCurHP = _monsterData.nMaxHP;

        m_nMaxMP = _monsterData.nMaxMP;
        m_nCurMP = _monsterData.nMaxMP;


        m_nCurSTR = _monsterData.nCurSTR;
        m_nCurExp = _monsterData.nDropExp;
    }

}
