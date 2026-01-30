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
            _worldStates.ModifyState(WorldStateType.IsChasing.ToString(), 0);
            agent.SetSpeed(agent.Config.GhostMoveSpeed);
            return false;
        }

        public override bool Perform(IAgentController agent)
        {
            throw new System.NotImplementedException();
        }

        public override void SetTarget(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}アクション開始");
            agent.SetSpeed(agent.Config.GhostDashSpeed);
        }
    }
}
