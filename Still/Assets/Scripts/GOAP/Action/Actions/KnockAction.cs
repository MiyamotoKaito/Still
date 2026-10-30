using System;
using Still.Common;
using VContainer;

namespace Still.Goap
{
    ///<summary>
    ///プレイヤーがこもっている部屋のドアをノックする
    ///</summary>
    [Serializable]
    public class KnockAction : ActionBase
    {
        private const float SanDamage = 5f;

        private IAudioService _audio;
        private ISanUsecase _san;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void InjectDependencies(IAudioService audio, ISanUsecase san)
        {
            _audio = audio;
            _san = san;
        }

        ///<summary>
        ///ノックしてSAN値を削る
        ///</summary>
        public override bool Perform(IAgentController agent)
        {
            _audio.PlaySe("Knock");
            _san.Damage(SanDamage);
            return true;
        }

        ///<summary>
        ///開始時の処理はない
        ///</summary>
        public override void SetTarget(IAgentController agent)
        {
        }
    }
}
