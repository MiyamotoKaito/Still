using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Still.Player
{
    ///<summary>
    ///画面効果を一瞬強めてから戻す（画面の乱れ）
    ///</summary>
    [Serializable]
    public class PostProcessPulse : FearActionBase
    {
        [SerializeField] private FearPostProcessParameter _parameter = FearPostProcessParameter.ChromaticAberration;
        [SerializeField, Tooltip("最大の強さ（彩度はマイナス）")] private float _peak = 1f;
        [SerializeField] private float _duration = 0.5f;

        protected override UniTask PlayCoreAsync(FearEffectContext context, CancellationToken ct)
        {
            return RunForAsync(_duration, t => context.PostProcess.AddPulse(_parameter, _peak * (1f - t) * (1f - t)), ct);
        }
    }

    ///<summary>
    ///画面に色を一瞬重ねる（赤いフラッシュなど）
    ///</summary>
    [Serializable]
    public class ColorFlash : FearActionBase
    {
        [SerializeField] private Color _color = new Color(0.8f, 0f, 0f, 0.8f);
        [SerializeField] private float _duration = 0.3f;

        protected override UniTask PlayCoreAsync(FearEffectContext context, CancellationToken ct)
        {
            return RunForAsync(_duration, t => context.PostProcess.AddFlash(new Color(_color.r, _color.g, _color.b, _color.a * (1f - t))), ct);
        }
    }

    ///<summary>
    ///カメラを小刻みに揺らす
    ///</summary>
    [Serializable]
    public class CameraShake : FearActionBase
    {
        [SerializeField, Tooltip("傾きの最大角度（度）")] private float _amplitude = 2f;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField, Tooltip("1秒あたりの揺れの回数")] private float _frequency = 18f;

        protected override async UniTask PlayCoreAsync(FearEffectContext context, CancellationToken ct)
        {
            try
            {
                float start = Time.time;
                await RunForAsync(_duration, t => context.ShakeDutch = Mathf.Sin((Time.time - start) * _frequency * Mathf.PI * 2f) * _amplitude * (1f - t), ct);
            }
            finally
            {
                context.ShakeDutch = 0f;
            }
        }
    }

    ///<summary>
    ///耳元でSEを鳴らす
    ///</summary>
    [Serializable]
    public class PlaySound : FearActionBase
    {
        [SerializeField, Tooltip("AudioManagerのSEリストに登録した名前")] private string _soundName = "Scream";

        protected override UniTask PlayCoreAsync(FearEffectContext context, CancellationToken ct)
        {
            context.AudioService.PlaySe(_soundName);
            return UniTask.CompletedTask;
        }
    }

    ///<summary>
    ///プレイヤーの近く（主に背後）で物音を立てる
    ///</summary>
    [Serializable]
    public class PlaySoundNearby : FearActionBase
    {
        private const float SourceLifetime = 6f;

        [SerializeField, Tooltip("AudioManagerのSEリストに登録した名前")] private string _soundName = "Knock";
        [SerializeField, Tooltip("プレイヤーからの距離")] private float _distance = 5f;
        [SerializeField, Tooltip("正面から何度以上離れた方向で鳴らすか（180=真後ろだけ）")] private float _minAngleFromForward = 90f;

        protected override UniTask PlayCoreAsync(FearEffectContext context, CancellationToken ct)
        {
            var player = context.Player.Transform.position;
            var forward = context.Camera.transform.forward;
            forward.y = 0f;
            float angle = Random.Range(_minAngleFromForward, 180f) * (Random.value < 0.5f ? -1f : 1f);
            var direction = Quaternion.Euler(0f, angle, 0f) * forward.normalized;
            var source = context.Audio.CreateOneShotSourceAt(player + direction * _distance + Vector3.up, SourceLifetime);
            context.AudioService.PlaySe(_soundName, source);
            return UniTask.CompletedTask;
        }
    }

    ///<summary>
    ///SAN値を少し削る
    ///</summary>
    [Serializable]
    public class SanDamage : FearActionBase
    {
        [SerializeField] private float _amount = 3f;

        protected override UniTask PlayCoreAsync(FearEffectContext context, CancellationToken ct)
        {
            context.San.Damage(_amount);
            return UniTask.CompletedTask;
        }
    }

    ///<summary>
    ///懐中電灯をちらつかせる
    ///</summary>
    [Serializable]
    public class FlashlightFlicker : FearActionBase
    {
        [SerializeField] private float _duration = 1f;

        protected override UniTask PlayCoreAsync(FearEffectContext context, CancellationToken ct) => context.FlickerFlashlightAsync(_duration, ct);
    }

    ///<summary>
    ///近くの部屋の照明をちらつかせる
    ///</summary>
    [Serializable]
    public class RoomLightFlicker : FearActionBase
    {
        [SerializeField] private float _radius = 12f;
        [SerializeField] private float _duration = 1.5f;

        protected override UniTask PlayCoreAsync(FearEffectContext context, CancellationToken ct)
        {
            return context.LightFlicker.FlickerNearAsync(context.Player.Transform.position, _radius, _duration, ct);
        }
    }

    ///<summary>
    ///視界の端に偽のゴーストが立つ。真っすぐ見ると消える
    ///</summary>
    [Serializable]
    public class PhantomAppear : FearActionBase
    {
        private const int Attempts = 16;
        private const float MinDistance = 3f;
        private const float MinAngle = 15f;
        private const float FloorProbeHeight = 1.5f;
        private const float HeadHeight = 1.2f;

        [SerializeField, Tooltip("プレイヤーからの距離")] private float _distance = 7f;
        [SerializeField, Tooltip("正面から左右にずらす角度（視界の端に出す）")] private float _angleFromForward = 40f;
        [SerializeField, Tooltip("見られなかった場合に消えるまでの秒数")] private float _duration = 2f;
        [SerializeField, Tooltip("正面からこの角度以内に入ったら消える")] private float _vanishAngle = 12f;

        protected override async UniTask PlayCoreAsync(FearEffectContext context, CancellationToken ct)
        {
            if (context.Phantom.IsVisible || !TryFindSpot(context, out var spot)) return;

            context.Phantom.Show(spot, context.Player.Transform.position);
            try
            {
                float end = Time.time + _duration;
                while (Time.time < end && !IsLookedAt(context)) await UniTask.Yield(ct);
            }
            finally
            {
                context.Phantom.Hide();
            }
        }

        // 見通しが通っていて、歩ける場所（床の上）を探す
        private bool TryFindSpot(FearEffectContext context, out Vector3 spot)
        {
            var eye = context.Camera.transform.position;
            var forward = context.Camera.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            for (int i = 0; i < Attempts; i++)
            {
                // 狭い部屋や廊下でも見つかるよう、試すたびに少しずつ近く・正面寄りにする
                float t = i / (float)(Attempts - 1);
                float distance = Mathf.Lerp(_distance, MinDistance, t);
                float angle = Mathf.Lerp(_angleFromForward, MinAngle, t) * (Random.value < 0.5f ? -1f : 1f);
                var candidate = context.Player.Transform.position + Quaternion.Euler(0f, angle, 0f) * forward * distance;
                // 見た目だけで歩かないので、NavMeshの無い部屋（ゴーストが入れない部屋）でも床さえあれば出す
                if (!Physics.Raycast(candidate + Vector3.up * FloorProbeHeight, Vector3.down, out var floor, FloorProbeHeight * 2f,
                                     Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) continue;

                var head = floor.point + Vector3.up * HeadHeight;
                if (Physics.Linecast(eye, head, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) continue;

                spot = floor.point;
                return true;
            }
            spot = default;
            return false;
        }

        private bool IsLookedAt(FearEffectContext context)
        {
            var toPhantom = context.Phantom.Position + Vector3.up * HeadHeight - context.Camera.transform.position;
            return Vector3.Angle(context.Camera.transform.forward, toPhantom) < _vanishAngle;
        }
    }
}
