using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipState
{
    HEAD,
    CHEST,
    GLOVES,
    WEAPON,
    CLOAK,
    SHOULDER,
    PANTS,
    BOOTS
}


public abstract class EquipmentItemData : ItemData
{
    [Newtonsoft.Json.JsonProperty]
    private int _maxDurability = 100;

    [Newtonsoft.Json.JsonProperty]
    private EquipState _eEquipState;


    [Newtonsoft.Json.JsonProperty]
    private float[] _fPosArray;

    [Newtonsoft.Json.JsonProperty]
    private float[] _fRotArray;

    [Newtonsoft.Json.JsonProperty]
    private string _equipGoPath;

    private GameObject _equipGo;

    private Vector3 pos;
    private Quaternion rot;


    public override void Init()
    {
        base.Init();

        EquipInit();
    }

    /// <summary>
    /// 장비의 장착 위치, 회전 / 장착 prefab obj 세팅
    /// </summary>
    public void EquipInit()
    {
        if (_fPosArray == null) return;

        pos = new Vector3(_fPosArray[0], _fPosArray[1], _fPosArray[2]);
        rot = Quaternion.Euler(_fRotArray[0], _fRotArray[1], _fRotArray[2]);

        _equipGo = Resources.Load<GameObject>(_equipGoPath);
    }

    public Vector3 GetPos() => pos;
    public Quaternion GetRot() => rot;
    public GameObject GetWeaponPrefab() => _equipGo;

    /// <summary> 최대 내구도 </summary>
    public int GetMaxDurability() => _maxDurability;
    public EquipState GetEquipState() => _eEquipState;
}
