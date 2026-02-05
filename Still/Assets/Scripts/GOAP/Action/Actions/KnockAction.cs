using Still.GOAP.Action;
using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class KnockAction : ActionBase
    {
        public override bool Perform(IAgentController agent)
        {
            AudioManager.Instance.PlaySE("Knock");
            return true;
        }

        public override void SetTarget(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}のターゲットを設定");
        }
    }
}