using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///決められた順番で調べる。間違えると最初からやり直し
    ///</summary>
    [Serializable]
    public class SequenceObjective : IObjective
    {
        [SerializeField, Tooltip("チェックリストに出す文（順番のヒントを含めてよい）")] private string _label;
        [SerializeField, Tooltip("調べる順番どおりに並べた対象のID")] private List<string> _orderedTargetIds = new();
        [SerializeField, Tooltip("間違えたときに減るSAN値")] private float _mistakeSanDamage = 10f;
        [SerializeField, Tooltip("間違えたときに鳴らすSE")] private string _mistakeSe = "Knock";

        ///<summary>
        ///進み具合を数えるTrackerを作る
        ///</summary>
        public IObjectiveTracker CreateTracker()
        {
            return new Tracker(_label, _orderedTargetIds.Select(id => new TargetId(id)).ToArray(), _mistakeSanDamage, _mistakeSe);
        }

        private sealed class Tracker : IObjectiveTracker
        {
            public Tracker(string label, IReadOnlyList<TargetId> order, float mistakeSanDamage, string mistakeSe)
            {
                _label = label;
                _order = order;
                _mistakeSanDamage = mistakeSanDamage;
                _mistakeSe = mistakeSe;
            }

            public bool IsCompleted => _nextIndex >= _order.Count;

            private int _nextIndex;

            private readonly string _label;
            private readonly IReadOnlyList<TargetId> _order;
            private readonly float _mistakeSanDamage;
            private readonly string _mistakeSe;

            public string Describe() => ChecklistFormat.Line(IsCompleted, $"{_label} ({_nextIndex}/{_order.Count})");

            public InteractOutcome HandleInteract(TargetId target)
            {
                if (IsCompleted || !_order.Contains(target)) return InteractOutcome.Ignored;

                if (_order[_nextIndex] == target)
                {
                    _nextIndex++;
                    return InteractOutcome.Progressed;
                }

                _nextIndex = 0;
                return InteractOutcome.Mistake(_mistakeSanDamage, _mistakeSe);
            }

            public bool Tick(float deltaTime, IMissionContext context) => false;
        }
    }
}
