using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class CatchAction : ActionBase
    {
        public override bool Perform(IAgentController agent)
        {
            Debug.Log("プレイヤーをキャッチ");
            return true;
        }

        public override void SetTarget(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}アクション開始");
        }
    }
}