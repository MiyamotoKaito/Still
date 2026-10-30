using System.Collections.Generic;
using Still.Common;

namespace Still.Goap
{
    ///<summary>
    ///ワールドステートが条件を満たすかの判定
    ///</summary>
    public static class Evaluation
    {
        ///<summary>
        ///全ての条件を満たすか（未登録のキーは満たさない扱い）
        ///</summary>
        public static bool IsSatisfied(IReadOnlyDictionary<WorldStateType, int> states,
                                       IReadOnlyDictionary<WorldStateType, Condition> conditions)
        {
            foreach (var condition in conditions)
            {
                if (!states.TryGetValue(condition.Key, out int current)) return false;
                if (!condition.Value.IsSatisfiedBy(current)) return false;
            }
            return true;
        }
    }
}
