using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> GameScene에 필수적인 요소 구성 </summary>
public abstract class GameSceneManager : BaseSceneManager
{
    private Transform startTr;

    protected GameObject player;
    protected GameManager gameManager;
    protected GameObject uiManager;

    protected Camera mainCamera;
    protected Camera miniMapCamera;

    protected override void Init() // 상속 받은 Awake() 안에서 실행됨. "GameScene"씬 초기화
    {
        base.Init();

        startTr = GameObject.FindWithTag("StartPos").transform;

       
        uiManager = Resources.Load<GameObject>("Prefab/SceneLoad/@UIManager");
        player = Resources.Load<GameObject>("Prefab/SceneLoad/@Player");
        gameManager = Resources.Load<GameManager>("Prefab/SceneLoad/@GameManager");

        Instantiate<GameObject>(uiManager);
        GameObject plaeyr = Instantiate<GameObject>(player, startTr.position, startTr.rotation);
        //gameManager는 Find 함수로 player, UI 목록을 찾아야 하기 때문에 가장 마지막에 생성
        gameManager = Instantiate<GameManager>(gameManager);

        mainCamera = FindObjectOfType<CameraController>()?.GetComponent<Camera>();
        if (mainCamera == null)
        {
            mainCamera = Resources.Load<Camera>("Prefab/SceneLoad/@MainCamera");

            mainCamera = Instantiate<Camera>(mainCamera, startTr.position, startTr.rotation);
        }
        miniMapCamera = FindObjectOfType<MiniMapCamMovement>()?.GetComponent<Camera>();
        if(miniMapCamera == null)
        {
            miniMapCamera = Resources.Load<Camera>("Prefab/SceneLoad/@MiniMapCamera");
            miniMapCamera = Instantiate<Camera>(miniMapCamera);
        }
    }
}