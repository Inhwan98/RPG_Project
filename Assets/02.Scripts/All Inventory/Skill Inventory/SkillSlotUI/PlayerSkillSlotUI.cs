using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 SKILL은 쿨타임 정보를 가지고 있다.
/// </summary>
public class PlayerSkillSlotUI : SKillSlotUI
{
    [SerializeField]
    private Image skill_CoolTimeImage;

    [SerializeField]
    private TMPro.TMP_Text skill_CoolTimeText;

    public Image GetCoolTimeImage() => skill_CoolTimeImage;
    public TMPro.TMP_Text GetCoolTimeText() => skill_CoolTimeText;

    public void HideHighlightImage()
    {
        StopAllCoroutines();
        _highlightGo.SetActive(false);
    }
}
