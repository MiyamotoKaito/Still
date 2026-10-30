using System;

namespace Still.Common
{
    ///<summary>
    ///ワールドステートの値に対する条件
    ///</summary>
    // インスペクタで編集するSOのデータなので、Unityのシリアライズに合わせて公開フィールドにしている
    [Serializable]
    public struct Condition
    {
        public Condition(int value, CompareType comparison)
        {
            Value = value;
            Comparison = comparison;
        }

        public int Value;
        public CompareType Comparison;

        ///<summary>
        ///値がこの条件を満たすか
        ///</summary>
        public bool IsSatisfiedBy(int current)
        {
            switch (Comparison)
            {
                case CompareType.Equal: return current == Value;
                case CompareType.NotEqual: return current != Value;
                case CompareType.Greater: return current > Value;
                case CompareType.Less: return current < Value;
                case CompareType.GreaterEqual: return current >= Value;
                case CompareType.LessEqual: return current <= Value;
                default: return false;
            }
        }
    }
}
