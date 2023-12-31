﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourcesData
{
    private ItemData[] _portionDatas; //포션데이터
    private ItemData[] _aromorDatas;  //방어구데이터
    private ItemData[] _weaponDatas;  //무기데이터

    private TMP_Text _questHeader;  //퀘스트의 제목
    private TMP_Text _questContext; //퀘스트의 내용

    private GameObject[] _levelUPEffect; //레벨업 이펙트

    private GameObject _questionMarkGo;
    private GameObject _exclamationMarkGO;

    private GameObject _villiagePortal;
    private GameObject _npcConversationKeyGo;

    private GameObject _miniMapObjIcon;

    public ResourcesData(AllData allData)
    {
        Init(allData);
    }

    public void Init(AllData allData)
    {
        _portionDatas = allData.PortionItemDB;
        _aromorDatas  = allData.ArmorItemDB;
        _weaponDatas  = allData.WeaponItemDB;

        _questHeader   = Resources.Load<TMP_Text>("Prefab/UI/Header");
        _questContext  = Resources.Load<TMP_Text>("Prefab/UI/Context");
        _levelUPEffect = Resources.LoadAll<GameObject>("Prefab/LevelUP");

        _questionMarkGo = Resources.Load<GameObject>("Prefab/Mark/QuestionMark");
        _exclamationMarkGO = Resources.Load<GameObject>("Prefab/Mark/ExclamationMark");

        _villiagePortal = Resources.Load<GameObject>("Prefab/SceneMove/Portal");

        _npcConversationKeyGo = Resources.Load<GameObject>("Prefab/UI/ConversationKey");

        _miniMapObjIcon = Resources.Load<GameObject>("Circle");
    }


    public List<ItemData> GetItemList(int[] _nIDArray)
    {
        int index;

        List<ItemData> itemDatas = new List<ItemData>();

        foreach(int nID in _nIDArray)
        {
            if (nID >= 1000 && nID < 2000)
            {
                index = nID - 1000;
                itemDatas.Add(_portionDatas[index]);
            }
            else if (nID >= 2000 && nID < 3000)
            {
                index = nID - 2000;
                itemDatas.Add(_aromorDatas[index]);
            }
            else if(nID >= 3000 && nID < 4000)
            {
                index = nID - 3000;
                itemDatas.Add(_weaponDatas[index]);
            }
        }

        return itemDatas;
    }

    public ItemData GetItem(int nID)
    {
        int index = 0;

        if (nID >= 1000 && nID < 2000)
        {
            index = nID - 1000;
            _portionDatas[index].Init();
            return _portionDatas[index];
        }
        else if(nID >= 2000 && nID < 3000)
        {
            index = nID - 2000;
            _aromorDatas[index].Init();
            return _aromorDatas[index];
        }
       else if(nID >= 3000 && nID < 4000)
        {
            index = nID - 3000;
            _weaponDatas[index].Init();
            
            return _weaponDatas[index];
        }

        return null;
    }

    public TMP_Text GetQuestHeader() => _questHeader;
    public TMP_Text GetQuestConText() => _questContext;
    public GameObject[] GetLevelUPEffect() => _levelUPEffect;

    public GameObject GetQuestionMarkGo() => _questionMarkGo;
    public GameObject GetExclamationMarkGO() => _exclamationMarkGO;
    public GameObject GetVilliagePortal() => _villiagePortal;

    public GameObject GetConversationKey() => _npcConversationKeyGo;

    public GameObject GetMinimapObjIcon() => _miniMapObjIcon;
}