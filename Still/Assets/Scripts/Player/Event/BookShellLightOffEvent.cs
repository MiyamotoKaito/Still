using System;
using Cysharp.Threading.Tasks;
using Still.Common;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Still.Player
{
    ///<summary>
    ///本棚の照明が消える演出をカメラを切り替えて見せる
    ///</summary>
    [Serializable]
    public class BookShellLightOffEvent : IEvent
    {
        private const int CameraBlendMilliseconds = 700;
        private const int AnimationMilliseconds = 2000;
        private const int ActivePriority = 10;

        ///<summary>演出が終わったか</summary>
        public bool IsFinished => _isFinished;

        [SerializeField] private CinemachineCamera _playerCamera;
        [SerializeField] private CinemachineCamera _bookCam;
        [SerializeField] private Animator _bookShellAnimator;

        private IPlayerAvatar _player;
        private bool _isFinished;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void Construct(IPlayerAvatar player)
        {
            _player = player;
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
            var ct = _bookCam.destroyCancellationToken;
            _bookCam.transform.position = _playerCamera.transform.position;
            _bookCam.LookAt = _bookShellAnimator.transform;
            _player.LockInput();
            _bookCam.Priority = ActivePriority;

            await UniTask.Delay(CameraBlendMilliseconds, cancellationToken: ct);
            _bookShellAnimator.SetTrigger("LightOff");
            await UniTask.Delay(AnimationMilliseconds, cancellationToken: ct);

            _player.UnlockInput();
            _bookCam.Priority = 0;
            _isFinished = true;
        }
    }
}
