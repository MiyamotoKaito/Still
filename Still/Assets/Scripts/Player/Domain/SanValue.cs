using System;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///SAN値（0〜最大値）
    ///</summary>
    public readonly struct SanValue : IEquatable<SanValue>
    {
        public SanValue(float value, float max)
        {
            if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max));

            Max = max;
            Value = Mathf.Clamp(value, 0f, max);
        }

        ///<summary>現在値</summary>
        public float Value { get; }
        ///<summary>最大値</summary>
        public float Max { get; }
        ///<summary>ゲームオーバーになるか（表示が0%になった時点）</summary>
        public bool IsDepleted => Value < 1f;

        ///<summary>
        ///ゴーストの行動を変える恐怖レベル（0〜4）を求める
        ///</summary>
        public int ToFearLevel()
        {
            if (Value > 80f) return 0;
            if (Value > 60f) return 1;
            if (Value > 40f) return 2;
            if (Value > 20f) return 3;
            return 4;
        }

        ///<summary>
        ///加算した値を返す（0〜最大値に収める）
        ///</summary>
        public SanValue Add(float delta) => new SanValue(Value + delta, Max);

        ///<summary>
        ///現在値と最大値がともに等しいか
        ///</summary>
        public bool Equals(SanValue other) => Value.Equals(other.Value) && Max.Equals(other.Max);

        public override bool Equals(object obj) => obj is SanValue other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Value, Max);

        public static bool operator ==(SanValue left, SanValue right) => left.Equals(right);
        public static bool operator !=(SanValue left, SanValue right) => !left.Equals(right);
    }
}
