using UnityEngine;
using UnityEngine.UI;

namespace Still.Player
{
    ///<summary>
    ///SAN値の表示（ビデオカメラのバッテリー残量のような区切りゲージ）
    ///</summary>
    public class SANView : MonoBehaviour
    {
        [SerializeField] private Image[] _segments;
        [SerializeField] private Graphic _label;
        [SerializeField] private Color _normalColor = new(0.92f, 0.92f, 0.88f, 0.9f);
        [SerializeField] private Color _emptyColor = new(1f, 1f, 1f, 0.12f);
        [SerializeField] private Color _dangerColor = new(0.85f, 0.12f, 0.1f, 0.95f);
        [SerializeField, Range(0f, 1f)] private float _dangerRatio = 0.3f;
        [SerializeField] private float _blinkInterval = 0.4f;

        private float _ratio = 1f;

        ///<summary>
        ///SAN値を割合（0〜1）で表示する
        ///</summary>
        public void UpdateSAN(float ratio)
        {
            _ratio = Mathf.Clamp01(ratio);
            Refresh();
        }

        // 危険域では残りの目盛りを点滅させるため、毎フレーム描き直す
        private void Update()
        {
            if (_ratio <= _dangerRatio) Refresh();
        }

        private void Refresh()
        {
            if (_segments == null || _segments.Length == 0) return;

            bool isDanger = _ratio <= _dangerRatio;
            bool isBlinkOff = isDanger && Mathf.Repeat(Time.unscaledTime, _blinkInterval * 2f) >= _blinkInterval;
            // 0%になるまでは最低1目盛り残し、まだ正気が残っていることを見せる
            int filled = _ratio <= 0f ? 0 : Mathf.Max(1, Mathf.CeilToInt(_ratio * _segments.Length));
            var fillColor = isDanger ? _dangerColor : _normalColor;

            for (int i = 0; i < _segments.Length; i++)
            {
                bool isFilled = i < filled;
                _segments[i].color = isFilled && !isBlinkOff ? fillColor : _emptyColor;
            }
            if (_label != null) _label.color = isDanger ? _dangerColor : _normalColor;
        }
    }
}
