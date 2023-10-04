using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameScene : BaseScene
{
    private Transform startTr;

    GameObject player;
    GameObject gameManager;
    GameObject uiManager;
    GameObject mainCamera;

    protected override void Init() // 상속 받은 Awake() 안에서 실행됨. "GameScene"씬 초기화
    {
        base.Init(); // 📜BaseScene의 Init()

        startTr = GameObject.FindWithTag("StartPos").transform;

       
        player = Resources.Load<GameObject>("Prefab/SceneLoad/Player");
        gameManager = Resources.Load<GameObject>("Prefab/SceneLoad/GameManager");
        uiManager = Resources.Load<GameObject>("Prefab/SceneLoad/UIManager");

        mainCamera = FindObjectOfType<CameraController>().gameObject;
        if (mainCamera == null)
        {
            mainCamera = Resources.Load<GameObject>("Prefab/SceneLoad/MainCamera");
            Instantiate<GameObject>(mainCamera);
        }


        Instantiate<GameObject>(player, startTr);
        Instantiate<GameObject>(gameManager);
        Instantiate<GameObject>(uiManager);
        
    }

    public override void Clear()
    {

    }
}