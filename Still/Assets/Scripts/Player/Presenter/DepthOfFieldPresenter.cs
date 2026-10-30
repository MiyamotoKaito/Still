using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using VContainer.Unity;

namespace Still.Player
{
    ///<summary>
    ///視線の先にピントを合わせる
    ///</summary>
    public sealed class DepthOfFieldPresenter : IInitializable, IDisposable
    {
        private const float FarFocusRange = 10f;

        public DepthOfFieldPresenter(PlayerSensorView sensorView, Volume volume)
        {
            _sensorView = sensorView;
            _volume = volume;
        }

        private readonly PlayerSensorView _sensorView;
        private readonly Volume _volume;

        ///<summary>
        ///購読を始める
        ///</summary>
        public void Initialize()
        {
            _sensorView.OnHitDetected += HandleHit;
            _sensorView.OnHitPlayerLost += HandleLost;
        }

        ///<summary>
        ///購読をやめる
        ///</summary>
        public void Dispose()
        {
            _sensorView.OnHitDetected -= HandleHit;
            _sensorView.OnHitPlayerLost -= HandleLost;
        }

        private void HandleHit(RaycastHit hit)
        {
            if (!_volume.profile.TryGet<DepthOfField>(out var depthOfField)) return;

            depthOfField.focusMode.value = DepthOfFieldMode.Manual;
            depthOfField.nearFocusStart.value = 0;
            depthOfField.nearFocusEnd.value = hit.distance;
            depthOfField.farFocusStart.value = hit.distance;
            depthOfField.farFocusEnd.value = hit.distance + FarFocusRange;
        }

        private void HandleLost()
        {
            if (!_volume.profile.TryGet<DepthOfField>(out var depthOfField)) return;
            depthOfField.focusMode.value = DepthOfFieldMode.Off;
        }
    }
}
