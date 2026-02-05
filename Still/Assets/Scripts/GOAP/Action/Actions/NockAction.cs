using Still.GOAP.Action;
using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class NockAction : ActionBase
    {
        public override bool Perform(IAgentController agent)
        {
          return true;
        }

        public override void SetTarget(IAgentController agent)
        {
            
        }
    }
}