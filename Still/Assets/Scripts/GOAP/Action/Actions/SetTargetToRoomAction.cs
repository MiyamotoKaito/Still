using Still.GOAP.Agent;
using Still.Object;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class SetTargetToRoom : ActionBase
    {
        public override bool Perform(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}アクション開始");
            return true;
        }

        public override void SetTarget(IAgentController agent)
        {
            var room = GameObject.FindObjectsByType<Room>(sortMode: FindObjectsSortMode.None);
            foreach (var r in room)
            {
                if (r.InTheRoom)
                {
                    agent.SetTarget(r.Door);
                    return;
                }
            }
        }
    }
}
