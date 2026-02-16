using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action
{
    [System.Serializable]
    public class CatchAction : ActionBase
    {
        [SerializeReference, SubclassSelector]
        private IEvent _catchEvent;
        public override bool Perform(IAgentController agent)
        {
            Debug.Log("プレイヤーをキャッチ");
            _catchEvent.Initialize();
            _catchEvent.OnEvent();
            return true;
        }

        public override void SetTarget(IAgentController agent)
        {
            Debug.Log($"{this.GetType().Name}アクション開始");
        }
    }
}