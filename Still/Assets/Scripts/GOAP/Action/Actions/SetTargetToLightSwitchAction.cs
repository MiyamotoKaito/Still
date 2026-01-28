using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class SetTargetToLightSwitchAction : ActionBase
    {
        private LightSwitch _lightSwtich;
        public override bool Perform(IAgentController agent)
        {
            if (_lightSwtich == null) return true;

            if (agent.IsArrived)
            {
                Debug.Log(($"[Action] {nameof(SetTargetToLightSwitchAction)}: 目的地に到着しました。"));
                return true;
            }
            return false;
        }

        public override void SetTarget(IAgentController agent)
        {
            _lightSwtich = GameObject.FindAnyObjectByType<LightSwitch>();
            agent.SetMoveDestination(_lightSwtich.transform.position);
        }
    }
}