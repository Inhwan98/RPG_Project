using System.Collections.Generic;
using UnityEngine;


public enum ActionType
{
    Inventory,
    SkillWindow,
    StatusWindow,
    QuickSlotA,
    QuickSlotB
    
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
        keyBindings[ActionType.QuickSlotA] = KeyCode.Z;
        keyBindings[ActionType.QuickSlotB] = KeyCode.X;
    
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

    public static void UseQuickSlot(ActionType actionType, QuickInvenManager quickInvenMgr, int index)
    {
        if(Input.GetKeyDown(keyBindings[actionType]))
        {
            quickInvenMgr.Use(index);
        }
    }



}

