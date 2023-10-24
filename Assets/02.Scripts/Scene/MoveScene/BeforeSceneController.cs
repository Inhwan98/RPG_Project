using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary> 현재 씬에서 -1 씬으로 이동 </summary>
public class BeforeSceneController : MoveSceneBase
{ 
    protected override void MoveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
      
        var beforeSceneindex = activeScene.buildIndex - 1;

        LodingSceneController.LoadScene(beforeSceneindex);
    }
}
