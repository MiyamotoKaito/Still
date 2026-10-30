using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;

namespace Still.Escape
{
    ///<summary>
    ///脱出のルール（調べる→鍵が出る→鍵で扉を開けて脱出）
    ///</summary>
    public sealed class EscapeUsecase
    {
        private const float FadeSeconds = 2f;
        private const int WaitAfterFadeMilliseconds = 2000;

        public EscapeUsecase(EscapeEntity entity, IPlayerAvatar player, IScreenFader fader, IStateMachine stateMachine)
        {
            _entity = entity;
            _player = player;
            _fader = fader;
            _stateMachine = stateMachine;
        }

        ///<summary>出口の扉を開けられるか</summary>
        public bool CanOpenExit => _entity.HasKey.CurrentValue;

        private readonly EscapeEntity _entity;
        private readonly IPlayerAvatar _player;
        private readonly IScreenFader _fader;
        private readonly IStateMachine _stateMachine;

        ///<summary>
        ///調べたものを記録する（同じものを何度調べても1回と数える）
        ///</summary>
        public void Achieve(EscapeItemType item)
        {
            _entity.SetProgress(_entity.Progress.CurrentValue.Achieve(item));
        }

        ///<summary>
        ///鍵を拾う
        ///</summary>
        public void CollectKey()
        {
            _entity.SetHasKey(true);
        }

        ///<summary>
        ///暗転してタイトルへ戻る
        ///</summary>
        public async UniTask EscapeAsync(CancellationToken ct)
        {
            _player.LockInput();
            await _fader.FadeOutAsync(FadeSeconds, ct);
            await UniTask.Delay(WaitAfterFadeMilliseconds, cancellationToken: ct);
            await _stateMachine.ChangeStateAsync(GameModeType.Title);
        }
    }
}
