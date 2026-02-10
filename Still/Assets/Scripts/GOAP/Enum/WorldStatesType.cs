namespace Still.Enum.WorldStates
{
    public enum WorldStateType
    {
        // プレイヤー関連
        PlayerVisible,
        NearbyPlayer,

        // エネミー自身の状態
        IsCatch,
        IsChasing,
        LookAround,
        EnemyPatrol,

        // ターゲットとコンテキスト
        HasTarget,
        IsAtTarget,
        IsAtLightSwitch,

        // オブジェクト・環境の状態
        OnLighting,
        TurnOffLight,
        OnLightSwitchIsBroken,
        HeardSound,

        // イベント
        EventCount,
        InTheRoom,
        FearLevel,
        Knock,

    }
}