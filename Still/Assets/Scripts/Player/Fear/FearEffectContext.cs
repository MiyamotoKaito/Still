using System.Threading;
using Cysharp.Threading.Tasks;
using Still.Common;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///恐怖演出が触れるもの一式（SerializeReferenceの演出データはDIできないので、これを渡して使わせる）
    ///</summary>
    public sealed class FearEffectContext
    {
        private const int MinFlickerMilliseconds = 40;
        private const int MaxFlickerMilliseconds = 140;

        public FearEffectContext(FearPostProcessView postProcess, FearAudioView audio, PhantomView phantom, CameraView camera,
                                 PlayerSensorView sensor, SpotLightEntity spotLight, IPlayerAvatar player, ISanUsecase san,
                                 PlayerStatusUsecase status, IAudioService audioService, IAmbientLightFlicker lightFlicker)
        {
            PostProcess = postProcess;
            Audio = audio;
            Phantom = phantom;
            Camera = camera;
            Player = player;
            San = san;
            Status = status;
            AudioService = audioService;
            LightFlicker = lightFlicker;
            _sensor = sensor;
            _spotLight = spotLight;
        }

        ///<summary>画面効果</summary>
        public FearPostProcessView PostProcess { get; }
        ///<summary>演出用の音源</summary>
        public FearAudioView Audio { get; }
        ///<summary>幻覚のゴースト</summary>
        public PhantomView Phantom { get; }
        ///<summary>プレイヤーのカメラ</summary>
        public CameraView Camera { get; }
        ///<summary>プレイヤー</summary>
        public IPlayerAvatar Player { get; }
        ///<summary>SAN値の操作</summary>
        public ISanUsecase San { get; }
        ///<summary>スタミナなどのルール</summary>
        public PlayerStatusUsecase Status { get; }
        ///<summary>SE/BGMの再生</summary>
        public IAudioService AudioService { get; }
        ///<summary>部屋の照明のちらつき</summary>
        public IAmbientLightFlicker LightFlicker { get; }
        ///<summary>常時のめまいによるカメラの傾き（度）</summary>
        public float SwayDutch { get; set; }
        ///<summary>反応の揺れによるカメラの傾き（度）</summary>
        public float ShakeDutch { get; set; }
        ///<summary>シーンの破棄でViewが無くなったか（エディタで再生を止めた直後など）</summary>
        public bool IsDisposed => PostProcess == null || Audio == null || Camera == null;

        private readonly PlayerSensorView _sensor;
        private readonly SpotLightEntity _spotLight;

        ///<summary>
        ///カメラの傾きを反映する
        ///</summary>
        public void ApplyCamera()
        {
            Camera.SetDutch(SwayDutch + ShakeDutch);
        }

        ///<summary>
        ///懐中電灯をしばらくちらつかせ、最後にプレイヤーが選んだ点灯状態へ戻す
        ///</summary>
        public async UniTask FlickerFlashlightAsync(float duration, CancellationToken ct)
        {
            var light = _sensor.SpotLight;
            try
            {
                float end = Time.time + duration;
                while (Time.time < end)
                {
                    light.enabled = _spotLight.IsOn.CurrentValue && Random.value > 0.5f;
                    await UniTask.Delay(Random.Range(MinFlickerMilliseconds, MaxFlickerMilliseconds), cancellationToken: ct);
                }
            }
            finally
            {
                if (light != null) light.enabled = _spotLight.IsOn.CurrentValue;
            }
        }
    }
}
