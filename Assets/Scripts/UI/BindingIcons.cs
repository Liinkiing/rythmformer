using System;
using UnityEngine;

public class BindingIcons : MonoBehaviour
{
    [Serializable]
    public struct BindingIcon
    {
        public Sprite Keyboard;
        public Sprite Gamepad;
    }

    public BindingIcon Jump;
    public BindingIcon Dash;
    public BindingIcon Retry;
    public BindingIcon SwitchMode;

    public Sprite GetSprite(BindingScheme change, GameAction action)
    {
        switch (action)
        {
            case GameAction.Dash:
                return change == BindingScheme.Gamepad ? Dash.Gamepad : Dash.Keyboard;
            case GameAction.Retry:
                return change == BindingScheme.Gamepad ? Retry.Gamepad : Retry.Keyboard;
            case GameAction.Jump:
                return change == BindingScheme.Gamepad ? Jump.Gamepad : Jump.Keyboard;
            case GameAction.SwitchMode:
                return change == BindingScheme.Gamepad ? SwitchMode.Gamepad : SwitchMode.Keyboard;
        }

        Debug.LogWarning(
            $"Could not find sprite for action {action.ToString()} in {change.ToString()} keymap. Defaulting it to keyboard jump sprite");
        return Jump.Keyboard;
    }
}