using System;
using UnityEngine;

namespace Still.Goap
{
    ///<summary>
    ///周囲のランダムな地点へ歩く
    ///</summary>
    [Serializable]
    public class PatrolAction : ActionBase
    {
        private Vector3 _destination;

        ///<summary>
        ///目的地に着いたら完了
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            return Vector3.Distance(agent.Position, _destination) < agent.Config.GhostStopDistance;
        }

        ///<summary>
        ///ランダムな目的地を決めて歩き出す
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            _destination = agent.GetRandomPos();
            agent.SetSpeed(agent.Config.GhostMoveSpeed);
            agent.SetMoveDestination(_destination);
        }
    }
}
