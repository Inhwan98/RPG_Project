﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class StageNPCData
{
    public string _sVillageName;
    public int _nNPCID;
    public string _sNPCName;
    public float[] _fPosArray;
    public float[] _fRotArray;

    private Vector3 _pos;
    private Quaternion _rot;

    public void PosAndRot_Init()
    {
        if (_fPosArray == null || _fRotArray == null) return;
        _pos = new Vector3(_fPosArray[0], _fPosArray[1], _fPosArray[2]);
        _rot = Quaternion.Euler(_fRotArray[0], _fRotArray[1], _fRotArray[2]);
    }

    public Vector3 GetPos() => _pos;
    public Quaternion GetRot() => _rot;
}


/// <summary> 마을 씬 구성 내용 (주로 NPC 생성 관련) </summary>
public class VillageSceneManager : GameSceneManager
{
    /******************************************
    *                  Fields
    *******************************************/
    private List<StageNPCData> _stageNPCDataList = new List<StageNPCData>();


    /******************************************
     *               Unity Evenet
     *******************************************/


    private void Start()
    {
        ViliageStageLoad();
        CreateNPC();
    }


    /******************************************
     *                 Methods
     *******************************************/

    protected override void Init()
    {
        base.Init();

        mainCamera.clearFlags = CameraClearFlags.Skybox;
    }

    //마을의 NPC를 초기화 한다.
    public void ViliageStageLoad()
    {
        var stageNPCDB = SaveSys.LoadAllData().StageNPCDB;
        Debug.Assert(stageNPCDB != null, "stageNPCDB Is NULL");

        foreach (var stageData in stageNPCDB)
        {
            if (stageData._sVillageName == _sSceneName)
            {
                stageData.PosAndRot_Init();
                _stageNPCDataList.Add(stageData);
            }
        }
    }

    /// <summary> SceneData에 해당하는 NPC들을 로드한다. </summary>
    private void CreateNPC()
    {
        for(int i = 0; i< _stageNPCDataList.Count; i++)
        {
            var curNPC = _stageNPCDataList[i];
            string path = Path.Combine("Prefab", "NPC", curNPC._sNPCName);
            NPC npc = Resources.Load<NPC>(path);
            npc.SetID(curNPC._nNPCID);
            npc.gameObject.SetActive(true);

            //DB에 정해둔 값으로 생성
            Instantiate(npc, curNPC.GetPos(), curNPC.GetRot());
        }
    }

}
