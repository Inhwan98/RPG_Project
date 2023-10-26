using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public enum ActionType
{
    Inventory,
    SkillWindow,
    StatusWindow,
    QuickSlotA,
    QuickSlotB,
    SKILLA,
    SKILLB,
    SKILLC,
    SKILLD,
    SKILLE,
    SKILLF
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

        keyBindings[ActionType.SKILLA] = KeyCode.Alpha1;
        keyBindings[ActionType.SKILLB] = KeyCode.Alpha2;
        keyBindings[ActionType.SKILLC] = KeyCode.Alpha3;
        keyBindings[ActionType.SKILLD] = KeyCode.Alpha4;
        keyBindings[ActionType.SKILLE] = KeyCode.Alpha5;
        keyBindings[ActionType.SKILLF] = KeyCode.Alpha6;

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

    public static void UseQuickSlot(ActionType actionType, System.Action quickUseAction)
    {
        if(Input.GetKeyDown(keyBindings[actionType]))
        {
            quickUseAction.Invoke();
        }
    }

    public static void UseSkillAttack(ActionType actionType, System.Action skillAction )
    {
        if (Input.GetKeyDown(keyBindings[actionType]))
        {
            skillAction.Invoke();
        }
    }



}

