using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Unknown, // 디폴트
    Village, // 로그인 화면 씬
    Dungeon, // 로비 씬
    Loading, // 인게임 씬
}

public abstract class BaseScene : MonoBehaviour
{
    public SceneType SceneType { get; protected set; } = SceneType.Unknown; // 디폴트로 Unknow 이라고 초기화

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
        SceneType = StringToEnum<SceneType>(scene.name);
    }

    T StringToEnum<T>(string e)
    {
        return (T)System.Enum.Parse(typeof(T), e);
    }

    public abstract void Clear();

}