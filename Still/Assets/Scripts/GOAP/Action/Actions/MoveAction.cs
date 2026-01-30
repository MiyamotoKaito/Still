using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class MoveAction : ActionBase
    {
        public override bool Perform(IAgentController agent)
        {
            if (agent.IsArrived)
            {
                Debug.Log("目的地にたどり着いた");
                return true;
            }
            return false;
        }

        public override void SetTarget(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}アクション開始");
            _targetPos = agent.TargetPos;
            agent.SetMoveDestination(_targetPos);
        }
        private Vector3 _targetPos;
    }
}