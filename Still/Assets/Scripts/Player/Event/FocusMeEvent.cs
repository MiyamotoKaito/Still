using System;
using Cysharp.Threading.Tasks;
using Still.Common;
using UnityEngine;
using VContainer;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーの視線をゴーストに釘付けにしてズームする
    ///</summary>
    [Serializable]
    public class FocusMeEvent : IEvent
    {
        ///<summary>演出が終わったか</summary>
        public bool IsFinished => _isFinished;

        [SerializeField] private float _focusDuration;
        [SerializeField] private float _focusFOV;

        private FieldOfViewEntity _fov;
        private IPlayerAvatar _player;
        private CameraView _cameraView;
        private bool _isFinished;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void Construct(FieldOfViewEntity fov, IPlayerAvatar player, CameraView cameraView)
        {
            _fov = fov;
            _player = player;
            _cameraView = cameraView;
        }

        ///<summary>
        ///演出を開始する
        ///</summary>
        public void Play()
        {
            // 以前は2回目以降も完了フラグが立ったままで、演出が即終了扱いになっていた
            _isFinished = false;
            PlayAsync().Forget();
        }

        private async UniTaskVoid PlayAsync()
        {
            var ct = _cameraView.destroyCancellationToken;
            _cameraView.SetLookInputEnabled(false);
            _player.LockInput();

            float initialFov = _fov.Fov.CurrentValue;
            float elapsed = 0f;
            while (elapsed < _focusDuration)
            {
                elapsed += Time.deltaTime;
                _fov.SetFov(Mathf.Lerp(initialFov, _focusFOV, elapsed / _focusDuration));
                await UniTask.Yield(ct);
            }
            _fov.SetFov(_focusFOV);

            await UniTask.Delay(TimeSpan.FromSeconds(_focusDuration), cancellationToken: ct);

            _fov.SetFov(initialFov);
            _player.UnlockInput();
            _cameraView.SetLookInputEnabled(true);
            _isFinished = true;
        }
    }
}
