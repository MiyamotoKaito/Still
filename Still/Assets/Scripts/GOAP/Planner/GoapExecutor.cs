using System.Collections.Generic;
using System.Linq;
using Still.GOAP.Action;
using Still.GOAP.Agent;
using Still.GOAP.Goal.Config;
using Still.GOAP.WorldState;
using UniRx;
using UnityEngine;
namespace Still.GOAP.Planner.Executor
{
    public class GoapExecutor
    {
        public KeyValuePair<SubGoalConfig, int> CurrentGoal => _currentGoal.Value;
        public Dictionary<SubGoalConfig, int> Goals => _goals;
        /// <summary>全てのゴールをまとめる辞書</summary>
        private readonly Dictionary<SubGoalConfig, int> _goals = new();
        /// <summary>現在最も優先度が高いゴール</summary>
        private ReactiveProperty<KeyValuePair<SubGoalConfig, int>> _currentGoal = new();
        [SerializeField,SubclassSelector]
        private IAction _currentAction;
        private Stack<IAction> _routeActions = new();
        private readonly List<IAction> _usableActions = new();
        private WorldStates _worldStates;
        // 初期化完了フラグ
        private bool _isStarted = false;

        public GoapExecutor(List<IAction> usableActions, WorldStates worldStates, List<SubGoalConfig> goals)
        {
            if (usableActions == null || worldStates == null || goals == null)
                Debug.Log("なんか入ってない");

            Debug.Log("Executorが呼ばれた");

            _usableActions = usableActions;
            _worldStates = worldStates;
            _goals = InitGoals(goals);
            _currentGoal
                .Where(x => x.Key != null)
                .Subscribe(x =>
                {
                    if (_isStarted)
                    {
                        Plan(x.Key);
                    }
                });

        }
        /// <summary>
        /// ゴールの初期化
        /// </summary>
        /// <param name="goals"></param>
        /// <returns></returns>
        private Dictionary<SubGoalConfig, int> InitGoals(List<SubGoalConfig> goals)
        {
            var dic = new Dictionary<SubGoalConfig, int>();
            foreach (var goal in goals)
            {
                dic.Add(goal, goal.AchievedPriority);
            }
            return dic;
        }
        public void RefreshGoal()
        {
            SetGoalPriority();
            _isStarted = true;
            Debug.Log("[GoapExecutor] プランニングを開始します。");
        }
        public void SetAction(IAgentController controller)
        {
            if (!_isStarted) return;

            // アクションが設定されていなかったらリターン
            if (_currentAction == null && _routeActions.Count == 0)
            {
                // ゴールが設定されていた場合そのまま再計画
                if (_currentGoal.Value.Key != null)
                {
                    Plan(_currentGoal.Value.Key);
                }
                return;
            }
            // アクションを取り出してアクションをセットする
            if (_currentAction == null)
            {
                _currentAction = _routeActions.Pop();
                _currentAction.SetTarget(controller);
            }
            // 実行中に前提条件が維持されているか確認
            if (!_currentAction.Execute(controller))
            {
                if (!_currentAction.Execute(controller))
                {
                    Debug.LogWarning($"[Executor] アクション {_currentAction.GetType().Name} の実行条件が破綻しました。");
                    CancelCurrentPlan();

                    // 現在のゴールで再プランニングを試みる
                    if (_currentGoal.Value.Key != null)
                    {
                        Plan(_currentGoal.Value.Key);
                    }
                    else
                    {
                        // ゴールがない場合は優先度を再評価
                        Debug.LogWarning("[Executor] 現在のゴールがnullです。ゴールを再設定します。");
                        SetGoalPriority();
                    }
                    return;
                }
            }
            // アクションの実行
            if (_currentAction.Perform(controller))
            {
                var effects = _currentAction.Effects;
                _currentAction = null;
                ApplyEffects(effects);
                CheckCurrentGoalStatus();
            }
        }
        /// <summary>
        /// ゴールの優先度を変更する
        /// </summary>
        /// <param name="config"></param>
        /// <param name="priority"></param>
        public void UpdateGoalPriority(SubGoalConfig config, int priority)
        {
            _goals[config] = priority; // 追加または更新
            SetGoalPriority();
        }
        /// <summary>
        /// 効果をワールドステートに反映する
        /// </summary>
        /// <param name="effects"></param>
        public void ApplyEffects(Dictionary<string, int> effects)
        {
            foreach (var effect in effects)
            {
                _worldStates.ModifyState(effect.Key, effect.Value);
            }
        }
        /// <summary>
        /// 計画する
        /// </summary>
        /// <param name="goalConfig"></param>
        private void Plan(SubGoalConfig goalConfig)
        {
            CancelCurrentPlan();

            // 現在のワールドステートを取得
            var currentWorldStates = _worldStates.CurrentStates;
            // ゴールの条件を取得
            var goal = goalConfig.GetGoalsConditions();
            // 計画する
            var plan = Planner.Planning(_usableActions, currentWorldStates, goal);

            if (plan != null)
            {
                _routeActions = plan;
                Debug.Log($"{goalConfig.name} へのプラン作成成功。ステップ数: {plan.Count}");
            }
            else
            {
                Debug.LogWarning($"{goalConfig.name} へのパスが見つかりませんでした。");
                UpdateGoalPriority(_currentGoal.Value.Key, _currentGoal.Value.Key.AchievedPriority);
            }
        }
        /// <summary>
        /// 現在のゴールが達成されているか確認して優先度を操作する
        /// </summary>
        private void CheckCurrentGoalStatus()
        {
            var config = _currentGoal.Value.Key;
            if (config == null) return;

            // Evalutionクラスなどの判定メソッドを使って現在の状態をチェック
            if (Evalution.IsSatisfied(_worldStates.CurrentStates, config.GetGoalsConditions()))
            {
                Debug.Log($"[Executor] アクションの結果、ゴール {config.name} を達成しました");
                UpdateGoalPriority(config, config.AchievedPriority);
            }
        }
        /// <summary>
        /// 今実行しているプランを破棄する
        /// </summary>
        private void CancelCurrentPlan()
        {
            _currentAction = null;
            _routeActions.Clear();
        }
        /// <summary>
        /// 優先度が最も高いゴールを設定する
        /// </summary>
        private void SetGoalPriority()
        {
            var bestGoal = _goals.OrderByDescending(x => x.Value).First();
            //　今現在一番優先度が高いゴールではなかったら設定する
            if (_currentGoal.Value.Value != bestGoal.Value)
            {
                Debug.Log($"最も高いゴールを設定{bestGoal.Key}{bestGoal.Value}");
                _currentGoal.Value = bestGoal;
            }
        }
    }
}