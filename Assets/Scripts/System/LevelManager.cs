using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private SongSynchronizer songSynchronizer;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private CharacterController2D player;

    public void ResetLevel()
    {
        player.transform.position = playerSpawn.position;
        songSynchronizer.ResetSync();
    }
}