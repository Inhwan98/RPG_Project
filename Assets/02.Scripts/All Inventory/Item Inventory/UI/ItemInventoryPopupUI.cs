using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> 인벤토리 UI 위에 띄울 작은 팝업들 관리 </summary>
public class ItemInventoryPopupUI : MonoBehaviour
{
    /*************************************************************
     *                           Fields
     ************************************************************/

    #region 확인, 취소 팝업
    [Header("Confirmation Popup")]
    [SerializeField] private GameObject _confirmationPopupObject;
    [SerializeField] private Text       _confirmationItemNameText;
    [SerializeField] private Text       _confirmationText;
    [SerializeField] private Button     _confirmationOkButton;          //OK
    [SerializeField] private Button     _confirmationCancelButton;      //Cancel
    private event Action OnConfirmationOK; //확인 버튼 누를 경우 실행할 이벤트 
    #endregion

    #region 수량 입력 팝업
    [Header("Amount Input Popup")]
    [SerializeField] private GameObject _amountInputPopupObject;
    [SerializeField] private Text _amountInputItemNameText;
    [SerializeField] private InputField _amountInputField;
    [SerializeField] private Button _amountPlusButton;        // +
    [SerializeField] private Button _amountMinusButton;       // -
    [SerializeField] private Button _amountInputOkButton;     // Ok
    [SerializeField] private Button _amountInputCancelButton; // Cancel

    // 확인 버튼 눌렀을 때 동작할 이벤트
    private event Action<int> OnAmountInputOK;
    // 수량 입력 제한 개수
    private int _maxAmount;
    #endregion

    /*************************************************************
    *                         Unity Event
    ************************************************************/
    private void Awake()
    {
        InitUIEvents();
    }

    /*************************************************************
    *                          Methods
    ************************************************************/
    #region private Methods
    private void InitUIEvents()
    {
        // 1. 확인 버튼 누를 경우 이벤트
        _confirmationOkButton.onClick.AddListener(HidePanel);
        _confirmationOkButton.onClick.AddListener(HideConfirmationPopup);
        _confirmationOkButton.onClick.AddListener(() => OnConfirmationOK?.Invoke());

        // 2. 취소 버튼 누를 경우 이벤트
        _confirmationCancelButton.onClick.AddListener(HidePanel);
        _confirmationCancelButton.onClick.AddListener(HideConfirmationPopup);


        _amountInputOkButton.onClick.AddListener(HidePanel);
        _amountInputOkButton.onClick.AddListener(HideAmountInputPopup);
        _amountInputOkButton.onClick.AddListener(() => OnAmountInputOK?.Invoke(int.Parse(_amountInputField.text)));

        _amountInputCancelButton.onClick.AddListener(HidePanel);
        _amountInputCancelButton.onClick.AddListener(HideAmountInputPopup);

        // [-] 버튼 이벤트
        _amountMinusButton.onClick.AddListener(() =>
        {
            // TryParse : 문자열 => 정수형으로 변환하고 bool값을 반환. 변환된 변수는 out에 의해 저장한다.
            int.TryParse(_amountInputField.text, out int amount);
            if (amount > 1)
            {
                // Shift 누르면 10씩 감소
                int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount - 10 : amount - 1;
                if (nextAmount < 1)
                    nextAmount = 1;
                _amountInputField.text = nextAmount.ToString(); // 정수를 다시 문자열로 변환
            }
        });

        // [+] 버튼 이벤트
        _amountPlusButton.onClick.AddListener(() =>
        {
            int.TryParse(_amountInputField.text, out int amount);
            if (amount < _maxAmount)
            {
                // Shift 누르면 10씩 증가
                int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount + 10 : amount + 1;
                if (nextAmount > _maxAmount)
                    nextAmount = _maxAmount;
                _amountInputField.text = nextAmount.ToString();
            }
        });

        // 입력 값 범위 제한
        _amountInputField.onValueChanged.AddListener(str =>
        {
            int.TryParse(str, out int amount);
            bool flag = false;

            if (amount < 1)
            {
                flag = true;
                amount = 1;
            }
            else if (amount > _maxAmount)
            {
                flag = true;
                amount = _maxAmount;
            }

            if (flag)
                _amountInputField.text = amount.ToString();
        });
    }
    private void ShowPanel() => gameObject.SetActive(true);
    private void HidePanel() => gameObject.SetActive(false);
    private void HideConfirmationPopup() => _confirmationPopupObject.SetActive(false);
    private void SetConfirmationOKEvent(Action handler) => OnConfirmationOK = handler;
    private void HideAmountInputPopup() => _amountInputPopupObject.SetActive(false);
    private void ShowConfirmationPopup(string itemName)
    {
        _confirmationItemNameText.text = itemName;
        _confirmationPopupObject.SetActive(true);
    }
    private void ShowAmountInputPopup(string itemName)
    {
        _amountInputItemNameText.text = itemName;
        _amountInputPopupObject.SetActive(true);
    }
    #endregion
    #region public Methods
    /// <summary> 확인/취소 팝업 띄우기 </summary>
    public void OpenConfirmationPopup(Action okCallback, string itemName)
    {
        ShowPanel();
        ShowConfirmationPopup(itemName);
        OnConfirmationOK = okCallback;
    }
    /// <summary> 수량 입력 팝업 띄우기 </summary>
    public void OpenAmountInputPopup(Action<int> okCallback, int currentAmount, string itemName)
    {
        _maxAmount = currentAmount - 1;
        _amountInputField.text = "1";

        ShowPanel();
        ShowAmountInputPopup(itemName);
        OnAmountInputOK = okCallback;
    }
    #endregion
}

