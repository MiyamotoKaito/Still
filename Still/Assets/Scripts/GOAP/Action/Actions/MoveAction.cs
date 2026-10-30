using System;

namespace Still.Goap
{
    ///<summary>
    ///現在のターゲットまで移動する
    ///</summary>
    [Serializable]
    public class MoveAction : ActionBase
    {
        ///<summary>
        ///ターゲットが無くなったら破綻とみなす
        ///</summary>
        public override bool Execute(IAgentController agent)
        {
            return agent.CurrentTarget != null && base.Execute(agent);
        }

        ///<summary>
        ///ターゲットに着いたら完了
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            return agent.IsArrived;
        }

        ///<summary>
        ///ターゲットへの移動を開始する
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            if (agent.CurrentTarget == null) return;
            agent.SetMoveDestination(agent.CurrentTarget.position);
        }
    }
}
