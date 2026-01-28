using Still.GOAP.Agent;
using Still.GOAP.Planner;
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

        }
    }
}