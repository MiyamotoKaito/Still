using System;
using Still.Common;
using Template.Editor;
using UnityEngine;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///ゴーストを見たプレイヤーの視線を釘付けにする
    ///</summary>
    [Serializable]
    public class FocusMeAction : ActionBase
    {
        private const float SanDamage = 20f;

        [SerializeReference, SubclassSelector]
        private IEvent _focusMeEvent;

        private IAudioService _audio;
        private ISanUsecase _san;

        ///<summary>
        ///依存を受け取り、演出にも注入する
        ///</summary>
        [Inject]
        public void InjectDependencies(IAudioService audio, ISanUsecase san, IObjectResolver resolver)
        {
            _audio = audio;
            _san = san;
            if (_focusMeEvent != null) resolver.Inject(_focusMeEvent);
        }

        ///<summary>
        ///演出が終わったら完了
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            return _focusMeEvent == null || _focusMeEvent.IsFinished;
        }

        ///<summary>
        ///その場で止まって笑い、演出を始める
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
            agent.StopMove();
            _san.Damage(SanDamage);
            _audio.PlaySe("Laugh", agent.AudioSource);
            _focusMeEvent?.Play();
        }
    }
}
