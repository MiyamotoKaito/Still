using Still.Common;

namespace Still.Escape
{
    ///<summary>
    ///ステージの開始時・達成時に起きること（MissionConfigにSerializeReferenceで並べる）
    ///</summary>
    public interface IMissionEffect
    {
        ///<summary>
        ///効果を実行する
        ///</summary>
        void Apply(IMissionEffectContext context);
    }

    ///<summary>
    ///効果が操作できるもの
    ///</summary>
    public interface IMissionEffectContext
    {
        ///<summary>
        ///鍵の掛かったドアを開けられるようにする
        ///</summary>
        void UnlockDoor(TargetId door);

        ///<summary>
        ///隠れていた調べる物を出現させる
        ///</summary>
        void RevealTarget(TargetId target);

        ///<summary>
        ///ワールドステートを書き換える（ゴーストへの合図）
        ///</summary>
        void SetWorldState(WorldStateType key, int value);

        ///<summary>
        ///SEを鳴らす
        ///</summary>
        void PlaySe(string soundName);
    }
}
