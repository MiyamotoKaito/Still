namespace Still.Common
{
    ///<summary>
    ///プレイヤーのSAN値の操作
    ///</summary>
    public interface ISanUsecase
    {
        ///<summary>SAN値が尽きてゲームオーバーになるか</summary>
        bool IsDepleted { get; }

        ///<summary>
        ///SAN値を減らす
        ///</summary>
        void Damage(float amount);
    }
}
