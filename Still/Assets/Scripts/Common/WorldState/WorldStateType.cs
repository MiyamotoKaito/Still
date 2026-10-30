namespace Still.Common
{
    ///<summary>
    ///GOAPのワールドステートのキー
    ///</summary>
    // アセットには数値で保存されるので、並びを変えたり途中に追加したりしない
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
        Arrived,
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

        GhostDetected,
        FocusMe,
        Teleport,
    }
}
