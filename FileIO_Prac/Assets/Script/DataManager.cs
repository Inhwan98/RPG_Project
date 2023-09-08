using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance = null;

    [Header("CharcterData")]
    [SerializeField] PlayerData _playerData;

    string path;
    string InitPath;


    string fileName = "playerData.json";
    string initFileName = "playerInit.json";

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);


        path = Path.Combine(Application.dataPath, fileName);
        InitPath = Path.Combine(Application.dataPath, initFileName);

        if (File.Exists(path))
        {
            //Load
            Debug.Log("파일 존재");
            string jsonData = File.ReadAllText(path);
            _playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        }
        else
        {
            //신규
            if (File.Exists(InitPath))
            {
                Debug.Log("파일 존재 하지 않으");
                string jsonData = File.ReadAllText(InitPath);
                _playerData = JsonUtility.FromJson<PlayerData>(jsonData);
            }
        }
    }

    public PlayerData GetPlayerData()
    {
        return _playerData;
    }

    private void OnDestroy()
    {
        string jsonData = JsonUtility.ToJson(_playerData, true);
        File.WriteAllText(path, jsonData);
    }
}
