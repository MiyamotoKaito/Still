using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Still.Player
{
    ///<summary>
    ///プレイヤーを本棚へ振り向かせ、照明が消えるところを見せる
    ///</summary>
    [Serializable]
    public class BookShellLightOffEvent : IEvent
    {
        private const int BeforeLightOffMilliseconds = 300;
        private const int AfterLightOffMilliseconds = 1500;
        private const float ZoomFovOffset = 10f;
        private const int ActivePriority = 10;

        ///<summary>演出が終わったか</summary>
        public bool IsFinished => _isFinished;

        [SerializeField] private CinemachineCamera _playerCamera;
        [SerializeField] private CinemachineCamera _bookCam;
        [SerializeField] private Animator _bookShellAnimator;

        private IPlayerAvatar _player;
        private CameraView _cameraView;
        private bool _isFinished;

        ///<summary>
        ///依存を受け取る
        ///</summary>
        [Inject]
        public void Construct(IPlayerAvatar player, CameraView cameraView)
        {
            _player = player;
            _cameraView = cameraView;
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
            var brain = CinemachineCore.FindPotentialTargetBrain(_bookCam);
            _player.LockInput();
            _cameraView.SetLookInputEnabled(false);

            // 目の位置のまま本棚の中心を向かせ、少し寄る。向きの補間はBrainのブレンドに任せる
            var eye = _playerCamera.transform.position;
            _bookCam.transform.SetPositionAndRotation(eye, Quaternion.LookRotation(BookShellCenter() - eye));
            var lens = _playerCamera.Lens;
            lens.FieldOfView -= ZoomFovOffset;
            _bookCam.Lens = lens;
            _bookCam.Priority = ActivePriority;
            await WaitBlendAsync(brain, ct);

            await UniTask.Delay(BeforeLightOffMilliseconds, cancellationToken: ct);
            _bookShellAnimator.SetTrigger("LightOff");
            await UniTask.Delay(AfterLightOffMilliseconds, cancellationToken: ct);

            // 元の向きへ戻すと往復して見えるので、プレイヤーの視点を本棚向きに揃えてから返す
            AlignPlayerLook(_bookCam.transform.rotation);
            _bookCam.Priority = 0;
            await WaitBlendAsync(brain, ct);

            _cameraView.SetLookInputEnabled(true);
            _player.UnlockInput();
            _isFinished = true;
        }

        private Vector3 BookShellCenter()
        {
            var renderers = _bookShellAnimator.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return _bookShellAnimator.transform.position;

            var bounds = renderers[0].bounds;
            foreach (var r in renderers) bounds.Encapsulate(r.bounds);
            return bounds.center;
        }

        private void AlignPlayerLook(Quaternion rotation)
        {
            if (!_playerCamera.TryGetComponent(out CinemachinePanTilt panTilt)) return;

            var euler = rotation.eulerAngles;
            panTilt.PanAxis.Value = panTilt.PanAxis.ClampValue(Mathf.DeltaAngle(0f, euler.y));
            panTilt.TiltAxis.Value = panTilt.TiltAxis.ClampValue(Mathf.DeltaAngle(0f, euler.x));
        }

        private static async UniTask WaitBlendAsync(CinemachineBrain brain, CancellationToken ct)
        {
            // 優先度の変更はBrainの次の更新で反映されるので、ブレンドが始まるのを待ってから終わりを待つ
            await UniTask.DelayFrame(2, cancellationToken: ct);
            if (brain == null) return;
            await UniTask.WaitWhile(() => brain.IsBlending, cancellationToken: ct);
        }
    }
}
