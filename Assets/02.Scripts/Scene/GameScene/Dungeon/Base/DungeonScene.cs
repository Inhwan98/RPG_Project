using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DungeonScene : GameScene
{
    GameManager gameMgr;
    DungeonManager dungeonMgr;

    protected override void Init()
    {
        base.Init();

        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = Color.black;

        //dungeonMgr = Resources.Load<DungeonManager>("Prefab/SceneLoad/DungeonManager");
        //dungeonMgr = Instantiate<DungeonManager>(dungeonMgr);

    }
}