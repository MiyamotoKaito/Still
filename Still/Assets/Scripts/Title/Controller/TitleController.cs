using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;
using Still.Utility;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Still.Title
{
    ///<summary>
    ///クリックでゲームを始める
    ///</summary>
    public sealed class TitleController : IStartable, IDisposable
    {
        private const float BgmFadeSeconds = 0.5f;

        public TitleController(TitleView view, SceneComponentList<OpendDoor> doors, IAudioService audio, IStateMachine stateMachine)
        {
            _view = view;
            _doors = doors.Items;
            _audio = audio;
            _stateMachine = stateMachine;
        }

        private bool _isStarting;

        private readonly TitleView _view;
        private readonly IReadOnlyList<OpendDoor> _doors;
        private readonly IAudioService _audio;
        private readonly IStateMachine _stateMachine;
        private readonly PlayerInputActions _inputActions = new();
        private readonly CancellationTokenSource _cts = new();

        ///<summary>
        ///タイトルを表示して入力を待つ
        ///</summary>
        public void Start()
        {
            _view.ShowTitle();
            _audio.PlayBgm("Title");
            foreach (var door in _doors) door.OnOpened += HandleDoorOpened;
            _inputActions.Title.Click.performed += HandleClick;
            _inputActions.Title.Enable();
        }

        ///<summary>
        ///入力の購読をやめる
        ///</summary>
        public void Dispose()
        {
            foreach (var door in _doors)
            {
                if (door != null) door.OnOpened -= HandleDoorOpened;
            }
            _inputActions.Title.Click.performed -= HandleClick;
            _inputActions.Dispose();
            _cts.Cancel();
            _cts.Dispose();
        }

        private void HandleClick(InputAction.CallbackContext context)
        {
            // 演出中に連打されても遷移は一度だけにする
            if (_isStarting) return;
            _isStarting = true;
            StartGameAsync(_cts.Token).Forget();
        }

        private void HandleDoorOpened()
        {
            _audio.PlaySe("DoorOpen");
        }

        private async UniTaskVoid StartGameAsync(CancellationToken ct)
        {
            _audio.FadeOutBgm(BgmFadeSeconds);
            _view.PlayStartTransition();
            await UniTask.Delay(_view.StartDelayMilliseconds, cancellationToken: ct);
            await _stateMachine.ChangeStateAsync(GameModeType.InGame);
        }
    }
}
