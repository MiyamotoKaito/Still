using System.Collections.Generic;
using Still.GOAP.Goal.Config;
using Still.GOAP.Planner;
using Still.GOAP.Planner.Executor;
using UniRx;
using UnityEngine;

namespace Still.GOAP.WorldState.Observer
{
    public class WorldStatesObserver
    {
        private readonly WorldStates _worldStates;
        private readonly GoapExecutor _executor;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        public WorldStatesObserver(WorldStates worldStates, GoapExecutor executor, List<SubGoalConfig> goals)
        {
            _worldStates = worldStates;
            _executor = executor;
            foreach (var goal in goals)
            {
                GoalSubscribe(goal);
            }
            Debug.Log($"WorldStatesObserverを生成");
        }
        /// <summary>
        /// ゴールの登録
        /// </summary>
        /// <param name="config"></param>
        private void GoalSubscribe(SubGoalConfig config)
        {
            _worldStates.OnStateChanged
            .Subscribe(_ =>
            {
                if (CheckConditions(config))
                {
                    ApplyGoalAchievedEffect(config);
                    _executor.UpdateGoalPriority(config, config.AchievedPriority);
                    return;
                }
                if (CanChangeGoalPriority(config))
                {
                    _executor.UpdateGoalPriority(config, config.Priority);
                }
                else
                {
                    if (_executor.Goals[config] != config.AchievedPriority)
                    {
                        _executor.UpdateGoalPriority(config, config.AchievedPriority);
                    }
                }
            }).AddTo(_disposables);
            Debug.Log("ゴールを購読");
            _executor.UpdateGoalPriority(config, config.AchievedPriority);
        }
        /// <summary>
        /// ゴールを設定する前提条件がクリアされていたら
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private bool CanChangeGoalPriority(SubGoalConfig config)
        {
            if (_executor.CurrentGoal.Key == config　&& _executor.Goals[config] == config.Priority) return false;

            var worldStates = _worldStates.CurrentStates;
            if (!Evalution.IsSatisfied(worldStates, config.GetGoalsWorldStateSettings())) return false;

            Debug.Log("ゴールを設定する");
            return true;
        }
        /// <summary>
        /// ゴールが達成された時の効果を反映する
        /// </summary>
        /// <param name="config"></param>
        private void ApplyGoalAchievedEffect(SubGoalConfig config)
        {
            Debug.Log("ゴールが達成された");
            foreach (var dic in config.GetGoalsEffect())
            {
                _worldStates.ModifyState(dic.Key, dic.Value);
            }
        }
        /// <summary>
        /// ゴールの達成条件がクリアされていたら
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private bool CheckConditions(SubGoalConfig config)
        {
            if (_executor.CurrentGoal.Key != config) return false;

            var worldStates = _worldStates.CurrentStates;
            var conditions = config.GetGoalsConditions();
            if (!Evalution.IsSatisfied(worldStates, conditions)) return false;

            Debug.Log("ゴールの達成条件がクリアされた");
            return true;
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}