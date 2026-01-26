namespace Still.Enum.WorldStates
{
    public enum WorldStateType
    {
        // プレイヤー
        PlayerVisible,
        NearbyPlayer,

        // エネミー
        IsCatch,
        IsChasing,
        IsSearching,
        EnemyPatrol,
        DistanceFromGhost,

        // イベント
        EventCount,
    }
}