using Cysharp.Threading.Tasks;
using Still.Player.View;
using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public class FocusMeEvent : IEvent
{
    [SerializeField]
    private float _focusDuration;
    [SerializeField]
    private float _focusFOV;
    private PlayerView _player;
    private CameraView _cameraView;
    private CinemachineCamera _cinemachineCamera;
    private bool _isFinished;

    public bool IsFinished => _isFinished;

    public void Initialize()
    {
        _player = GameObject.FindAnyObjectByType<PlayerView>();
        _cameraView = GameObject.FindAnyObjectByType<CameraView>();
        _cinemachineCamera = _cameraView.GetComponent<CinemachineCamera>();
        _player.DisablePlayerInput();
    }

    public void OnEvent()
    {
        FocusMe().Forget();
    }
    private async UniTask FocusMe()
    {
        float duration = _focusDuration;
        float elapsed = 0f;
        float initialFOV = _cinemachineCamera.Lens.FieldOfView;

        // Focus in
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(initialFOV, _focusFOV, t);
            await UniTask.Yield();
        }

        elapsed = 0f;
        // Focus out
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            _cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(_focusFOV, initialFOV, t);
            await UniTask.Yield();
        }
        _player.EnablePlayerInput();
        _isFinished = true;
    }
}
