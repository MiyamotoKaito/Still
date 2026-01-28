using Still.GOAP.Action.Setting;
using System.Collections.Generic;
using UnityEngine;

namespace Still.GOAP.Goal.Config
{
    [CreateAssetMenu(fileName = "Goal", menuName = "Config/GOAP/Goal")]
    public class SubGoalConfig : ScriptableObject
    {
        public int Priority => _priority;
        public int AchievedPriority => _achievedPrioity;

        [SerializeField]
        [Tooltip("ゴールを設定するための条件")]
        private List<HasConditionActions> _settingGoalConditions = new();
        [SerializeField]
        [Tooltip("このゴールの達成に必要な条件")]
        private List<HasConditionActions> _targetWorldStates = new();
        [SerializeField]
        [Tooltip("ゴールが達成された後のワールドステートの値")]
        private List<NoConditionActions> _achievedtWorldStates = new();
        [Tooltip("このゴールの優先度")]
        [SerializeField]
        private int _priority;
        [SerializeField]
        [Tooltip("目標が達成された後の優先度")]
        private int _achievedPrioity;

        /// <summary>
        /// ゴールを設定するための条件を取得する
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Condition> GetGoalsWorldStateSettings()
        {
            var dict = new Dictionary<string, Condition>();
            foreach (var state in _settingGoalConditions)
            {
                dict[state.WorldStateKey] = state.Value; // 後勝ちで上書き
            }
            return dict;
        }
        /// <summary>
        /// ゴールを達成するための条件を取得する
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Condition> GetGoalsConditions()
        {
            var dict = new Dictionary<string, Condition>();
            foreach (var state in _targetWorldStates)
            {
                dict[state.WorldStateKey] = state.Value; // 後勝ちで上書き
            }
            return dict;
        }
        /// <summary>
        /// ゴールが達成された後の効果を反映する
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetGoalsEffect()
        {
            var dict = new Dictionary<string, int>();
            foreach (var state in _achievedtWorldStates)
            {
                dict[state.WorldStateKey] = state.Value; // 後勝ちで上書き
            }
            return dict;
        }
    }
}