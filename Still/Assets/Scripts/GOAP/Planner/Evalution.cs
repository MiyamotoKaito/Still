using Still.Enum.CompareType;
using System.Collections.Generic;
using UnityEngine;
namespace Still.GOAP.Planner
{
    public static class Evalution
    {
        public static bool IsSatisfied(Dictionary<string, int> currentWorldStates,
                                       Dictionary<string, Condition> conditions)
        {
            foreach (var conditionEntry in conditions)
            {
                string key = conditionEntry.Key;
                Condition condition = conditionEntry.Value;

                // 現在のWorldStateにキーが存在しない場合、条件は満たされていないと判断
                if (!currentWorldStates.TryGetValue(key, out int currentValue))
                {
                    Debug.Log($"ワールドステートがない : {key}");
                    return false;
                }

                bool isMet = false;
                switch (condition.Comparison)
                {
                    case CompareType.Equal:
                        isMet = (currentValue == condition.Value);
                        break;
                    case CompareType.NotEqual:
                        isMet = (currentValue != condition.Value);
                        break;
                    case CompareType.Greater:
                        isMet = (currentValue > condition.Value);
                        break;
                    case CompareType.Less:
                        isMet = (currentValue < condition.Value);
                        break;
                    case CompareType.GreaterEqual:
                        isMet = (currentValue >= condition.Value);
                        break;
                    case CompareType.LessEqual:
                        isMet = (currentValue <= condition.Value);
                        break;
                }

                // 一つでも条件を満たさなければ、即座に false を返す
                if (!isMet)
                {
                    return false;
                }
            }

            // 全ての条件を満たした場合に true を返す
            return true;
        }
    }
}
