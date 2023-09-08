using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class PlayerController : MonoBehaviour
{
    [Header("CharcterData")]
    [SerializeField] PlayerData _playerData;

    [Header("ChracterInfo")]
    [SerializeField] string _name;
    [SerializeField] int  _age;
    [SerializeField] int  _level;
    [SerializeField] bool _isDead;


    private void Start()
    {
        _playerData = DataManager.instance.GetPlayerData();

        _age    = _playerData.GetAge();
        _isDead = _playerData.GetDead();
        _level  = _playerData.GetLevel();
        _name   = _playerData.GetName();
    }


    //private void OnApplicationQuit()
    //{
    //    string jsonData = JsonUtility.ToJson(playerData, true);
    //    File.WriteAllText(path, jsonData);
    //}

    //private void OnDisable()
    //{
    //    string jsonData = JsonUtility.ToJson(playerData, true);
    //    File.WriteAllText(path, jsonData);
    //}


}
