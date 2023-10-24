using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary> 현재씬에서 다음씬으로 이동 </summary>
public class NextSceneController : MoveSceneBase
{ 
    protected override void MoveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
      
        _playerCtr.SetVillageScene(activeScene.name);
        var nextSceneindex = activeScene.buildIndex + 1;

        LodingSceneController.LoadScene(nextSceneindex);
    }
}
