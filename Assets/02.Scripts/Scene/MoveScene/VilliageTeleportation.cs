using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> 이전에 플레이어가 머물던 마을로 돌아가게 함 </summary>
public class VilliageTeleportation : MoveSceneBase
{
    protected override void MoveScene()
    { 
        string stayingVilliage = _playerCtr.GetPlayerStayingVilliage();
        LodingSceneController.LoadScene(stayingVilliage);
    }
}
