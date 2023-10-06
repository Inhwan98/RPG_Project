using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DungeonScene : GameScene
{
    protected override void Init()
    {
        base.Init();

        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = Color.black;
    }
}