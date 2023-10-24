using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
        [상속 구조]

        MoveSceneBase(abstract) : 씬 이동 관련 스크립트

             - BeforeSceneController : 이전 씬 인덱스로 이동
             - NextSceneController   : 다음 씬 인덱스로 이동
             - VilliageTeleportation : 이전에 있던 Village Scene.name 으로 이동 (index 아님)

         MoveScene(abstract) 함수를 포함하고 있으며 비동기 로딩 씬 전환 함수이다.
         
         LodingSceneController(static) 에서 비동기 씬 전환 처리가 이루어 진다.
 */



/// <summary>
/// 포탈 게임 오브젝트에 컴포넌트로 존재해야 하는 스크립트
/// Player와 포탈의 상호작용, Scene이동의 관한 클래스
/// 씬 마다 연결된 Scene Index 관계 중요할 것
/// </summary>
public abstract class MoveSceneBase : MonoBehaviour
{
    protected PlayerController _playerCtr;
    protected void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _playerCtr = coll.GetComponent<PlayerController>();
            MoveScene();
        }
    }

    /// <summary> 비동기 로딩씬 전환, 설정한 씬으로 이동</summary>
    protected abstract void MoveScene();
}