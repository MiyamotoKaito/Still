using System;
using System.Collections.Generic;
using R3;

namespace Still.Escape
{
    ///<summary>
    ///いまのステージと各目標の進み具合、開けられるドア
    ///</summary>
    public sealed class MissionEntity : IDisposable
    {
        ///<summary>いまのステージ番号（全ステージ達成後はステージ数と同じ）</summary>
        public ReadOnlyReactiveProperty<int> StageIndex => _stageIndex;
        ///<summary>いまのステージの目標の進み具合</summary>
        public IReadOnlyList<IObjectiveTracker> Trackers => _trackers;
        ///<summary>進み具合が変わった（チェックリストの更新用）</summary>
        public Observable<Unit> OnProgressChanged => _onProgressChanged;
        ///<summary>開けられるドアが増えた</summary>
        public Observable<TargetId> OnDoorUnlocked => _onDoorUnlocked;
        ///<summary>隠れていた調べる物を出現させる</summary>
        public Observable<TargetId> OnTargetRevealed => _onTargetRevealed;

        private IReadOnlyList<IObjectiveTracker> _trackers = Array.Empty<IObjectiveTracker>();

        private readonly ReactiveProperty<int> _stageIndex = new(-1);
        private readonly HashSet<TargetId> _unlockedDoors = new();
        private readonly HashSet<TargetId> _activeTargets = new();
        private readonly Subject<Unit> _onProgressChanged = new();
        private readonly Subject<TargetId> _onDoorUnlocked = new();
        private readonly Subject<TargetId> _onTargetRevealed = new();

        ///<summary>
        ///ステージを切り替える
        ///</summary>
        public void SetStage(int stageIndex, IReadOnlyList<IObjectiveTracker> trackers)
        {
            _trackers = trackers;
            _stageIndex.Value = stageIndex;
            NotifyProgressChanged();
        }

        ///<summary>
        ///進み具合が変わったことを知らせる
        ///</summary>
        public void NotifyProgressChanged() => _onProgressChanged.OnNext(Unit.Default);

        ///<summary>
        ///ドアを開けられるようにする
        ///</summary>
        public void UnlockDoor(TargetId door)
        {
            if (_unlockedDoors.Add(door)) _onDoorUnlocked.OnNext(door);
        }

        ///<summary>
        ///ドアを開けられるか
        ///</summary>
        public bool IsDoorUnlocked(TargetId door) => _unlockedDoors.Contains(door);

        ///<summary>
        ///隠れていた調べる物の出現を知らせる
        ///</summary>
        public void RevealTarget(TargetId target) => _onTargetRevealed.OnNext(target);

        ///<summary>
        ///切り替え式の物が動いているかを記録する
        ///</summary>
        public void SetTargetActive(TargetId target, bool isActive)
        {
            if (isActive) _activeTargets.Add(target);
            else _activeTargets.Remove(target);
        }

        ///<summary>
        ///切り替え式の物が動いているか
        ///</summary>
        public bool IsTargetActive(TargetId target) => _activeTargets.Contains(target);

        ///<summary>
        ///破棄する
        ///</summary>
        public void Dispose()
        {
            _stageIndex.Dispose();
            _onProgressChanged.Dispose();
            _onDoorUnlocked.Dispose();
            _onTargetRevealed.Dispose();
        }
    }
}
