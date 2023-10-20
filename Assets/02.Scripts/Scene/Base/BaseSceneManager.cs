using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Unknown,
    Village,
    DragonDungeon, 
    Loading,
}

public abstract class BaseSceneManager : MonoBehaviour
{
    public static BaseSceneManager instance = null;

    protected SceneType _sceneType = SceneType.Unknown; // 디폴트로 Unknow 이라고 초기화
    protected string _sSceneName;

    protected virtual void Awake()
    {
        Init();
    }

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
        _sceneType = StringToEnum<SceneType>(scene.name);
    }

    T StringToEnum<T>(string e)
    {
        return (T)System.Enum.Parse(typeof(T), e);
    }

}
