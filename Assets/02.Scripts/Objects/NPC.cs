﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC : MonoBehaviour
{
    [SerializeField]
    private int nID;

    public int GetID()
    {
        return nID;
    }
}