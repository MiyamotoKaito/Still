using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Still.Player
{
    ///<summary>
    ///恐怖演出専用のポストプロセス（Global Volumeとは別に、優先度の高いVolumeで上書きする）
    ///</summary>
    // 値が0の効果は上書きしないので、恐怖を感じていない間はGlobal Volumeの見た目がそのまま出る
    [RequireComponent(typeof(Volume))]
    public class FearPostProcessView : MonoBehaviour
    {
        private const float Epsilon = 0.001f;
        private const float VolumePriority = 100f;

        private static readonly int ParameterCount = System.Enum.GetValues(typeof(FearPostProcessParameter)).Length;

        private VolumeProfile _profile;
        private Vignette _vignette;
        private ColorAdjustments _colorAdjustments;
        private ChromaticAberration _chromaticAberration;
        private FilmGrain _filmGrain;
        private LensDistortion _lensDistortion;
        private Color _flash = Color.clear;

        private readonly float[] _base = new float[ParameterCount];
        private readonly float[] _pulse = new float[ParameterCount];

        ///<summary>
        ///恐怖の度合いから決まる常時の値を設定する
        ///</summary>
        public void SetBase(FearPostProcessParameter parameter, float value)
        {
            _base[(int)parameter] = value;
        }

        ///<summary>
        ///このフレームだけ一時的に加える値（反応の演出用。毎フレーム呼ぶ）
        ///</summary>
        public void AddPulse(FearPostProcessParameter parameter, float value)
        {
            // 同じ効果に複数の演出が重なったときは、強い方だけを使う
            ref var current = ref _pulse[(int)parameter];
            if (Mathf.Abs(value) > Mathf.Abs(current)) current = value;
        }

        ///<summary>
        ///このフレームだけ画面に色を重ねる（毎フレーム呼ぶ）
        ///</summary>
        public void AddFlash(Color color)
        {
            if (color.a > _flash.a) _flash = color;
        }

        private void Awake()
        {
            var volume = GetComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = VolumePriority;

            _profile = ScriptableObject.CreateInstance<VolumeProfile>();
            _vignette = _profile.Add<Vignette>();
            _colorAdjustments = _profile.Add<ColorAdjustments>();
            _chromaticAberration = _profile.Add<ChromaticAberration>();
            _filmGrain = _profile.Add<FilmGrain>();
            _filmGrain.type.Override(FilmGrainLookup.Medium3);
            _lensDistortion = _profile.Add<LensDistortion>();
            volume.profile = _profile;
        }

        private void LateUpdate()
        {
            Apply(_vignette.intensity, Value(FearPostProcessParameter.Vignette));
            Apply(_colorAdjustments.saturation, Value(FearPostProcessParameter.Saturation));
            Apply(_chromaticAberration.intensity, Value(FearPostProcessParameter.ChromaticAberration));
            Apply(_filmGrain.intensity, Value(FearPostProcessParameter.FilmGrain));
            Apply(_lensDistortion.intensity, Value(FearPostProcessParameter.LensDistortion));

            _colorAdjustments.colorFilter.overrideState = _flash.a > Epsilon;
            _colorAdjustments.colorFilter.value = Color.Lerp(Color.white, new Color(_flash.r, _flash.g, _flash.b, 1f), _flash.a);

            System.Array.Clear(_pulse, 0, _pulse.Length);
            _flash = Color.clear;
        }

        private void OnDestroy()
        {
            if (_profile != null) Destroy(_profile);
        }

        private float Value(FearPostProcessParameter parameter) => _base[(int)parameter] + _pulse[(int)parameter];

        private static void Apply(VolumeParameter<float> parameter, float value)
        {
            parameter.overrideState = Mathf.Abs(value) > Epsilon;
            parameter.value = value;
        }
    }
}
