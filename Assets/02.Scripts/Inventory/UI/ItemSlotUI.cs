using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : SlotUIBase
{
    [Tooltip("아이템 개수 텍스트")]
    [SerializeField] private Text _amountText;


    //Text 활성화 / 비활성화
    private void ShowText() => _textGo.SetActive(true);
    private void HideText() => _textGo.SetActive(false);

    private GameObject _textGo;

    protected override void InitComponents()
    {
        _textGo = _amountText.gameObject;
        base.InitComponents();
    }

    /// <summary> 아이템 개수 텍스트 설정(amount가 1 이하일 경우 텍스트 미표시 </summary>
    public void SetItemAmount(int amount)
    {
        if (GetHasItem() && amount > 1)
            ShowText();
        else
            HideText();

        _amountText.text = amount.ToString();
    }


}