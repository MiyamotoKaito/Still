using System;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///スタミナ（0〜最大値）
    ///</summary>
    public readonly struct StaminaValue : IEquatable<StaminaValue>
    {
        public StaminaValue(float value, float max)
        {
            if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max));

            Max = max;
            Value = Mathf.Clamp(value, 0f, max);
        }

        ///<summary>現在値</summary>
        public float Value { get; }
        ///<summary>最大値</summary>
        public float Max { get; }
        ///<summary>最大値に対する割合（0〜1）</summary>
        public float Ratio => Value / Max;
        ///<summary>使い切ったか</summary>
        public bool IsEmpty => Value <= 0f;
        ///<summary>満タンか</summary>
        public bool IsFull => Value >= Max;

        ///<summary>
        ///加算した値を返す（0〜最大値に収める）
        ///</summary>
        public StaminaValue Add(float delta) => new StaminaValue(Value + delta, Max);

        ///<summary>
        ///現在値と最大値がともに等しいか
        ///</summary>
        public bool Equals(StaminaValue other) => Value.Equals(other.Value) && Max.Equals(other.Max);

        public override bool Equals(object obj) => obj is StaminaValue other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Value, Max);

        public static bool operator ==(StaminaValue left, StaminaValue right) => left.Equals(right);
        public static bool operator !=(StaminaValue left, StaminaValue right) => !left.Equals(right);
    }
}
