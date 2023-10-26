using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotUI : SlotUIBase
{
    [Tooltip("������ ���� �ؽ�Ʈ")]
    [SerializeField] private Text _amountText;

    //Text Ȱ��ȭ / ��Ȱ��ȭ
    private void ShowText() => _textGo.SetActive(true);
    private void HideText() => _textGo.SetActive(false);

    private GameObject _textGo;

    protected override void InitComponents()
    {
        _textGo = _amountText.gameObject;
        base.InitComponents();
    }


    /// <summary> ������ ���� �ؽ�Ʈ ����(amount�� 1 ������ ��� �ؽ�Ʈ ��ǥ�� </summary>
    public void SetItemAmount(int amount)
    {
        if (GetHasItem() && amount > 1)
            ShowText();
        else
            HideText();

        _amountText.text = amount.ToString();
    }
}
