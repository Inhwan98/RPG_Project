using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SKillSlotUI : SlotUIBase
{
    private Skill_InvenUI _skInvenUI;

    protected override void InitComponents()
    {
        _skInvenUI = GetComponentInParent<Skill_InvenUI>();
        base.InitComponents();
    }

    /// <summary> 아이템 활성화 / 비활성화 여부 설정  </summary>
    public void SetItemAccessibleState(bool value)
    {
        if (_isAccessibleItem == value) return;

        if (value)
        {
            _iconImage.color = Color.white;
        }
        else
        {
            _iconImage.color = InaccessibleIconColor;
        }

        _isAccessibleItem = value;
    }

}