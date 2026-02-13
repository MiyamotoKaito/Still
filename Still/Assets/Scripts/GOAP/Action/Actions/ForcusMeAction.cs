using Still.Enum.WorldStates;
using Still.GOAP.Agent;
using UnityEngine;
namespace Still.GOAP.Action.Actions
{
    [System.Serializable]
    public class FocusMeAction : ActionBase
    {
        [SerializeReference, SubclassSelector]
        private IEvent _focusMeEvent;
        public override bool Perform(IAgentController agent)
        {
            return _focusMeEvent.IsFinished;
        }

        public override void SetTarget(IAgentController agent)
        {
            _focusMeEvent.Initialize();
            _focusMeEvent.OnEvent();
        }
    }
}