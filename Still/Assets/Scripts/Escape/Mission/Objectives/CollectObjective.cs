using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///散らばった物を順不同ですべて集める
    ///</summary>
    [Serializable]
    public class CollectObjective : IObjective
    {
        [SerializeField, Tooltip("チェックリストに出す文")] private string _label;
        [SerializeField, Tooltip("集める対象のID")] private List<string> _targetIds = new();

        ///<summary>
        ///進み具合を数えるTrackerを作る
        ///</summary>
        public IObjectiveTracker CreateTracker() => new Tracker(_label, _targetIds.Select(id => new TargetId(id)).ToArray());

        private sealed class Tracker : IObjectiveTracker
        {
            public Tracker(string label, IReadOnlyList<TargetId> targets)
            {
                _label = label;
                _targets = targets;
            }

            public bool IsCompleted => _collected.Count >= _targets.Count;

            private readonly string _label;
            private readonly IReadOnlyList<TargetId> _targets;
            private readonly HashSet<TargetId> _collected = new();

            public string Describe() => ChecklistFormat.Line(IsCompleted, $"{_label} ({_collected.Count}/{_targets.Count})");

            public InteractOutcome HandleInteract(TargetId target)
            {
                if (!_targets.Contains(target) || !_collected.Add(target)) return InteractOutcome.Ignored;
                return InteractOutcome.Progressed;
            }

            public bool Tick(float deltaTime, IMissionContext context) => false;
        }
    }
}
