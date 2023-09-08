using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [SerializeField] string name;
    [SerializeField] int age;
    [SerializeField] int level;
    [SerializeField] bool isDead;

    public string GetName() => name;
    public int    GetAge() => age;
    public int    GetLevel() => level;
    public bool   GetDead() => isDead;

    public void SetName(string value) => name = value;
    public void SetAge(int value)     => age = value;
    public void SetLevel(int value)   => level = value;
    public void SetDead(bool value)   => isDead = value;

}

