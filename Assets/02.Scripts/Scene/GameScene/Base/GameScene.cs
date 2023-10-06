using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GameScene : BaseScene
{
    private Transform startTr;

    GameObject player;
    GameObject gameManager;
    GameObject uiManager;

    protected Camera mainCamera;

    protected override void Init() // 상속 받은 Awake() 안에서 실행됨. "GameScene"씬 초기화
    {
        base.Init();

        startTr = GameObject.FindWithTag("StartPos").transform;

       
        player = Resources.Load<GameObject>("Prefab/SceneLoad/Player");
        gameManager = Resources.Load<GameObject>("Prefab/SceneLoad/GameManager");
        uiManager = Resources.Load<GameObject>("Prefab/SceneLoad/UIManager");


        Instantiate<GameObject>(uiManager);
        Instantiate<GameObject>(player, startTr.position, startTr.rotation);
        Instantiate<GameObject>(gameManager);

        mainCamera = FindObjectOfType<CameraController>()?.GetComponent<Camera>();
        if (mainCamera == null)
        {
            mainCamera = Resources.Load<Camera>("Prefab/SceneLoad/MainCamera");

            mainCamera = Instantiate<Camera>(mainCamera, startTr.position, startTr.rotation);
        }
    }
}