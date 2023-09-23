using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public static class SaveSys
{
    public static void SavePlayer(PlayerController playerCtr)
    {
        ObjectData playerData = new ObjectData(playerCtr);
        string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented);

        string path = Path.Combine(Application.dataPath, "PlayerData.json");

        File.WriteAllText(path, jsonData);
    }

    public static SkillData[] LoadSkillSet(string filename)
    {
        string path = Path.Combine(Application.dataPath, filename);
        string jsonData;
        if (File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            SkillData[] SkillSet = JsonConvert.DeserializeObject<SkillData[]>(jsonData); 
            return SkillSet;
        }
        else
        {
            Debug.LogError("LoadSkillSet Path is NULL!!");
            return null;
        }
    }

    public static void SavePlayerSkillSet(SkillData[] skillset)
    {

        //string jsonData = JsonUtility.ToJson(skillset, true);

        string jsonData = JsonConvert.SerializeObject(skillset, Formatting.Indented);

        string path = Path.Combine(Application.dataPath, "PlayerSkillSetData.Json");

        File.WriteAllText(path, jsonData);
    }

    /// <summary> Inventory Data 저장 </summary>
    public static void SaveInvenItem(Item[] items)
    {
        string jsonData = JsonConvert.SerializeObject(items, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });

        string path = Path.Combine(Application.dataPath, "Item.Json");

        File.WriteAllText(path, jsonData);
    }

    public static Item[] LoadInvenitem()
    {
        string path = Path.Combine(Application.dataPath, "Item.Json");
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
        else
        {
            
        }
        return null;
    }

    /// <summary> Json => Object Data 반환 </summary>
    public static ObjectData LoadObject(string fileName)
    {
        string path = Path.Combine(Application.dataPath, fileName);
        string jsonData;
        ObjectData playerData;

        if (File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            playerData = JsonConvert.DeserializeObject<ObjectData>(jsonData);
            //playerData = JsonUtility.FromJson<ObjectData>(jsonData);
            return playerData;
        }
        else
        {
            Debug.LogError($"Save File Not Found in {path}");
        }
        return null;
    }

    public static T LoadItem<T>(string fileName) where T : ItemData
    {
        string path = Path.Combine(Application.dataPath, fileName);
        string jsonData;
        T data;

        if(File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<T>(jsonData);
            data.SetIcon();
            return data;
        }
        else
        {
            Debug.LogError($"Save File Not Found in {path}");
        }

        return null;
    }

    public static AllData LoadDialogData()
    {
        string path = Path.Combine(Application.dataPath, "DialogDB.Json");
        string jsonData;
        AllData datas;

        if (File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            datas = JsonUtility.FromJson<AllData>(jsonData);
            return datas;
        }
        else
        {
            Debug.LogError("Save File Not Found");
            return null;
        }
    }
}



