using System;

namespace Still.Escape
{
    ///<summary>
    ///ミッションで指定する、シーン上の調べられる物やドアのID
    ///</summary>
    public readonly struct TargetId : IEquatable<TargetId>
    {
        public TargetId(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("IDが空です", nameof(value));

            Value = value;
        }

        ///<summary>ID文字列</summary>
        public string Value { get; }

        ///<summary>
        ///IDが等しいか
        ///</summary>
        public bool Equals(TargetId other) => Value == other.Value;

        public override bool Equals(object obj) => obj is TargetId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;

        public static bool operator ==(TargetId left, TargetId right) => left.Equals(right);
        public static bool operator !=(TargetId left, TargetId right) => !left.Equals(right);
    }
}
