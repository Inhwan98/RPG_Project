using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public static class SaveSys
{
    public static void SavePlayer(PlayerController playerCtr)
    {
        PlayerData playerData = new PlayerData(playerCtr);
        string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented);

#if !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, "PlayerData.json");
#endif
#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, "PlayerData.json");
#endif

        File.WriteAllText(path, jsonData);
    }


    /// <summary> Inventory Data 저장 </summary>
    public static void SaveInvenItem(Item[] items, string name)
    {
        string jsonData = JsonConvert.SerializeObject(items, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });

#if !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, name);
#endif
#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, name);
#endif

        File.WriteAllText(path, jsonData);
    }


    public static Item[] LoadInvenitem(string name)
    {
#if !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, name);
#endif

#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, name);
#endif
        string jsonData;
        Item[] items;

        if (File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            items = JsonConvert.DeserializeObject<Item[]>(jsonData, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All                
            });
      
            return items;
        }

        return null;
    }

    /// <summary> Json => Object Data 반환 </summary>
    public static PlayerData LoadObject(string fileName)
    {
#if !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, fileName);
#endif
#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, fileName);
#endif
        string jsonData;
        PlayerData playerData;

        if (File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            playerData = JsonConvert.DeserializeObject<PlayerData>(jsonData);
            //playerData = JsonUtility.FromJson<ObjectData>(jsonData);
            return playerData;
        }
        else
        {
            //Debug.LogError($"Save File Not Found in {path}");
        }
        return null;
    }

    public static T LoadItem<T>(string fileName) where T : ItemData
    {
#if !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, fileName);
#endif
#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, fileName);
#endif
        string jsonData;
        T data;

        if(File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<T>(jsonData);
            data.Init();
            return data;
        }
        else
        {
            Debug.LogError($"Save File Not Found in {path}");
        }

        return null;
    }

    public static void SaveAllData(AllData datas)
    {
        string jsonData = JsonConvert.SerializeObject(datas, Formatting.Indented);
#if !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, "GameRPGDB.json");
#endif

#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, "GameRPGDB.json");
#endif

        File.WriteAllText(path, jsonData);
    }

    public static AllData LoadAllData()
    {
#if !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, "GameRPGDB.Json");
#endif

#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, "GameRPGDB.Json");
#endif
        string jsonData;
        AllData datas;

        if (File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            datas = JsonConvert.DeserializeObject<AllData>(jsonData, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All                
            });
            return datas;
        }
        else
        {
            Debug.LogError("Save File Not Found");
            return null;
        }
    }
}



