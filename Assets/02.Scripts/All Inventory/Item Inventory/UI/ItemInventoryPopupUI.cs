using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> �κ��丮 UI ���� ��� ���� �˾��� ���� </summary>
public class ItemInventoryPopupUI : MonoBehaviour
{
    /*************************************************************
     *                           Fields
     ************************************************************/

    #region Ȯ��, ��� �˾�
    [Header("Confirmation Popup")]
    [SerializeField] private GameObject _confirmationPopupObject;
    [SerializeField] private Text       _confirmationItemNameText;
    [SerializeField] private Text       _confirmationText;
    [SerializeField] private Button     _confirmationOkButton;          //OK
    [SerializeField] private Button     _confirmationCancelButton;      //Cancel
    private event Action OnConfirmationOK; //Ȯ�� ��ư ���� ��� ������ �̺�Ʈ 
    #endregion

    #region ���� �Է� �˾�
    [Header("Amount Input Popup")]
    [SerializeField] private GameObject _amountInputPopupObject;
    [SerializeField] private Text _amountInputItemNameText;
    [SerializeField] private InputField _amountInputField;
    [SerializeField] private Button _amountPlusButton;        // +
    [SerializeField] private Button _amountMinusButton;       // -
    [SerializeField] private Button _amountInputOkButton;     // Ok
    [SerializeField] private Button _amountInputCancelButton; // Cancel

    // Ȯ�� ��ư ������ �� ������ �̺�Ʈ
    private event Action<int> OnAmountInputOK;
    // ���� �Է� ���� ����
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
        // 1. Ȯ�� ��ư ���� ��� �̺�Ʈ
        _confirmationOkButton.onClick.AddListener(HidePanel);
        _confirmationOkButton.onClick.AddListener(HideConfirmationPopup);
        _confirmationOkButton.onClick.AddListener(() => OnConfirmationOK?.Invoke());

        // 2. ��� ��ư ���� ��� �̺�Ʈ
        _confirmationCancelButton.onClick.AddListener(HidePanel);
        _confirmationCancelButton.onClick.AddListener(HideConfirmationPopup);


        _amountInputOkButton.onClick.AddListener(HidePanel);
        _amountInputOkButton.onClick.AddListener(HideAmountInputPopup);
        _amountInputOkButton.onClick.AddListener(() => OnAmountInputOK?.Invoke(int.Parse(_amountInputField.text)));

        _amountInputCancelButton.onClick.AddListener(HidePanel);
        _amountInputCancelButton.onClick.AddListener(HideAmountInputPopup);

        // [-] ��ư �̺�Ʈ
        _amountMinusButton.onClick.AddListener(() =>
        {
            // TryParse : ���ڿ� => ���������� ��ȯ�ϰ� bool���� ��ȯ. ��ȯ�� ������ out�� ���� �����Ѵ�.
            int.TryParse(_amountInputField.text, out int amount);
            if (amount > 1)
            {
                // Shift ������ 10�� ����
                int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount - 10 : amount - 1;
                if (nextAmount < 1)
                    nextAmount = 1;
                _amountInputField.text = nextAmount.ToString(); // ������ �ٽ� ���ڿ��� ��ȯ
            }
        });

        // [+] ��ư �̺�Ʈ
        _amountPlusButton.onClick.AddListener(() =>
        {
            int.TryParse(_amountInputField.text, out int amount);
            if (amount < _maxAmount)
            {
                // Shift ������ 10�� ����
                int nextAmount = Input.GetKey(KeyCode.LeftShift) ? amount + 10 : amount + 1;
                if (nextAmount > _maxAmount)
                    nextAmount = _maxAmount;
                _amountInputField.text = nextAmount.ToString();
            }
        });

        // �Է� �� ���� ����
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
    /// <summary> Ȯ��/��� �˾� ���� </summary>
    public void OpenConfirmationPopup(Action okCallback, string itemName)
    {
        ShowPanel();
        ShowConfirmationPopup(itemName);
        OnConfirmationOK = okCallback;
    }
    /// <summary> ���� �Է� �˾� ���� </summary>
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

