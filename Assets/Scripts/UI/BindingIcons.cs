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
    public BindingIcon SkipCutscene;

    public Sprite GetSprite(BindingScheme scheme, GameAction action)
    {
        switch (action)
        {
            case GameAction.Dash:
                return scheme == BindingScheme.Gamepad ? Dash.Gamepad : Dash.Keyboard;
            case GameAction.Retry:
                return scheme == BindingScheme.Gamepad ? Retry.Gamepad : Retry.Keyboard;
            case GameAction.Jump:
                return scheme == BindingScheme.Gamepad ? Jump.Gamepad : Jump.Keyboard;
            case GameAction.SwitchMode:
                return scheme == BindingScheme.Gamepad ? SwitchMode.Gamepad : SwitchMode.Keyboard;
            case GameAction.SkipCutscene:
                return scheme == BindingScheme.Gamepad ? SkipCutscene.Gamepad : SkipCutscene.Keyboard;
        }

        Debug.LogWarning(
            $"Could not find sprite for action {action.ToString()} in {scheme.ToString()} keymap. Defaulting it to keyboard jump sprite");
        return Jump.Keyboard;
    }
}