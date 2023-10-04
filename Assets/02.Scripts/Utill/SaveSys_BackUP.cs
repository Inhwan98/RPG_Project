using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public static class SaveSys_BackUP
{
    //public static void SavePlayer(PlayerController playerCtr)
    //{
    //    ObjectData playerData = new ObjectData(playerCtr);
    //    string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented);

    //    string path = Path.Combine(Application.dataPath, "PlayerData.json");

    //    File.WriteAllText(path, jsonData);
    //}

    /// <summary> Inventory Data 저장 </summary>
    public static void SaveInvenItem(Item[] items)
    {
        string jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);

        string path = Path.Combine(Application.dataPath, "Item.Json");

        File.WriteAllText(path, jsonData);
    }

    public static Item[] LoadInvenitem()
    {
        string path = Path.Combine(Application.dataPath, "Item.Json");
        string jsonData;
        Item[] invenData = new Item[64];

        if (File.Exists(path))
        {
            jsonData = File.ReadAllText(path);
            JArray jsonArray = JArray.Parse(jsonData);

            if (jsonArray != null)
            {
                List<Item> itemList = new List<Item>();

                for(int i = 0; i < jsonArray.Count; i++)
                {
                    JObject jsonObject = jsonArray[i] as JObject;

                    if (jsonObject == null)
                    {
                        itemList.Add(null);
                        continue;
                    }

                    
                    string typeName = jsonObject["Type"].ToString();
    
                    Item item = null;
                    ItemData itemData = null;

                    if (typeName == "PortionItem")
                    {
                        itemData = jsonObject["m_data"].ToObject<PortionItemData>();
                      
                    }
                    else if (typeName == "WeaponItem")
                    {            
                        itemData = jsonObject["m_data"].ToObject<WeaponItemData>();
                   
                    }
                    else if (typeName == "ArmorItem")
                    {
                        itemData = jsonObject["m_data"].ToObject<ArmorItemData>();
                        
                    }

                    if (itemData != null)
                    {
                        item = itemData.CreateItem();
                        
                        //item.SetData(itemData);
                        itemList.Add(item);
                    }
                    else
                    {
                        Debug.LogError("ItemData is NULL!!!");
                    }
                }

                return itemList.ToArray();
            }
            else
            {
                Debug.Log("JSON data is null.");
            }
        }
        else
        {
            return null;
            //Debug.LogError($"Save File Not Found in {path}");
        }
        return null;
    }

    /// <summary> Json => Object Data 반환 </summary>
    //public static ObjectData LoadObject(string fileName)
    //{
    //    string path = Path.Combine(Application.dataPath, fileName);
    //    string jsonData;
    //    ObjectData playerData;

    //    if (File.Exists(path))
    //    {
    //        jsonData = File.ReadAllText(path);
    //        playerData = JsonConvert.DeserializeObject<ObjectData>(jsonData);
    //        //playerData = JsonUtility.FromJson<ObjectData>(jsonData);
    //        return playerData;
    //    }
    //    else
    //    {
    //        Debug.LogError($"Save File Not Found in {path}");
    //    }
    //    return null;
    //}

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
}



