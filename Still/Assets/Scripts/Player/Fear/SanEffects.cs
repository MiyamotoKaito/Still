using System;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///画面効果を恐怖の度合いに応じて強める
    ///</summary>
    [Serializable]
    public class PostProcessByFear : ISanEffect
    {
        [SerializeField] private FearPostProcessParameter _parameter;
        [SerializeField, Tooltip("横軸=恐怖の度合い(0〜1)、縦軸=効果の値（彩度は-100〜0）")] private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 0.5f);

        ///<summary>
        ///反映する
        ///</summary>
        public void Apply(float fear, FearEffectContext context) => context.PostProcess.SetBase(_parameter, _curve.Evaluate(fear));

        ///<summary>
        ///元に戻す
        ///</summary>
        public void Reset(FearEffectContext context) => context.PostProcess.SetBase(_parameter, 0f);
    }

    ///<summary>
    ///画面効果を恐怖の度合いに応じた強さで、ときどき波のように強める
    ///</summary>
    // 歪みのように常にかかると見づらいだけの効果は、強めては戻すのを繰り返す
    [Serializable]
    public class PostProcessSurgeByFear : ISanEffect
    {
        [SerializeField] private FearPostProcessParameter _parameter;
        [SerializeField, Tooltip("横軸=恐怖の度合い(0〜1)、縦軸=波の頂点での効果の値")] private AnimationCurve _curve = AnimationCurve.Linear(0.7f, 0f, 1f, -0.2f);
        [SerializeField, Tooltip("1回の波で強まってから戻るまでの秒数")] private float _duration = 2.5f;
        [SerializeField, Tooltip("次の波までの秒数（最小〜最大）")] private Vector2 _interval = new(4f, 9f);

        [NonSerialized] private float _untilNext;
        [NonSerialized] private float _elapsed = float.MaxValue;

        ///<summary>
        ///反映する
        ///</summary>
        public void Apply(float fear, FearEffectContext context)
        {
            float peak = _curve.Evaluate(fear);
            // 効果が出ない間は待たせない。SAN値が下がった直後に最初の波が来る
            if (Mathf.Approximately(peak, 0f))
            {
                Reset(context);
                return;
            }

            _elapsed += Time.deltaTime;
            _untilNext -= Time.deltaTime;
            if (_untilNext <= 0f)
            {
                _elapsed = 0f;
                _untilNext = _duration + UnityEngine.Random.Range(_interval.x, _interval.y);
            }

            float progress = _elapsed / _duration;
            float envelope = progress < 1f ? Mathf.Sin(progress * Mathf.PI) : 0f;
            context.PostProcess.SetBase(_parameter, peak * envelope);
        }

        ///<summary>
        ///元に戻す
        ///</summary>
        public void Reset(FearEffectContext context)
        {
            _untilNext = 0f;
            _elapsed = float.MaxValue;
            context.PostProcess.SetBase(_parameter, 0f);
        }
    }

    ///<summary>
    ///心音や息遣いをループさせ、恐怖の度合いで音量と速さを変える
    ///</summary>
    // 音はAudioManagerのBGMリストに名前で登録する。未登録なら最初の1回だけ警告して鳴らさない
    [Serializable]
    public class LoopSoundByFear : ISanEffect
    {
        private const float Silent = 0.001f;

        [SerializeField, Tooltip("AudioManagerのBGMリストに登録した名前")] private string _soundName = "HeartBeat";
        [SerializeField, Tooltip("横軸=恐怖の度合い、縦軸=音量の倍率")] private AnimationCurve _volume = AnimationCurve.Linear(0.3f, 0f, 1f, 1f);
        [SerializeField, Tooltip("横軸=恐怖の度合い、縦軸=再生速度（心拍が速くなる）")] private AnimationCurve _pitch = AnimationCurve.Linear(0f, 0.9f, 1f, 1.4f);

        [NonSerialized] private bool _isUnavailable;
        [NonSerialized] private float _baseVolume = -1f;

        ///<summary>
        ///反映する
        ///</summary>
        public void Apply(float fear, FearEffectContext context)
        {
            if (_isUnavailable) return;

            var source = context.Audio.GetLoopSource(_soundName);
            float volume = _volume.Evaluate(fear);
            if (volume <= Silent)
            {
                if (source.isPlaying) source.Stop();
                return;
            }

            if (!source.isPlaying)
            {
                context.AudioService.PlayBgm(_soundName, source);
                if (!source.isPlaying)
                {
                    _isUnavailable = true;
                    return;
                }
                // PlayBgmは3Dで鳴らすので、耳元で鳴る2Dに戻す
                source.spatialBlend = 0f;
                if (_baseVolume < 0f) _baseVolume = source.volume;
            }

            source.volume = _baseVolume * volume;
            source.pitch = _pitch.Evaluate(fear);
        }

        ///<summary>
        ///止めて、次の再生で登録状況を確認し直す
        ///</summary>
        public void Reset(FearEffectContext context)
        {
            _isUnavailable = false;
            _baseVolume = -1f;
            context.Audio.GetLoopSource(_soundName).Stop();
        }
    }

    ///<summary>
    ///恐怖の度合いに応じて視界がゆっくり傾く（めまい）
    ///</summary>
    [Serializable]
    public class ViewSwayByFear : ISanEffect
    {
        [SerializeField, Tooltip("横軸=恐怖の度合い、縦軸=傾きの最大角度（度）")] private AnimationCurve _amplitude = AnimationCurve.Linear(0.6f, 0f, 1f, 3f);
        [SerializeField, Tooltip("1秒あたりの揺れの回数")] private float _frequency = 0.25f;

        ///<summary>
        ///反映する
        ///</summary>
        public void Apply(float fear, FearEffectContext context)
        {
            // 単純な往復だと機械的なので、2つの周期を重ねて不規則にする
            float t = Time.time * _frequency * Mathf.PI * 2f;
            float wave = Mathf.Sin(t) * 0.7f + Mathf.Sin(t * 2.3f + 1.3f) * 0.3f;
            context.SwayDutch = Mathf.Max(0f, _amplitude.Evaluate(fear)) * wave;
        }

        ///<summary>
        ///元に戻す
        ///</summary>
        public void Reset(FearEffectContext context) => context.SwayDutch = 0f;
    }

    ///<summary>
    ///恐怖で息が上がり、スタミナの回復が遅くなる
    ///</summary>
    [Serializable]
    public class StaminaRecoveryByFear : ISanEffect
    {
        [SerializeField, Tooltip("横軸=恐怖の度合い、縦軸=スタミナ回復の倍率")] private AnimationCurve _multiplier = AnimationCurve.Linear(0f, 1f, 1f, 0.4f);

        ///<summary>
        ///反映する
        ///</summary>
        public void Apply(float fear, FearEffectContext context) => context.Status.RecoveryMultiplier = Mathf.Max(0f, _multiplier.Evaluate(fear));

        ///<summary>
        ///元に戻す
        ///</summary>
        public void Reset(FearEffectContext context) => context.Status.RecoveryMultiplier = 1f;
    }
}
