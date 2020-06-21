using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private CharacterController2D player;
    [SerializeField] private SpriteRenderer uiScore;
    [SerializeField] private Sprite perfect;
    [SerializeField] private Sprite nice;
    [SerializeField] private Sprite failed;
    
    private Transform spriteReference;

    #endregion

    private void Awake()
    {
        player.ActionPerformed += PlayerOnActionPerformed;
        spriteReference = GameObject.Find("SpriteScore").transform;
    }

    private void OnDestroy()
    {
        player.ActionPerformed -= PlayerOnActionPerformed;
    }

    private void PlayerOnActionPerformed(CharacterController2D sender, CharacterController2D.OnActionEventArgs action)
    {
        var spriteRenderer = Instantiate(uiScore, spriteReference.transform.position, spriteReference.transform.rotation);
        spriteRenderer.enabled = false;
        spriteRenderer.transform.SetParent(transform);
        spriteRenderer.transform.localScale = new Vector3(80f,80f,1);

        switch (action.Score)
        {
            case SongSynchronizer.EventScore.Perfect:
                spriteRenderer.sprite = perfect;
                break;
            case SongSynchronizer.EventScore.Nice:
                spriteRenderer.sprite = nice;
                break;
            case SongSynchronizer.EventScore.Failed:
                spriteRenderer.sprite = failed;
                break;
        }

        spriteRenderer.enabled = true;
    }
}
