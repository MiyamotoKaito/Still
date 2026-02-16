using Still.GOAP.Action;
using Still.GOAP.Agent;
using UnityEngine;
[System.Serializable]
public class TeleportAction : ActionBase
{
    public override bool Perform(IAgentController agent)
    {
        if (_farthestPos == null) return false;
        agent.Teleport(_farthestPos);
        return true;
    }

    public override void SetTarget(IAgentController agent)
    {
        var pos = GameObject.FindAnyObjectByType<TeleportPos>();
        float dis = 0;
        foreach (var p in pos.TeleportPosList)
        {
            float d = Mathf.Abs(agent.Position.x - p.transform.position.x)
                                                + Mathf.Abs(agent.Position.z - p.transform.position.z);

            if (d > dis)
            {
                _farthestPos = p.transform.position;
                dis = d;
            }
        }
    }
    private Vector3 _farthestPos;
}