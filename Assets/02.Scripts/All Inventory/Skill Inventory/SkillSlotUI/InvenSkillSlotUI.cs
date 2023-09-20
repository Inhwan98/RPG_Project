using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvenSkillSlotUI : SKillSlotUI

{
    [SerializeField] private Button _plusButton;
    [SerializeField] private Text _levelText;

    /// <summary> Slot이 갖는 스킬 레벨업 버튼 반환.
    /// <para/> Skill_InvenUI Awake에서 초기화 후 SkillManager 호출해 이벤트 함수 할당
    /// </summary>
    public Button GetPlusButton() => _plusButton;
    public Text GetLevelText() => _levelText;
    public void SetLevelText(int level) => _levelText.text = $"{level}";

}
