using System;
using System.Collections.Generic;
using R3;
using Still.Common;
using UnityEngine;
using VContainer.Unity;

namespace Still.Goap
{
    ///<summary>
    ///ワールドステートの変化を見て、各ゴールの優先度を上げ下げする
    ///</summary>
    public sealed class WorldStatesObserver : IInitializable, IDisposable
    {
        public WorldStatesObserver(IWorldStates worldStates, GoapExecutor executor, List<SubGoalConfig> goals)
        {
            _worldStates = worldStates;
            _executor = executor;
            _goals = goals;
        }

        private readonly IWorldStates _worldStates;
        private readonly GoapExecutor _executor;
        private readonly IReadOnlyList<SubGoalConfig> _goals;
        private readonly CompositeDisposable _disposables = new();

        ///<summary>
        ///全ゴールの監視を始める
        ///</summary>
        public void Initialize()
        {
            foreach (var goal in _goals)
            {
                _worldStates.OnStateChanged
                    .Subscribe(goal, (_, g) => Evaluate(g))
                    .AddTo(_disposables);
            }
        }

        ///<summary>
        ///監視をやめる
        ///</summary>
        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void Evaluate(SubGoalConfig goal)
        {
            var states = _worldStates.CurrentStates;
            bool isCurrent = _executor.CurrentGoal == goal;

            if (isCurrent && Evaluation.IsSatisfied(states, goal.GetGoalsConditions()))
            {
                Debug.Log($"[WorldStatesObserver] {goal.name} を達成したので優先度を下げる");
                foreach (var effect in goal.GetGoalsEffect()) _worldStates.ModifyState(effect.Key, effect.Value);
                _executor.UpdateGoalPriority(goal, goal.AchievedPriority);
                return;
            }

            // 実行中で発動済みのゴールは、達成するまで優先度を動かさない
            if (isCurrent && _executor.GetPriority(goal) == goal.Priority) return;

            if (Evaluation.IsSatisfied(states, goal.GetGoalsWorldStateSettings()))
            {
                if (_executor.GetPriority(goal) == goal.Priority) return;
                Debug.Log($"[WorldStatesObserver] {goal.name} の発動条件を満たしたので優先度を上げる");
                _executor.UpdateGoalPriority(goal, goal.Priority);
                return;
            }

            if (_executor.GetPriority(goal) != goal.AchievedPriority)
            {
                Debug.Log($"[WorldStatesObserver] {goal.name} の発動条件が崩れたので優先度を下げる");
                _executor.UpdateGoalPriority(goal, goal.AchievedPriority);
            }
        }
    }
}
