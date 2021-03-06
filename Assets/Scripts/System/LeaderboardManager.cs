﻿using System;
using Duck.Http;
using Duck.Http.Service;
using HttpModel;
using Rythmformer;
using UnityEngine;

[Serializable]
public struct LeaderboardConfig
{
    public string RootUrl;
    public string Token;

    public string ApiRoot => $"{RootUrl}/api";
}

public class LeaderboardManager : MonoSingleton<LeaderboardManager>
{
    private GameManager _gm;

    [SerializeField] private LeaderboardConfig _config;

    public override void Init()
    {
        Http.SetSuperHeader("X-AUTH-TOKEN", _config.Token);
        _gm = GetComponent<GameManager>();
    }

    public IHttpRequest FetchBestScoreForLevel(World world, Level level)
    {
        Debug.Log($"[LEADERBOARD] - Fetching best score for {world} - {level}");
        return Http.Get(_config.ApiRoot + $"/score/{world}/{level}/best?type=score");
    }

    public IHttpRequest FetchBestTimerForLevel(World world, Level level)
    {
        Debug.Log($"[LEADERBOARD] - Fetching best time for {world} - {level}");
        return Http.Get(_config.ApiRoot + $"/score/{world}/{level}/best?type=timer");
    }

    public IHttpRequest PostTimerForLevel(World world, Level level, float timer)
    {
        Debug.Log($"[LEADERBOARD] - Posting timer for {world} - {level}");
        var entry = new NewScoreEntry()
        {
            identifier = SystemInfo.deviceUniqueIdentifier,
            timer = timer,
            score = 0,
            world = world,
            level = level,
        };
        var postData = JSONSerializer.Serialize(typeof(NewScoreEntry), entry);
        return Http
            .Put(_config.ApiRoot + $"/score/new", postData)
            .SetHeader("Content-Type", "application/json");
    }

    public IHttpRequest WakeServer()
    {
        Debug.Log("[LEADERBOARD] - Waking up backend...");
        return Http.Get(_config.RootUrl + $"/status/check")
            .OnError(res => Debug.LogError(res.Error))
            .OnSuccess(res =>
            {
                Debug.Log($"[LEADERBOARD] - {res.Text}");
            }).Send();
    }
}