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

public abstract class BaseScene : MonoBehaviour
{
    protected SceneType _sceneType = SceneType.Unknown; // 디폴트로 Unknow 이라고 초기화

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
        {
            obj = Resources.Load<Object>("Prefab/UI/EventSystem");
            obj.name = "@EventSystem";
            Instantiate(obj);
        }
        Scene scene = SceneManager.GetActiveScene();
        _sceneType = StringToEnum<SceneType>(scene.name);
    }

    T StringToEnum<T>(string e)
    {
        return (T)System.Enum.Parse(typeof(T), e);
    }

}