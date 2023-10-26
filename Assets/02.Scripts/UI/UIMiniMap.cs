using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class UIMiniMap : MonoBehaviour
{
    [SerializeField] private TMP_Text minimapName;

    private void Awake()
    {
        minimapName.text = SceneManager.GetActiveScene().name;
    }
}

