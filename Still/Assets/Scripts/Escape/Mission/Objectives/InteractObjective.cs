using System;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///指定した物を1回調べる
    ///</summary>
    [Serializable]
    public class InteractObjective : IObjective
    {
        [SerializeField, Tooltip("チェックリストに出す文")] private string _label;
        [SerializeField, Tooltip("調べる対象のID")] private string _targetId;

        ///<summary>
        ///進み具合を数えるTrackerを作る
        ///</summary>
        public IObjectiveTracker CreateTracker() => new Tracker(_label, new TargetId(_targetId));

        private sealed class Tracker : IObjectiveTracker
        {
            public Tracker(string label, TargetId target)
            {
                _label = label;
                _target = target;
            }

            public bool IsCompleted { get; private set; }

            private readonly string _label;
            private readonly TargetId _target;

            public string Describe() => ChecklistFormat.Line(IsCompleted, _label);

            public InteractOutcome HandleInteract(TargetId target)
            {
                if (IsCompleted || target != _target) return InteractOutcome.Ignored;

                IsCompleted = true;
                return InteractOutcome.Progressed;
            }

            public bool Tick(float deltaTime, IMissionContext context) => false;
        }
    }
}
