using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance = null;
    
    private int m_curLevel;

    [Header("HP UI")]
    [SerializeField] private Image hpBarImage;
    [SerializeField] private RectTransform hpHandleRT;
    [SerializeField] private Text hpText;

    [Header("MP UI")]
    [SerializeField] private Image[] mpBarImage = new Image[2];

    [Header("EXP UI")]
    [SerializeField] private Image expBarImage;
    [SerializeField] private RectTransform expHandleRT;
    [SerializeField] private Text expText;

    private PlayerController _playerCtr;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

    }

    public void SetPlayerCtrReference(PlayerController playerCtr)
    {
        _playerCtr = playerCtr;
    }

    public void SetHPbar(float _playerHP, float _playerMaxHP)
    {
        float amount = (_playerHP / _playerMaxHP);
        hpBarImage.fillAmount = amount;

        Vector2 vec;
        vec.x = amount;
        vec.y = 0;
        hpHandleRT.anchorMin = vec;
        vec.y = 1;
        hpHandleRT.anchorMax = vec;
        hpText.text = $"{_playerHP}/{_playerMaxHP}";
    }

    public void SetMPbar(float _playerMP, float _playerMaxMP)
    {
        float amount = (_playerMP / _playerMaxMP);
        mpBarImage[0].fillAmount = amount;
        mpBarImage[1].fillAmount = amount;
    }

    public void SetEXPbar(float _playerEXP, float _playerMaxEXP)
    {
        float amount = (_playerEXP / _playerMaxEXP);
        expBarImage.fillAmount = amount;

        Vector2 vec;
        vec.x = amount;
        vec.y = 0;
        expHandleRT.anchorMin = vec;
        vec.y = 1;
        expHandleRT.anchorMax = vec;
        expText.text = $"{_playerEXP}/{_playerMaxEXP} XP";
    }

    public void DisplayInfo(int _level, float _maxHP, float _maxMP, float _str)
    {
        m_curLevel = _level;
     
    }
}
