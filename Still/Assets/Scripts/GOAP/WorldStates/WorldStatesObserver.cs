using Still.GOAP.Goal.Config;
using Still.GOAP.Planner;
using Still.GOAP.Planner.Executor;
using System.Collections.Generic;
using UniRx;

namespace Still.GOAP.WorldState.Observer
{
    public class WorldStatesObserver
    {
        private WorldStates _worldStates;
        private GoapExecutor _executor;
        private List<SubGoalConfig> _goals;
        private CompositeDisposable _disposables = new CompositeDisposable();
        public WorldStatesObserver(WorldStates worldStates, GoapExecutor executor, List<SubGoalConfig> goals)
        {
            _worldStates = worldStates;
            _executor = executor;
            _goals = goals;
            foreach (var goal in goals)
            {
                GoalSubscribe(goal);
            }
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

            return true;
        }
        /// <summary>
        /// ゴールが達成された時の効果を反映する
        /// </summary>
        /// <param name="config"></param>
        private void ApplyGoalAchievedEffect(SubGoalConfig config)
        {
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

            return true;
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}