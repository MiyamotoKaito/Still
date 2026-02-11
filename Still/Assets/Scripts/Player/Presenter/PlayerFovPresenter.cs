using Cysharp.Threading.Tasks;
using Still.Camera.Model;
using Still.Player.View;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerFovPresenter
{
    private PlayerView _playerView;
    private CameraView _cameraView;
    private FieldOfViewModel _fovModel;
    private CinemachineCamera _cinemachineCamera;

    public PlayerFovPresenter(PlayerView playerView, CameraView cameraView, FieldOfViewModel fovModel)
    {
        _playerView = playerView;
        _cameraView = cameraView;
        _fovModel = fovModel;

        _cinemachineCamera = _cameraView.GetComponent<CinemachineCamera>();
        _playerView.OnDashEvent += () => ModifyFOV(80);
        _playerView.OnDashCancelEvent +=  ResetFOV;
    }
    public void UpdateFOV()
    {
        _cinemachineCamera.Lens.FieldOfView = _fovModel.FOV;
    }

    public void ModifyFOV(float newFOV)
    {
        SmoothChangeFOV(newFOV, 0.5f).Forget();
    }

    public void ResetFOV()
    {
        SmoothChangeFOV(_fovModel.DefaultFOV, 0.5f).Forget();
    }

    private async UniTask SmoothChangeFOV(float targetFOV, float duration)
    {
        float startFOV = _fovModel.FOV; // 現在のFOVを保存
        float elapsed = 0f; // 経過時間

        while (elapsed < duration)
        {
            // 経過時間を更新
            elapsed += Time.deltaTime;
            // 経過時間の割合を計算
            var t = elapsed / duration;
            // FOVを線形補間で更新
            _fovModel.FOV = Mathf.Lerp(startFOV, targetFOV, t);
            // 次のフレームまで待機
            await UniTask.Yield();
        }

        _fovModel.FOV = targetFOV; // 最終的に目標FOVに設定
    }
    public void Dispose()
    {
        _playerView.OnDashEvent -= () => ModifyFOV(80);
        _playerView.OnDashCancelEvent -= ResetFOV;
    }
}
