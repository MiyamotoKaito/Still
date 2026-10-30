namespace Still.Escape
{
    ///<summary>
    ///調べたことに対する目標の反応の種類
    ///</summary>
    public enum InteractOutcomeType
    {
        Ignored,
        Progressed,
        Mistake,
    }

    ///<summary>
    ///調べたことに対する目標の反応（間違えたときのペナルティを含む）
    ///</summary>
    public readonly struct InteractOutcome
    {
        public InteractOutcome(InteractOutcomeType outcomeType, float sanDamage, string mistakeSe)
        {
            OutcomeType = outcomeType;
            SanDamage = sanDamage;
            MistakeSe = mistakeSe;
        }

        ///<summary>この目標とは関係なかった</summary>
        public static InteractOutcome Ignored => new InteractOutcome(InteractOutcomeType.Ignored, 0f, null);
        ///<summary>目標が進んだ</summary>
        public static InteractOutcome Progressed => new InteractOutcome(InteractOutcomeType.Progressed, 0f, null);

        ///<summary>反応の種類</summary>
        public InteractOutcomeType OutcomeType { get; }
        ///<summary>間違えたときに減るSAN値</summary>
        public float SanDamage { get; }
        ///<summary>間違えたときに鳴らすSE（無ければnull）</summary>
        public string MistakeSe { get; }

        ///<summary>
        ///間違えたときの反応を作る
        ///</summary>
        public static InteractOutcome Mistake(float sanDamage, string mistakeSe) => new InteractOutcome(InteractOutcomeType.Mistake, sanDamage, mistakeSe);
    }
}
