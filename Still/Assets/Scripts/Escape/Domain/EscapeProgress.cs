using System;

namespace Still.Escape
{
    ///<summary>
    ///脱出の進み具合（どれを調べたか）
    ///</summary>
    public readonly struct EscapeProgress : IEquatable<EscapeProgress>
    {
        public EscapeProgress(EscapeItemType achievedItems, int requiredCount)
        {
            if (requiredCount <= 0) throw new ArgumentOutOfRangeException(nameof(requiredCount));

            AchievedItems = achievedItems;
            RequiredCount = requiredCount;
        }

        ///<summary>調べ終わったもの</summary>
        public EscapeItemType AchievedItems { get; }
        ///<summary>鍵が出るのに必要な数</summary>
        public int RequiredCount { get; }
        ///<summary>調べ終わった数</summary>
        public int AchievedCount => CountBits((int)AchievedItems);
        ///<summary>鍵が出る条件を満たしたか</summary>
        public bool IsKeyUnlocked => AchievedCount >= RequiredCount;

        ///<summary>
        ///指定したものを調べ終わったか
        ///</summary>
        public bool IsAchieved(EscapeItemType item) => (AchievedItems & item) == item;

        ///<summary>
        ///調べ終わったものを加えた値を返す（同じものは二重に数えない）
        ///</summary>
        public EscapeProgress Achieve(EscapeItemType item) => new EscapeProgress(AchievedItems | item, RequiredCount);

        ///<summary>
        ///調べ終わったものと必要数がともに等しいか
        ///</summary>
        public bool Equals(EscapeProgress other) => AchievedItems == other.AchievedItems && RequiredCount == other.RequiredCount;

        public override bool Equals(object obj) => obj is EscapeProgress other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(AchievedItems, RequiredCount);

        public static bool operator ==(EscapeProgress left, EscapeProgress right) => left.Equals(right);
        public static bool operator !=(EscapeProgress left, EscapeProgress right) => !left.Equals(right);

        private static int CountBits(int value)
        {
            int count = 0;
            for (; value != 0; value &= value - 1) count++;
            return count;
        }
    }
}
