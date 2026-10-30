using System.Collections.Generic;
using Still.Common;
using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///ゴーストのゴール
    ///</summary>
    [CreateAssetMenu(fileName = "Goal", menuName = "Config/GOAP/Goal")]
    public class SubGoalConfig : ScriptableObject
    {
        ///<summary>発動条件を満たしたときの優先度</summary>
        public int Priority => _priority;
        ///<summary>達成後（平常時）の優先度</summary>
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
        [SerializeField]
        [Tooltip("このゴールの優先度")]
        private int _priority;
        [SerializeField]
        [Tooltip("目標が達成された後の優先度")]
        private int _achievedPrioity;

        ///<summary>
        ///ゴールを発動するための条件を取得する
        ///</summary>
        public Dictionary<WorldStateType, Condition> GetGoalsWorldStateSettings() => ActionSettingConverter.ToDictionary(_settingGoalConditions);

        ///<summary>
        ///ゴールを達成するための条件を取得する
        ///</summary>
        public Dictionary<WorldStateType, Condition> GetGoalsConditions() => ActionSettingConverter.ToDictionary(_targetWorldStates);

        ///<summary>
        ///ゴール達成後に書き換えるワールドステートを取得する
        ///</summary>
        public Dictionary<WorldStateType, int> GetGoalsEffect() => ActionSettingConverter.ToDictionary(_achievedtWorldStates);
    }
}
