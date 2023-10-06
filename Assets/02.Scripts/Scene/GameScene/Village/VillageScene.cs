using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VillageScene : GameScene
{
    protected override void Init()
    {
        base.Init();

        mainCamera.clearFlags = CameraClearFlags.Skybox;
    }
}