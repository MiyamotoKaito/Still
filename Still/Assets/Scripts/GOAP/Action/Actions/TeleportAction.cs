using Still.GOAP.Action;
using Still.GOAP.Agent;
using UnityEngine;

public class TeleportAction : ActionBase
{
    public override bool Perform(IAgentController agent)
    {
        throw new System.NotImplementedException();
    }

    public override void SetTarget(IAgentController agent)
    {
        
    }
    private Vector3 _teleportPosition;
}
