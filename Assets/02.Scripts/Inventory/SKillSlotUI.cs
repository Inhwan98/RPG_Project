using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SKillSlotUI : SlotUIBase
{
    private SkillTreeUI _skillTreeUI;

    protected override void InitComponents()
    {
        _skillTreeUI = GetComponentInParent<SkillTreeUI>();
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