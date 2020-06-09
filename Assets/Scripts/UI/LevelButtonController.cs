using UnityEngine;
using UnityEngine.EventSystems;

public class LevelButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private LevelSelector _levelSelectorController;

    private void Awake()
    {
        _levelSelectorController = GameObject.Find("LevelSelector UI").GetComponent<LevelSelector>();
    }

    public void OnSelect(BaseEventData data)
    {

        int indexSelectedButton = _levelSelectorController._levelButtons.FindIndex(0, o => o == data.selectedObject);
        _levelSelectorController.AnimateSun(indexSelectedButton);
    }
    
    public void OnDeselect(BaseEventData data)
    {
        _levelSelectorController.lastSelectedLevelButton = gameObject;
    }
}
