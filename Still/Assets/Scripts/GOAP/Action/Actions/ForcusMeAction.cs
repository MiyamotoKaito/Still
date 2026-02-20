using Still.GOAP.Agent;
using Still.Player;
using UnityEngine;
namespace Still.GOAP.Action.Actions
{
    [System.Serializable]
    public class FocusMeAction : ActionBase
    {
        [SerializeReference, SubclassSelector]
        private IEvent _focusMeEvent;
        private GameInstaller _gameInstaller;
        public override bool Perform(IAgentController agent)
        {
            return _focusMeEvent.IsFinished;
        }

        public override void SetTarget(IAgentController agent)
        {
            _gameInstaller = GameObject.FindAnyObjectByType<GameInstaller>();
            agent.StopMove();
            _gameInstaller.SANValueModel.ModifySAN(-20);
            AudioManager.Instance.PlaySE("Laugh", agent.AudioSource);
            _focusMeEvent.Initialize();
            _focusMeEvent.OnEvent();
        }
    }
}