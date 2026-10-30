using System.Collections.Generic;
using R3;

namespace Still.Common
{
    ///<summary>
    ///GOAPのワールドステート。ゴーストの判断材料を各モジュールが書き込む
    ///</summary>
    public interface IWorldStates
    {
        ///<summary>値が変わったキーを通知する</summary>
        Observable<WorldStateType> OnStateChanged { get; }
        ///<summary>現在の全ステート</summary>
        IReadOnlyDictionary<WorldStateType, int> CurrentStates { get; }

        ///<summary>
        ///値を取得する（未登録のキーは0）
        ///</summary>
        int GetStateValue(WorldStateType key);

        ///<summary>
        ///値を上書きする
        ///</summary>
        void ModifyState(WorldStateType key, int value);

        ///<summary>
        ///値に加算する
        ///</summary>
        void AddState(WorldStateType key, int value);
    }
}
