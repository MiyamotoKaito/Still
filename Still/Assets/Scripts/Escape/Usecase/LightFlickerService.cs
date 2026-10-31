using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;
using Still.Utility;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///恐怖演出のために、近くで点いている照明をちらつかせる
    ///</summary>
    // 点灯状態（ゴーストやミッションが参照する）は変えず、見た目だけを点滅させて最後に元へ戻す
    public sealed class LightFlickerService : IAmbientLightFlicker
    {
        private const int MinIntervalMilliseconds = 40;
        private const int MaxIntervalMilliseconds = 160;

        public LightFlickerService(SceneComponentList<LightSwitch> lightSwitches)
        {
            _lightSwitches = lightSwitches;
        }

        private readonly SceneComponentList<LightSwitch> _lightSwitches;

        ///<summary>
        ///指定位置の近くで点いている照明をしばらくちらつかせる
        ///</summary>
        public async UniTask FlickerNearAsync(Vector3 position, float radius, float duration, CancellationToken ct)
        {
            var targets = _lightSwitches.Items
                .Where(s => s.IsOn && (s.LightPosition - position).sqrMagnitude <= radius * radius)
                .ToList();
            if (targets.Count == 0) return;

            try
            {
                float end = Time.time + duration;
                while (Time.time < end)
                {
                    bool isVisible = Random.value > 0.4f;
                    foreach (var target in targets) target.SetLightVisible(isVisible);
                    await UniTask.Delay(Random.Range(MinIntervalMilliseconds, MaxIntervalMilliseconds), cancellationToken: ct);
                }
            }
            finally
            {
                foreach (var target in targets)
                {
                    if (target != null) target.SetLightVisible(target.IsOn);
                }
            }
        }
    }
}
