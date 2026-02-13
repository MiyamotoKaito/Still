using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DepthOfFieldPresenter
{
    private PlayerSensorView _playerSensorView;
    private Volume _postProcessingVolume;

    public DepthOfFieldPresenter(PlayerSensorView playerSensorView, Volume postProcessingVolume)
    {
        _playerSensorView = playerSensorView;
        _postProcessingVolume = postProcessingVolume;
        _playerSensorView.OnHitDetected += HandleHitDetected;
        _playerSensorView.OnHitLost += HandleHitLost;
    }
    private void HandleHitDetected(RaycastHit hit)
    {
        if (_postProcessingVolume.profile.TryGet<DepthOfField>(out var depthOfField))
        {
            depthOfField.focusMode.value = DepthOfFieldMode.Manual;

            // 焦点距離をヒットしたオブジェクトの距離に設定
            depthOfField.nearFocusStart.value = 0;
            depthOfField.nearFocusEnd.value = hit.distance;

            // 遠くの焦点距離をヒットしたオブジェクトの距離に設定
            depthOfField.farFocusStart.value = hit.distance;
            depthOfField.farFocusEnd.value = hit.distance + 10f;
        }
    }
    private void HandleHitLost()
    {
        if (_postProcessingVolume.profile.TryGet<DepthOfField>(out var depthOfField))
        {
            depthOfField.focusMode.value = DepthOfFieldMode.Off;
        }
    }
    public void Dispose()
    {
        _playerSensorView.OnHitDetected -= HandleHitDetected;
        _playerSensorView.OnHitLost -= HandleHitLost;
    }
}
