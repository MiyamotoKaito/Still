using Still.GOAP.Goal.Config;
using Still.GOAP.Planner.ExeCutor;
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
        public WorldStatesObserver(WorldStates worldStates,GoapExecutor executor, List<SubGoalConfig> goals)
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
        /// ゴールの
        /// </summary>
        /// <param name="config"></param>
        private void GoalSubscribe(SubGoalConfig config)
        {
            _worldStates.OnStateChanged
            .Subscribe(_ =>
            {
                if (CanChangeGoalPriority(config))
                {
                    _executor.UpdateGoalPriority(config, config.Priority);
                }
                else if (CheckConditions(config))
                {
                    ApplyGoalAchievedEffect(config);
                    _executor.UpdateGoalPriority(config, config.AchievedPriority);
                }
            }).AddTo(_disposables);
            _executor.UpdateGoalPriority(config, config.AchievedPriority);
        }
        private bool CanChangeGoalPriority(SubGoalConfig config)
        {
            if (_executor.CurrentGoal.Key == config) return false;

            var worldStates = _worldStates.CurrentStates;
            foreach (var condition in config.GetGoalsWorldStateSettings())
            {
                if (worldStates.TryGetValue(condition.Key, out int value))
                {
                    if (condition.Value != value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void ApplyGoalAchievedEffect(SubGoalConfig config)
        {
            foreach (var dic in config.GetGoalsEffect())
            {
                _worldStates.ModifyState(dic.Key, dic.Value);
            }

        }
        private bool CheckConditions(SubGoalConfig config)
        {
            if (_executor.CurrentGoal.Key != config) return false;

            var conditions = config.GetGoalsConditions();
            foreach (var condition in conditions)
            {
                if (condition.Value != _worldStates.GetStateValue(condition.Key))
                {
                    return false;
                }
            }
            return true;
        }
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}