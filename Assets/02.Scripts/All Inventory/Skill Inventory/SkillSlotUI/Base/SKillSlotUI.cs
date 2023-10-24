using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬 슬롯은 제한 레벨에 따라 비활성화 됌
/// </summary>
public abstract class SKillSlotUI : SlotUIBase
{
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