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
                // 現在のゴールが達成できるか？
                if (CheckConditions(config))
                {
                    Debug.Log($"{config.name}が達成されたので優先度を下げた");
                    ApplyGoalAchievedEffect(config);
                    _executor.UpdateGoalPriority(config, config.AchievedPriority);
                    return;
                }

                // 現在実行中のゴールは優先度変更の対象外とする
                if (_executor.CurrentGoal.Key == config && _executor.CurrentGoal.Value == config.Priority)
                {
                    // 現在実行中のゴールはスキップ（優先度を変更しない）
                    return;
                }

                // 最も優先度が高い以外のゴールでゴールをセットする前提条件が達成されていたら
                if (CanChangeGoalPriority(config))
                {
                    Debug.Log($"{config.name}の優先度を上げた");
                    _executor.UpdateGoalPriority(config, config.Priority);
                }
                else
                {
                    if (_executor.Goals[config] != config.AchievedPriority)
                    {
                        Debug.Log($"{config.name}の達成条件が満たされなくなったので優先度を下げた");
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
            if (_executor.CurrentGoal.Key == config　&& _executor.CurrentGoal.Value == config.Priority) return false;

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
            Debug.Log("ゴールが達成されたのでワールドステートの値を書き換える");
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