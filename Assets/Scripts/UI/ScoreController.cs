using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private CharacterController2D player;
    [SerializeField] private TextMeshProUGUI uiScore;
    
    private Color32 _green = new Color32(102,187,106, 255);
    private Color32 _yellow = new Color32(198, 255, 0, 255);
    private Color32 _red = new Color32(216, 67, 21, 255);
    private Transform textReference;

    #endregion

    private void Awake()
    {
        player.ActionPerformed += PlayerOnActionPerformed;
        textReference = GameObject.Find("TextScore").transform;
    }

    private void OnDestroy()
    {
        player.ActionPerformed -= PlayerOnActionPerformed;
    }

    private void PlayerOnActionPerformed(CharacterController2D sender, CharacterController2D.OnActionEventArgs action)
    {
        TextMeshProUGUI text = Instantiate(uiScore, textReference.position, textReference.transform.rotation);
        text.enabled = false;
        text.transform.SetParent(transform);
        text.transform.localScale = new Vector3(1,1,1);

        switch (action.Score)
        {
            case SongSynchronizer.EventScore.Perfect:
                text.faceColor = _green;
                break;
            case SongSynchronizer.EventScore.Nice:
                text.faceColor = _yellow;
                break;
            case SongSynchronizer.EventScore.Failed:
                text.faceColor = _red;
                break;
        }

        text.SetText(action.Score.ToString());
        text.enabled = true;
    }
}
