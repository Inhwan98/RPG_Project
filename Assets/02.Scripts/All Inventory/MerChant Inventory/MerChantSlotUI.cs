using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MerChantSlotUI : SlotUIBase
{
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _itemLevelText;
    [SerializeField] private TMP_Text _itemPriceText;

    [SerializeField] private Button _buyButton;

    public Button GetBuyButton() => _buyButton;


    public void MerChantSlotUI_Init(ItemData slotData)
    {
        if(slotData == null)
        {
            _itemNameText.text = "준비중";
            _itemLevelText.text = "";
            _itemPriceText.text = "";
            return;
        }
        _itemNameText.text = slotData.GetName();
        _itemLevelText.text = $"Lv. {slotData.GetUsedLevel().ToString()}";
        _itemPriceText.text = slotData.GetPrice().ToString();
            
    }

}