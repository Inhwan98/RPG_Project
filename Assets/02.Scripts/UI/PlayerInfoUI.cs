using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerInfoUI : MonoBehaviour
{
    public static PlayerInfoUI instance = null;
    private PlayerStatusData _PSD;

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

    [Header("Notice Text")]
    [SerializeField] private RectTransform _conversationRect;
    [SerializeField] private TMP_Text _conversationText;
    RectTransform _canvasRect;

    //직업 Level 체력 마나 힘 공격력 방어력

    [SerializeField] private TMP_Text[] _statusText = new TMP_Text[7];

    //[SerializeField] private TMP_Text _classTMP;
    //[SerializeField] private TMP_Text _levelTMP;
    //[SerializeField] private TMP_Text _hpTMP;
    //[SerializeField] private TMP_Text _mpTMP;
    //[SerializeField] private TMP_Text _strTMP;
    //[SerializeField] private TMP_Text _powerTMP;
    //[SerializeField] private TMP_Text _defenceTMP;


    private PlayerUIManager _playerUIMgr;

    public void SetPlayerUIMgr(PlayerUIManager playerUIManager) => this._playerUIMgr = playerUIManager;

    private void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(this.gameObject);
        }
        instance = this;

        _canvasRect = transform.GetChild(0).GetComponent<RectTransform>();
        _conversationRect.gameObject.SetActive(false);
    }

    #region PlayerStatus / Info
    public void UpdateHPbar(float _playerHP, float _playerMaxHP)
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

    public void UpdateMPbar(float _playerMP, float _playerMaxMP)
    {
        float amount = (_playerMP / _playerMaxMP);
        mpBarImage[0].fillAmount = amount;
        mpBarImage[1].fillAmount = amount;
    }

    public void UpdateEXPbar(float _playerEXP, float _playerMaxEXP)
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
    #endregion
    #region ConverSationKey
    /// <summary>
    /// NPC 상호작용 Text 상자 활성화 후, 목표 월드 좌표 계산하여 나타냄
    /// </summary>
    public void ConversationKeyActiveOn(Vector3 pos)
    {
        _conversationRect.gameObject.SetActive(true);
        //_npcConversationKeyGo.transform.position = pos;

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(pos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f)));
        _conversationRect.anchoredPosition = WorldObject_ScreenPosition;
    }

    /// <summary> NPC 상호작용 Text 비활성화 </summary>
    public void ConversationKeyActiveOff() => _conversationRect.gameObject.SetActive(false);
    #endregion
    #region DisPlayer PlayerInfo

    public void SetPlayerStatusData(PlayerStatusData PSD) => _PSD = PSD;

    public void StatusDisplay()
    {
        _PSD.UpdateStatusData();
        _statusText[0].text = _PSD.m_sClassName;
        _statusText[1].text = $"{_PSD.m_nLevel}";
        _statusText[2].text = $"{_PSD.m_nCurHP}/{_PSD.m_nMaxHP}";
        _statusText[3].text = $"{_PSD.m_nCurMP}/{_PSD.m_nMaxMP}";
        _statusText[4].text = $"{_PSD.m_nStr}";
        _statusText[5].text = $"{_PSD.m_MinPower} ~ {_PSD.m_nMaxPower}";
        _statusText[6].text = $"{_PSD.m_nDefence}";
    }



    #endregion

}
