using UnityEngine;
using System.IO;


public static class SaveSys
{
    public static void SavePlayer(PlayerController playerCtr)
    {
        ObjectData playerData = new ObjectData(playerCtr);
        string jsonData = JsonUtility.ToJson(playerData, true);

        string path = Path.Combine(Application.dataPath, "PlayerData.json");

        File.WriteAllText(path, jsonData);
    }

    public static void Saveitem(ItemData itemdata)
    {
        string jsonData = JsonUtility.ToJson(itemdata, true);

        string path = Path.Combine(Application.dataPath, "Item.Json");

        File.WriteAllText(path, jsonData);
    }


    public static ObjectData LoadPlayer()
    {
        string path = Path.Combine(Application.dataPath, "PlayerData.Json");
        string jsonData;
        ObjectData playerData;

        if (File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            playerData = JsonUtility.FromJson<ObjectData>(jsonData);
            return playerData;
        }
        else
        {
            path = Path.Combine(Application.dataPath, "PlayerStartData.Json");

            if(File.Exists(path))
            {
                jsonData = File.ReadAllText(path);
                playerData = JsonUtility.FromJson<ObjectData>(jsonData);
                return playerData;
            }
            else
            {
                Debug.LogError($"Save File Not Found in {path}");
            }
        }
        return null;
    }

    public static T LoadItem<T>(string name) where T : ItemData
    {
        string path = Path.Combine(Application.dataPath, name);
        string jsonData;
        T data;

        if(File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            data = JsonUtility.FromJson<T>(jsonData);
            data.SetIcon();
            return data;
        }
        else
        {
            Debug.LogError($"Save File Not Found in {path}");
        }

        return null;
    }
}



