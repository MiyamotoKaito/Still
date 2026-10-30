namespace Still.Escape
{
    ///<summary>
    ///チェックリストの1行の書式
    ///</summary>
    public static class ChecklistFormat
    {
        ///<summary>
        ///達成済みなら■、未達成なら□を付ける
        ///</summary>
        public static string Line(bool isDone, string text) => $"{(isDone ? "■" : "□")}:{text}";
    }
}
