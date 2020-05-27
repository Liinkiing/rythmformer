using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RythmDebug : MonoBehaviour {
        [SerializeField]
    private Transform player;

    [SerializeField]
    private GameObject marker;
    
    private SongSynchronizer _synchronizer;
    private LevelManager _levelManager;
    
    private void Awake()
    {
        _synchronizer = Utils.FindObjectOfTypeOrThrow<SongSynchronizer>();
        _levelManager = Utils.FindObjectOfTypeOrThrow<LevelManager>();
    }
    
    private void OnEnable()
    {
        if (_synchronizer != null)
        {
            _synchronizer.StepThresholded += MakeMarker;
            _levelManager?.OnLevelReset.AddListener(OnLevelReset);
        }
    }

    private void OnDisable()
    {
        if (_synchronizer != null)
        {
            _synchronizer.StepThresholded -= MakeMarker;
            _levelManager?.OnLevelReset.RemoveListener(OnLevelReset);
        }
    }

    private void OnLevelReset()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void MakeMarker(SongSynchronizer sender, SongSynchronizer.EventState state)
    {
        if (state == SongSynchronizer.EventState.Mid)
        {
            var m = Instantiate(marker);
            m.transform.parent = transform;
            m.transform.position = player.position;
        }
    }
}
