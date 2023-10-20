using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BeforeSceneController : MoveSceneBase
{ 
    protected override void MoveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
      
        var beforeSceneindex = activeScene.buildIndex - 1;

        LodingSceneController.LoadScene(beforeSceneindex);
    }
}
