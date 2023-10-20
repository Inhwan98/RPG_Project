using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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
