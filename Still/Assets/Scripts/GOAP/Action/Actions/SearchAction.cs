using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class SearchAction : ActionBase
    {
        public override bool Perform(IAgentController agent)
        {
            _duration -= Time.deltaTime;

            if (_duration <= 0)
            {
                return true;
            }
            return false;
        }

        public override void SetTarget(IAgentController agent)
        {
            if (agent.Config == null)
            {
                Debug.Log("コンフィグない");
            }
            Debug.Log($"{this.GetType().Name}アクション開始");
            _duration = agent.Config.Duration;
        }
        private float _duration;
    }
}