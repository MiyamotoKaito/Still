using System.Collections.Generic;
using System.Linq;
using Still.Common;
using UnityEngine;
using VContainer.Unity;

namespace Still.Goap
{
    ///<summary>
    ///最優先ゴールのプランを立て、アクションを毎フレーム実行する
    ///</summary>
    public sealed class GoapExecutor : IStartable, ITickable
    {
        public GoapExecutor(GhostActionData actionData, IWorldStates worldStates,
                            List<SubGoalConfig> goals, IAgentController agent)
        {
            _usableActions = actionData.GhostActions;
            _worldStates = worldStates;
            _agent = agent;
            foreach (var goal in goals) _goals[goal] = goal.AchievedPriority;
        }

        ///<summary>現在追っているゴール</summary>
        public SubGoalConfig CurrentGoal => _currentGoal;

        private SubGoalConfig _currentGoal;
        private int _currentGoalPriority;
        private IAction _currentAction;
        private Stack<IAction> _routeActions = new();
        private bool _isStarted;

        private readonly IReadOnlyList<IAction> _usableActions;
        private readonly IWorldStates _worldStates;
        private readonly IAgentController _agent;
        private readonly Dictionary<SubGoalConfig, int> _goals = new();

        ///<summary>
        ///最優先ゴールを決めてプランニングを始める
        ///</summary>
        public void Start()
        {
            _isStarted = true;
            SelectBestGoal(forcePlan: true);
        }

        ///<summary>
        ///アクションを1フレーム分進める
        ///</summary>
        public void Tick()
        {
            if (!_isStarted) return;

            if (_currentAction == null && _routeActions.Count == 0)
            {
                // 前のプランを使い切ったら、同じゴールで立て直す
                if (_currentGoal != null) Plan(_currentGoal);
                return;
            }

            // アクション内でワールドステートが変わると、その場でゴールが切り替わって計画が破棄されることがある。
            // 実行中のアクションを手元に持ち、差し替わっていたら以降の処理をしない
            var action = _currentAction;
            if (action == null)
            {
                action = _routeActions.Pop();
                _currentAction = action;
                action.SetTarget(_agent);
                if (_currentAction != action) return;
            }

            if (!action.Execute(_agent))
            {
                if (_currentAction != action) return;
                Debug.LogWarning($"[GoapExecutor] {action} の実行条件が崩れたので計画を破棄します");
                CancelCurrentPlan();
                UpdateGoalPriority(_currentGoal, _currentGoal.AchievedPriority);
                return;
            }

            if (!action.Perform(_agent) || _currentAction != action) return;

            _currentAction = null;
            foreach (var effect in action.Effects) _worldStates.ModifyState(effect.Key, effect.Value);
            CheckCurrentGoalStatus();
        }

        ///<summary>
        ///ゴールの現在の優先度を取得する
        ///</summary>
        public int GetPriority(SubGoalConfig goal) => _goals[goal];

        ///<summary>
        ///ゴールの優先度を変更し、最優先ゴールを選び直す
        ///</summary>
        public void UpdateGoalPriority(SubGoalConfig goal, int priority)
        {
            _goals[goal] = priority;
            SelectBestGoal(forcePlan: false);
        }

        private void Plan(SubGoalConfig goal)
        {
            CancelCurrentPlan();

            var plan = Planner.Planning(_usableActions, _worldStates.CurrentStates, goal.GetGoalsConditions());
            if (plan != null)
            {
                _routeActions = plan;
                Debug.Log($"[GoapExecutor] {goal.name} のプランを作成（{plan.Count}ステップ）");
                return;
            }

            Debug.LogWarning($"[GoapExecutor] {goal.name} へのプランが見つからないので優先度を下げます");
            UpdateGoalPriority(goal, goal.AchievedPriority);
        }

        private void CheckCurrentGoalStatus()
        {
            if (_currentGoal == null) return;
            if (!Evaluation.IsSatisfied(_worldStates.CurrentStates, _currentGoal.GetGoalsConditions())) return;

            Debug.Log($"[GoapExecutor] ゴール {_currentGoal.name} を達成");
            UpdateGoalPriority(_currentGoal, _currentGoal.AchievedPriority);
        }

        private void CancelCurrentPlan()
        {
            _currentAction = null;
            _routeActions.Clear();
        }

        private void SelectBestGoal(bool forcePlan)
        {
            if (_goals.Count == 0) return;

            var best = _goals.OrderByDescending(x => x.Value).First();
            // 同点なら今のゴールを続ける（以前は優先度の値だけを比べていたため、同点の別ゴールへ切り替わらなかった）
            var next = _currentGoal != null && _goals[_currentGoal] >= best.Value ? _currentGoal : best.Key;
            int nextPriority = _goals[next];

            bool changed = next != _currentGoal || nextPriority != _currentGoalPriority;
            _currentGoal = next;
            _currentGoalPriority = nextPriority;

            if (!_isStarted || (!changed && !forcePlan)) return;

            Debug.Log($"[GoapExecutor] 最優先ゴール: {next.name}（優先度{nextPriority}）");
            Plan(next);
        }
    }
}
