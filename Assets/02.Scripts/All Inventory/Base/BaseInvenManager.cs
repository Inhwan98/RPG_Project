using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/*
    [상속 구조]

    BaseInvenManager(abstract)
        
        * 아이템 관련 인벤토리
        - BaseItemInvenManager(abstract) : 모든 아이템 인벤토리에 공통될 정보를 가지고 있다. ex) GetItem, AddItem
            - ItemInventoryManager       : 아이템 인벤토리를 관리하는 매니져 스크립트
            - MerChantInventoryManager   : 상인 아이템 인벤토리를 관리하는 매니져 스크립트
            - PlayerStatManager          : 플레이어의 장비를 관리하는 매니져 스크립트

        * 스킬 관련 인벤토리
        - SkillManager :전체 스킬 인벤토리를 관리한다.

*/

public abstract class BaseInvenManager : MonoBehaviour
{
    public abstract void SetPlayerCtr(PlayerController player);
    public abstract bool IsValidIndex(int index);
    public abstract bool HasItem(int index);
    /// <summary> 해당 슬롯의 아이템 이름 리턴 </summary>
    public abstract string GetItemName(int index);
    /// <summary> 해당하는 인덱스의 슬롯들의 상태 및 UI 갱신 </summary>
    public abstract void UpdateSlot(params int[] indices);
    /// <summary> 해당하는 인덱스의 슬롯 상태 및 UI 갱신 </summary>
    public abstract void UpdateSlot(int index);
    /// <summary> 모든 슬롯들의 상태를 UI에 갱신 </summary>
    public abstract void UpdateAllSlot();
    /// <summary> Inventory 활성화 유무 </summary>
    public abstract void SetWindowActive(bool value);
    ///<summary> 해당 슬롯의 아이템 제거 </summary>
    public abstract void Remove(int index);
    public abstract void Swap(int indexA, int indexB);
    public abstract void UpdateAccessibleStatesAll();
}
