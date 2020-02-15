using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private CharacterController2D player;
    [SerializeField] private TextMeshProUGUI uiScore;
    
    #endregion

    private void Awake()
    {
        player.ActionPerformed += PlayerOnActionPerformed; 
    }

    private void OnDestroy()
    {
        player.ActionPerformed -= PlayerOnActionPerformed;
    }

    private void PlayerOnActionPerformed(CharacterController2D sender, CharacterController2D.OnActionEventArgs action)
    {
        uiScore.SetText(action.Score.ToString());
        uiScore.enabled = true;
        StartCoroutine(DoDisableScore());
    }

    IEnumerator DoDisableScore()
    {
        yield return new WaitForSeconds(1);
        uiScore.enabled = false;
    }
}
