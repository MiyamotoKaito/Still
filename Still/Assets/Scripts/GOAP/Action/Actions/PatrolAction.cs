using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class PatrolAction : ActionBase
    {
        public override bool Perform(IAgentController agent)
        {
            if (agent.IsArrived)
            {
                return true;
            }
            return false;
        }

        public override void SetTarget(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}アクション開始");
            Vector3 targetPos = agent.GetRandomPos();
            // NavMeshAgent に目的地をセット
            agent.SetMoveDestination(targetPos);
        }
    }
}