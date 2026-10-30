using System.Collections.Generic;
using Still.Common;

namespace Still.Goap
{
    ///<summary>
    ///ゴーストのアクション
    ///</summary>
    public interface IAction
    {
        ///<summary>実行に必要な条件</summary>
        IReadOnlyDictionary<WorldStateType, Condition> Preconditions { get; }
        ///<summary>完了時にワールドステートへ書き込む値</summary>
        IReadOnlyDictionary<WorldStateType, int> Effects { get; }
        ///<summary>プランニング時のコスト</summary>
        int ActionCost { get; }

        ///<summary>
        ///アクション開始時に一度だけ呼ばれる（ターゲットの設定など）
        ///</summary>
        void SetTarget(IAgentController agent);

        ///<summary>
        ///毎フレーム呼ばれる。完了したら true を返す
        ///</summary>
        bool Perform(IAgentController agent);

        ///<summary>
        ///実行中に前提条件が維持されているか確認する
        ///</summary>
        bool Execute(IAgentController agent);
    }
}
