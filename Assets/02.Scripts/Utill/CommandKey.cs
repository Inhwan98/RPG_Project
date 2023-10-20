using System.Collections.Generic;
using UnityEngine;


public enum ActionType
{
    Inventory,
    SkillWindow,
    StatusWindow
}

public static class Commandkey
{
    // Dictionary to store KeyCode for different actions
    private static Dictionary<ActionType, KeyCode> keyBindings = new Dictionary<ActionType, KeyCode>();

    static Commandkey()
    {
        keyBindings[ActionType.Inventory] = KeyCode.I;
        keyBindings[ActionType.SkillWindow] = KeyCode.K;
        keyBindings[ActionType.StatusWindow] = KeyCode.P;
    }

    public static void CheckInput(ActionType actionType, ref bool isUseWindow, BaseInvenManager invenWindowManager)
    {
        if (Input.GetKeyDown(keyBindings[actionType]))
        {
            isUseWindow = !isUseWindow;
            invenWindowManager.SetWindowActive(isUseWindow);
            PlayerController.instance.CheckFreezController();
            PlayerController.instance.PlayerIdleState();
        }
    }



}

