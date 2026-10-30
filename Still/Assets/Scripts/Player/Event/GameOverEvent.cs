using System;
using Cysharp.Threading.Tasks;
using Still.Common;
using VContainer;

namespace Still.Player
{
    ///<summary>
    ///ゴーストに捕まったとき、SAN値を削って最後のチェックポイントへ戻す
    ///</summary>
    [Serializable]
    public class GameOverEvent : IEvent
    {
        private const float SanDamage = 10f;
        private const float FadeSeconds = 2f;

        ///<summary>演出が終わったか</summary>
        public bool IsFinished => _isFinished;

        private ISanUsecase _san;
        private ICheckpoint _checkpoint;
        private IPlayerAvatar _player;
        private IScreenFader _fader;
        private bool _isFinished;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void Construct(ISanUsecase san, ICheckpoint checkpoint, IPlayerAvatar player, IScreenFader fader)
        {
            _san = san;
            _checkpoint = checkpoint;
            _player = player;
            _fader = fader;
        }

        ///<summary>
        ///演出を開始する
        ///</summary>
        public void Play()
        {
            _isFinished = false;
            PlayAsync().Forget();
        }

        private async UniTaskVoid PlayAsync()
        {
            _san.Damage(SanDamage);
            // SAN値が尽きた場合はGameOverControllerがタイトルへ戻すので、ここでは何もしない
            if (_san.IsDepleted)
            {
                _isFinished = true;
                return;
            }

            _player.LockInput();
            _fader.SetBlack();
            // 暗転中に移動させて、ワープを見せない
            _player.Warp(_checkpoint.Load());

            // 以前はフェードの目標値が1（不透明）のままで、暗転が解除されずに急に消えていた
            await _fader.FadeInAsync(FadeSeconds, default);

            _player.UnlockInput();
            _isFinished = true;
        }
    }
}
