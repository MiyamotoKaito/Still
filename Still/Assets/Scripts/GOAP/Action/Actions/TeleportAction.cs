using Still.GOAP.Action;
using Still.GOAP.Agent;
using UnityEngine;
[System.Serializable]
public class TeleportAction : ActionBase
{
    public override bool Perform(IAgentController agent)
    {
        if (_farthestPos == null) return false;

        return true;
    }

    public override void SetTarget(IAgentController agent)
    {
        var pos = GameObject.FindObjectsByType<TeleportPos>(FindObjectsSortMode.None);
        float dis = 500;
        foreach (var p in pos)
        {
            var d = Vector3.Distance(agent.Position, p.transform.position);
            
            if (d < dis)
            {
                _farthestPos = p.transform.position;
                dis = d;
            }
        }
    }
    private Vector3 _farthestPos;
}