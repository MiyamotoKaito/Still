namespace Still.Enum.WorldStates
{
    public enum WorldStateType
    {
        // プレイヤー関連
        PlayerVisible,
        NearbyPlayer,
        LastKnownPlayerLocationID,

        // エネミー自身の状態
        IsCatch,
        IsChasing,
        IsSearching,
        EnemyPatrol,
        DistanceFromGhost,
        IsAlerted,

        // ターゲットとコンテキスト
        CurrentTargetID,
        HasTarget,
        IsAtTarget,
        IsAtLightSwitch,
        IsInRangeOftarget,
        CurrentTargetIsPlayer,
        CurrentTagetIsLightSwitch,
        CurrentTargeIsSound,

        // オブジェクト・環境の状態
        OnLighting,
        OnLightSwitchIsBroken,
        HeardSound,

        // イベント
        EventCount,
    }
}