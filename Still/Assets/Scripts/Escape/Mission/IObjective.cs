namespace Still.Escape
{
    ///<summary>
    ///ミッションの目標の定義（MissionConfigにSerializeReferenceで並べる）
    ///</summary>
    // 定義はSOに保存されて再生のたびに使い回されるため、進み具合は持たせずTrackerに分ける
    public interface IObjective
    {
        ///<summary>
        ///この目標の進み具合を数えるTrackerを作る
        ///</summary>
        IObjectiveTracker CreateTracker();
    }

    ///<summary>
    ///目標1つ分の進み具合
    ///</summary>
    public interface IObjectiveTracker
    {
        ///<summary>達成したか</summary>
        bool IsCompleted { get; }

        ///<summary>
        ///チェックリストに出す文
        ///</summary>
        string Describe();

        ///<summary>
        ///プレイヤーが何かを調べたときの反応
        ///</summary>
        InteractOutcome HandleInteract(TargetId target);

        ///<summary>
        ///時間で進む目標の更新。表示が変わったら true
        ///</summary>
        bool Tick(float deltaTime, IMissionContext context);
    }

    ///<summary>
    ///時間で進む目標が参照するシーンの状態
    ///</summary>
    public interface IMissionContext
    {
        ///<summary>
        ///切り替え式の調べる物（蓄音機など）が動いているか
        ///</summary>
        bool IsTargetActive(TargetId target);

        ///<summary>
        ///指定した照明スイッチが点いているか
        ///</summary>
        bool IsLightOn(TargetId lightSwitch);
    }
}
