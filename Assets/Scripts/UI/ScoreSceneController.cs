using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSceneController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private GameObject _sceneTransition;

    private void Awake()
    {
        if (SaveManager.instance.Data.Difficulty == Difficulty.Chill)
        {
            ScoreText.gameObject.SetActive(false);
        }
    }
    
    public void BackToChapter()
    {
        StartCoroutine(_sceneTransition.GetComponent<SceneLoader>().LoadLevel("LevelSelector"));
    }
}
