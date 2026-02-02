using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class SetTargetToLightSwitchAction : ActionBase
    {
        public override bool Perform(IAgentController agent)
        {
            if(agent.CurrentTarget == null) return false;
            Debug.Log(($"[Action] {nameof(SetTargetToLightSwitchAction)}: LightSwitchをターゲットに決定"));
            return true;
        }

        public override void SetTarget(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}アクション開始");
            var lights = GameObject.FindObjectsByType<LightSwitch>(FindObjectsSortMode.None);
            foreach (var light in lights)
            {
                if (light.IsOn)
                {
                    agent.SetTarget(light.gameObject);
                    break;
                }
            }
        }
    }
}