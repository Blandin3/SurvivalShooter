public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public static class DifficultySettings
{
    public static Difficulty Current = Difficulty.Normal;

    public static float EnemyHealthMultiplier
    {
        get
        {
            switch (Current)
            {
                case Difficulty.Easy: return 0.75f;
                case Difficulty.Hard: return 1.5f;
                default: return 1f;
            }
        }
    }

    public static float EnemyDamageMultiplier
    {
        get
        {
            switch (Current)
            {
                case Difficulty.Easy: return 0.75f;
                case Difficulty.Hard: return 1.5f;
                default: return 1f;
            }
        }
    }

    // Multiplies the spawner's wait time between spawns, so Hard spawns more often (smaller multiplier).
    public static float SpawnIntervalMultiplier
    {
        get
        {
            switch (Current)
            {
                case Difficulty.Easy: return 1.3f;
                case Difficulty.Hard: return 0.6f;
                default: return 1f;
            }
        }
    }
}
