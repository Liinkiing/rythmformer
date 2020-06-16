using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class PrincessFoot : MonoBehaviour
{
    [SerializeField] private AudioClip footstepStoneSfx;
    [SerializeField] private AudioClip footstepGrassSfx;
    [SerializeField] private Animator playerAnimator;
    private AudioSource source;
    private LevelManager _levelManager;
    private const float DelayBetweenSounds = 0.5f;
    private float minTime = DelayBetweenSounds;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
    }

    private void Update()
    {
        minTime -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("anim_run") && minTime < 0f)
        {
            source.pitch = Random.Range(0.9f, 1.2f);
            source.PlayOneShot(_levelManager.Config.Floor == LevelManager.FloorType.Grass ? footstepGrassSfx : footstepStoneSfx, 0.5f);
            minTime = DelayBetweenSounds;
        }
    }
}