using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    public void BackToMainMenu()
    {
        UIManager.instance.ToggleSettingsUI();
        UIManager.instance.ToggleMainMenuUI();
    }
}
