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
            if (_lightSwtich == null) return false;

            Debug.Log(($"[Action] {nameof(SetTargetToLightSwitchAction)}: LightSwitchをターゲットに決定"));
            return true;
        }

        public override void SetTarget(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}アクション開始");
            _lightSwtich = GameObject.FindAnyObjectByType<LightSwitch>();
            agent.SetTarget(_lightSwtich.transform.position);
        }
    }
}