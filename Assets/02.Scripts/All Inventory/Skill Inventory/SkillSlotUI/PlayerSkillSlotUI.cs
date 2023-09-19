using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerSkillSlotUI : SKillSlotUI
{
    [SerializeField]
    private Image skill_CoolTimeImage;

    [SerializeField]
    private TMPro.TMP_Text skill_CoolTimeText;

    public Image GetCoolTimeImage() => skill_CoolTimeImage;
    public TMPro.TMP_Text GetCoolTimeText() => skill_CoolTimeText;

}
