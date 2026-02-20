using Still.GOAP.Agent;
using Unity.Cinemachine;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class PatrolAction : ActionBase
    {
        private Vector3 _targetPos;
        public override bool Perform(IAgentController agent)
        {
            if (Vector3.Distance(agent.Position, _targetPos) < agent.Config.GhostStopDistance)
            {
                Debug.Log($"{this.GetType().Name}アクション完了");
                return true;
            }
            return false;
        }

        public override void SetTarget(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}アクション開始");
            _targetPos = agent.GetRandomPos();
            agent.SetSpeed(agent.Config.GhostMoveSpeed);
            // NavMeshAgent に目的地をセット
            agent.SetMoveDestination(_targetPos);
        }
    }
}