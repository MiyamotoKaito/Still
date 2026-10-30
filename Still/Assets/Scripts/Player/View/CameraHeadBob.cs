using Unity.Cinemachine;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///歩いているときだけカメラを揺らす
    ///</summary>
    public class CameraHeadBob : MonoBehaviour
    {
        [Header("Head Bob")]
        [SerializeField] private float _amplitude = 1;
        [SerializeField] private float _frequency = 1;

        private PlayerView _playerView;
        private CinemachineBasicMultiChannelPerlin _noise;

        private void Awake()
        {
            _playerView = GetComponent<PlayerView>();
            _noise = FindAnyObjectByType<CinemachineBasicMultiChannelPerlin>();
        }

        private void LateUpdate()
        {
            if (_playerView.CurrentMoveValue.magnitude <= 0.1f)
            {
                _noise.AmplitudeGain = 0;
                _noise.FrequencyGain = 0;
                return;
            }

            float scale = _playerView.IsDash ? 2f : 1f;
            _noise.AmplitudeGain = _amplitude * scale;
            _noise.FrequencyGain = _frequency * scale;
        }
    }
}
