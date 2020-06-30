using System;

namespace HttpModel
{
    [Serializable]
    public struct ScoreEntry
    {
        public float timer;
        public int score;
        public World world;
        public Level level;
    }
    [Serializable]
    public struct NewScoreEntry
    {
        public string identifier;
        public float timer;
        public int score;
        public World world;
        public Level level;
    }
}