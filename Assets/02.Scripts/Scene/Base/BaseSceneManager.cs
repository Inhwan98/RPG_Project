using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


/*
    [상속 구조]

    BaseSceneManager(abstract) : 모든 씬에 공통적으로 있어야 하는부분인 EventSystem이 존재하는지 여부를 확인 후 조치
                               : 씬매니져는 정적으로 항상 씬에 존재해야 한다.
       
        * 게임씬
        - GameSceneManager : Player, UIManager, GameManager, MainCamara가 항상 씬에 존재하게 만듦
            - DungeonSceneManager : 던전 씬을 구성하고, 던전을 관리감독한다.
            - VillageSceneManager : 마을 씬을 구성하고, 마을 NPC를 관리한다.
*/

public abstract class BaseSceneManager : MonoBehaviour
{
    /************************************************
     *                   Fields
     ************************************************/
    public static BaseSceneManager instance = null;

    protected string _sSceneName;

    /************************************************
    *                 Unity Event
    ************************************************/

    protected virtual void Awake()
    {
        Init();
    }

   /************************************************
    *                 Methods
    ************************************************/

    protected virtual void Init()
    {
        #region SingleTone
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        #endregion

        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
        {
            obj = Resources.Load<Object>("Prefab/UI/EventSystem");
            obj.name = "@EventSystem";
            Instantiate(obj);
        }
        Scene scene = SceneManager.GetActiveScene();
        _sSceneName = scene.name;
    }

    T StringToEnum<T>(string e)
    {
        return (T)System.Enum.Parse(typeof(T), e);
    }

}
