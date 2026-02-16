using Cysharp.Threading.Tasks;
using Still.Camera.Model;
using Still.Player;
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
    private bool _isFinished;
    private GameInstaller _gameInstaller;
    private FieldOfViewModel _fovModel;
    private CameraView _cameraView;
    private CinemachineInputAxisController _axis;

    public bool IsFinished => _isFinished;

    public void Initialize()
    {
        _player = GameObject.FindAnyObjectByType<PlayerView>();
        _gameInstaller = GameObject.FindAnyObjectByType<GameInstaller>();
        _fovModel = _gameInstaller.FieldOfViewModel;
        _cameraView = GameObject.FindAnyObjectByType<CameraView>();
        _axis = _cameraView.GetComponent<CinemachineInputAxisController>();
        _axis.enabled = false; // プレイヤーのカメラ操作を無効化
        _player.DisablePlayerInput();
    }

    public void OnEvent()
    {
        FocusMe().Forget();
    }
    private async UniTask FocusMe()
    {
        float initialFOV = _fovModel.FOV;

        // Focus in
        await LerpFOV(initialFOV, _focusFOV, _focusDuration);
        await UniTask.Delay((int)(_focusDuration * 1000)); // Focus状態を維持
                                                           // Focus out
        _fovModel.FOV = initialFOV;

        _player.EnablePlayerInput();
        _axis.enabled = true; // プレイヤーのカメラ操作を有効化
        _isFinished = true;
    }
    private async UniTask LerpFOV(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Modelの値を更新（UpdateFOVで反映される）
            _fovModel.FOV = Mathf.Lerp(from, to, t);

            await UniTask.Yield();
        }

        // 最終値を確実に設定
        _fovModel.FOV = to;
    }
}
