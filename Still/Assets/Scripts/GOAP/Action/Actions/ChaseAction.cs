using Still.Enum.WorldStates;
using Still.GOAP.Agent;
using Still.GOAP.Planner;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class ChaseAction : ActionBase
    {
        public override bool Execute(IAgentController agent)
        {
            if (Evalution.IsSatisfied(_worldStates.CurrentStates, Preconditions))
            {
                return true;
            }
            agent.SetSpeed(agent.Config.GhostMoveSpeed);
            return false;
        }

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
            agent.SetSpeed(agent.Config.GhostDashSpeed);
            //agent.SetMoveDestination();
        }
    }
}
