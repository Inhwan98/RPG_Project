using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerInfoUI : MonoBehaviour
{
    /*********************************
     *            Fields
     *********************************/
    #region static Fields
    public static PlayerInfoUI instance = null;
    #endregion

    #region option Fields
    [Header("Level UI")]
    [SerializeField] private TMP_Text _levelText;

    [Header("HP UI")]
    [SerializeField] private Image _hpBarImage;
    [SerializeField] private RectTransform _hpHandleRT;
    [SerializeField] private Text _hpText;

    [Header("MP UI")]
    [SerializeField] private Image[] _mpBarImage = new Image[2];

    [Header("EXP UI")]
    [SerializeField] private Image _expBarImage;
    [SerializeField] private RectTransform _expHandleRT;
    [SerializeField] private Text _expText;

    [Header("Notice Text")]
    [SerializeField] private RectTransform _conversationRect;
    [SerializeField] private TMP_Text _conversationText;

    [Header("StatusInfo Text")]
    [SerializeField] private TMP_Text[] _statusText = new TMP_Text[7];

    [Header("Player Die UI")]
    [SerializeField] private GameObject _playerDiePopUp;
    [SerializeField] private Button _confirmationOKButton;
    #endregion

    #region private Fields
    private PlayerStatusData _PSD;
    private RectTransform _canvasRect;
    private PlayerStatManager _playerStatMgr;
    #endregion


    /*********************************
    *        Get, Set Methods
    *********************************/
    #region Set Methods
    public void SetPlayerUIMgr(PlayerStatManager playerStatManager) => this._playerStatMgr = playerStatManager;
    public void SetPlayerStatusData(PlayerStatusData PSD) => _PSD = PSD;
    #endregion

    /*********************************
    *          Unity Evenet
    *********************************/
    #region Unity Event
    private void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate(this.gameObject);
        }
        instance = this;

        HidePlayerDieUI();

        _canvasRect = transform.GetChild(0).GetComponent<RectTransform>();
        _conversationRect.gameObject.SetActive(false);
    }

    private void Start()
    {
        _confirmationOKButton.onClick.AddListener(_playerStatMgr.ReturnToVillage);
        _confirmationOKButton.onClick.AddListener(HidePlayerDieUI);
        PlayerController.OnPlayerDie += ShowPlayerDieUI;
    }

    #endregion

    public void ShowPlayerDieUI()
    {
        _playerDiePopUp.SetActive(true);
    }

    public void HidePlayerDieUI()
    {
        _playerDiePopUp.SetActive(false);
    }


    /*********************************
    *             Methods
    *********************************/

    #region PlayerStatus / Info
    public void UpdateHPbar(float _playerHP, float _playerMaxHP)
    {
        float amount = (_playerHP / _playerMaxHP);
        _hpBarImage.fillAmount = amount;

        Vector2 vec;
        vec.x = amount;
        vec.y = 0;
        _hpHandleRT.anchorMin = vec;
        vec.y = 1;
        _hpHandleRT.anchorMax = vec;
        _hpText.text = $"{_playerHP}/{_playerMaxHP}";
    }

    public void UpdateMPbar(float _playerMP, float _playerMaxMP)
    {
        float amount = (_playerMP / _playerMaxMP);
        _mpBarImage[0].fillAmount = amount;
        _mpBarImage[1].fillAmount = amount;
    }

    public void UpdateEXPbar(float _playerEXP, float _playerMaxEXP)
    {
        float amount = (_playerEXP / _playerMaxEXP);
        _expBarImage.fillAmount = amount;

        Vector2 vec;
        vec.x = amount;
        vec.y = 0;
        _expHandleRT.anchorMin = vec;
        vec.y = 1;
        _expHandleRT.anchorMax = vec;
        _expText.text = $"{_playerEXP}/{_playerMaxEXP} XP";
    }

    public void UpdateLevelText(int nLevel)
    {
        _levelText.text = $"Lv.{nLevel}";
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

    /// <summary> Status UI를 업데이트 해준다. </summary>
    public void UpdateStatusDisplay()
    {
        if (_PSD == null) return;

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
