using Still.GOAP.Agent;
using System.Collections.Generic;
namespace Still.GOAP.Action
{
    public interface IAction
    {
        Dictionary<string, Condition> Preconditions { get; }
        Dictionary<string, int> Effects { get; }
        int ActionCost { get; }

        /// <summary>
        /// アクション開始時に一度だけ呼ばれる（ターゲットの設定など）
        /// </summary>
        /// <param name="agent"></param>
        void SetTarget(IAgentController agent);

        /// <summary>
        /// 毎フレーム呼ばれる。完了したら true を返す
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        bool Perform(IAgentController agent);

        /// <summary>
        /// 実行中に前提条件が維持されているか確認
        /// </summary>
        /// <param name="agent"></param>
        /// <returns></returns>
        bool Execute(IAgentController agent);
    }
}