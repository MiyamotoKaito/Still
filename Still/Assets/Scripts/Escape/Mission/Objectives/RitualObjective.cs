using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///切り替え式の物（蓄音機など）を、照明が点いた状態で一定時間動かし続ける
    ///</summary>
    // 進行中はワールドステート RitualActive が立ち、ゴーストが照明を消しに来る
    [Serializable]
    public class RitualObjective : IObjective
    {
        [SerializeField, Tooltip("チェックリストに出す文")] private string _label;
        [SerializeField, Tooltip("動かし続ける切り替え式の物のID")] private string _activatorTargetId;
        [SerializeField, Tooltip("点いていなければならない照明スイッチのID")] private List<string> _requiredLightIds = new();
        [SerializeField, Tooltip("動かし続ける秒数")] private float _durationSeconds = 30f;

        ///<summary>
        ///進み具合を数えるTrackerを作る
        ///</summary>
        public IObjectiveTracker CreateTracker()
        {
            return new RitualTracker(_label, new TargetId(_activatorTargetId),
                                     _requiredLightIds.Select(id => new TargetId(id)).ToArray(), _durationSeconds);
        }
    }

    ///<summary>
    ///儀式の進み具合
    ///</summary>
    public sealed class RitualTracker : IObjectiveTracker
    {
        public RitualTracker(string label, TargetId activator, IReadOnlyList<TargetId> requiredLights, float durationSeconds)
        {
            _label = label;
            _activator = activator;
            _requiredLights = requiredLights;
            _durationSeconds = durationSeconds;
        }

        ///<summary>達成したか</summary>
        public bool IsCompleted => _elapsed >= _durationSeconds;
        ///<summary>儀式が進行中か（ゴーストが邪魔しに来る）</summary>
        public bool IsRunning => !IsCompleted && _isActivatorOn;

        private float _elapsed;
        private bool _isActivatorOn;
        private bool _isLit;

        private readonly string _label;
        private readonly TargetId _activator;
        private readonly IReadOnlyList<TargetId> _requiredLights;
        private readonly float _durationSeconds;

        ///<summary>
        ///チェックリストに出す文
        ///</summary>
        public string Describe()
        {
            var line = $"{_label} ({Mathf.FloorToInt(_elapsed)}/{Mathf.CeilToInt(_durationSeconds)}s)";
            if (!IsCompleted && _isActivatorOn && !_isLit) line += " - 明かりが消えている";
            return ChecklistFormat.Line(IsCompleted, line);
        }

        ///<summary>
        ///調べただけでは進まない（切り替えはView側で行う）
        ///</summary>
        public InteractOutcome HandleInteract(TargetId target) => InteractOutcome.Ignored;

        ///<summary>
        ///動いていて明るい間だけ時間を進める
        ///</summary>
        public bool Tick(float deltaTime, IMissionContext context)
        {
            if (IsCompleted) return false;

            bool wasOn = _isActivatorOn, wasLit = _isLit;
            int wasSeconds = Mathf.FloorToInt(_elapsed);

            _isActivatorOn = context.IsTargetActive(_activator);
            _isLit = _requiredLights.All(context.IsLightOn);
            if (_isActivatorOn && _isLit) _elapsed = Mathf.Min(_elapsed + deltaTime, _durationSeconds);

            return wasOn != _isActivatorOn || wasLit != _isLit || wasSeconds != Mathf.FloorToInt(_elapsed);
        }
    }
}
