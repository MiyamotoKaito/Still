using UnityEngine;
using UnityEngine.UI;

namespace Still.Player
{
    ///<summary>
    ///スタミナゲージ（細いバー。満タンのまましばらく経つと薄くなる）
    ///</summary>
    public class StaminaView : MonoBehaviour
    {
        [SerializeField] private RectTransform _fill;
        [SerializeField] private Image _fillImage;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Color _normalColor = new(0.92f, 0.92f, 0.88f, 0.9f);
        [SerializeField] private Color _lowColor = new(0.85f, 0.12f, 0.1f, 0.95f);
        [SerializeField, Range(0f, 1f)] private float _lowRatio = 0.25f;
        [SerializeField] private float _hideDelay = 2f;
        [SerializeField, Range(0f, 1f)] private float _idleAlpha = 0.25f;
        [SerializeField] private float _fadeSpeed = 2f;

        private float _ratio = 1f;
        private float _lastChangedTime = float.NegativeInfinity;

        ///<summary>
        ///ゲージの残量を割合（0〜1）で表示する
        ///</summary>
        public void UpdateStamina(float ratio)
        {
            _ratio = Mathf.Clamp01(ratio);
            if (_ratio < 1f) _lastChangedTime = Time.unscaledTime;

            if (_fill != null) _fill.anchorMax = new Vector2(_ratio, _fill.anchorMax.y);
            if (_fillImage != null) _fillImage.color = _ratio <= _lowRatio ? _lowColor : _normalColor;
        }

        // 走っていない間は目立たせないよう、満タンになって少し経ったら薄くする
        private void Update()
        {
            if (_group == null) return;

            bool isIdle = _ratio >= 1f && Time.unscaledTime - _lastChangedTime > _hideDelay;
            _group.alpha = Mathf.MoveTowards(_group.alpha, isIdle ? _idleAlpha : 1f, _fadeSpeed * Time.unscaledDeltaTime);
        }
    }
}
