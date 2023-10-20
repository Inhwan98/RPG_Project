using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> 이전에 플레이어가 머물던 마을로 돌아가게 함 </summary>
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

    protected abstract void MoveScene();


}