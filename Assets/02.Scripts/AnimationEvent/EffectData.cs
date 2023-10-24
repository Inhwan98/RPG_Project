using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectData
{
    public int nID;
    public bool bIsMultiple;    //다중 생성 되는 이펙트인가
    public int nMultipleAmount; //다중 생성 되는 양
    public string sName;

    public float[] fPosArray;
    public float[] fRotArray;
    [Newtonsoft.Json.JsonProperty]
    private float fDisableAfter;
    [Newtonsoft.Json.JsonProperty]
    private bool bUseLocalPosition;

    private Vector3 pos;
    private Quaternion rot;

    public void PosAndRot_Init()
    {
        pos = new Vector3(fPosArray[0], fPosArray[1], fPosArray[2]);
        rot = Quaternion.Euler(fRotArray[0], fRotArray[1], fRotArray[2]);
    }
    public Vector3 GetPos() => pos;
    public Quaternion GetRot() => rot;

    public float GetDisableAfter() => fDisableAfter;
    public bool GetUseLocalPos() => bUseLocalPosition;
}