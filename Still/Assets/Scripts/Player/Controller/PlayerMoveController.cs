using Still.Common;
using UnityEngine;
using VContainer.Unity;

namespace Still.Player
{
    ///<summary>
    ///移動入力をカメラ基準の移動に変換し、ダッシュによる消耗を反映する
    ///</summary>
    public sealed class PlayerMoveController : IStartable, ITickable
    {
        public PlayerMoveController(IPlayerController player, IPlayerAvatar avatar, CameraView cameraView,
                                    PlayerStatusUsecase status, PlayerConfig config)
        {
            _player = player;
            _avatar = avatar;
            _cameraView = cameraView;
            _status = status;
            _config = config;
        }

        private readonly IPlayerController _player;
        private readonly IPlayerAvatar _avatar;
        private readonly CameraView _cameraView;
        private readonly PlayerStatusUsecase _status;
        private readonly PlayerConfig _config;

        ///<summary>
        ///操作を受け付け始める
        ///</summary>
        public void Start()
        {
            _avatar.UnlockInput();
        }

        ///<summary>
        ///1フレーム分移動させる
        ///</summary>
        public void Tick()
        {
            _status.Tick(_player.IsDash, Time.deltaTime);

            var cameraTransform = _cameraView.transform;
            var forward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
            var right = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;
            var input = _player.CurrentMoveValue;
            var direction = forward * input.y + right * input.x;

            float speed = _player.IsDash && _status.CanDash ? _config.PlayerMaxSpeed : _config.PlayerDefaultSpeed;
            _player.Move(direction, speed);
        }
    }
}
