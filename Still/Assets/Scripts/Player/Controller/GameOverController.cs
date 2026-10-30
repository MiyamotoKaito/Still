using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Still.Common;
using VContainer.Unity;

namespace Still.Player
{
    ///<summary>
    ///SAN値が尽きたら暗転してタイトルへ戻す
    ///</summary>
    public sealed class GameOverController : IStartable, IDisposable
    {
        private const float FadeSeconds = 2f;
        private const int WaitAfterFadeMilliseconds = 500;

        public GameOverController(PlayerStatusEntity status, IPlayerAvatar avatar, IScreenFader fader, IStateMachine stateMachine)
        {
            _status = status;
            _avatar = avatar;
            _fader = fader;
            _stateMachine = stateMachine;
        }

        private IDisposable _subscription;

        private readonly PlayerStatusEntity _status;
        private readonly IPlayerAvatar _avatar;
        private readonly IScreenFader _fader;
        private readonly IStateMachine _stateMachine;
        private readonly CancellationTokenSource _cts = new();

        ///<summary>
        ///SAN値の監視を始める
        ///</summary>
        public void Start()
        {
            _subscription = _status.San
                .Where(san => san.IsDepleted)
                .Take(1)
                .Subscribe(_ => GameOverAsync(_cts.Token).Forget());
        }

        ///<summary>
        ///監視をやめる
        ///</summary>
        public void Dispose()
        {
            _subscription?.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }

        private async UniTaskVoid GameOverAsync(CancellationToken ct)
        {
            _avatar.LockInput();
            await _fader.FadeOutAsync(FadeSeconds, ct);
            await UniTask.Delay(WaitAfterFadeMilliseconds, cancellationToken: ct);
            await _stateMachine.ChangeStateAsync(GameModeType.Title);
        }
    }
}
