using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Still.Player
{
    ///<summary>
    ///ビデオカメラの録画表示（点滅する赤丸とタイムコード）
    ///</summary>
    public class RecIndicatorView : MonoBehaviour
    {
        [SerializeField] private Graphic _dot;
        [SerializeField] private TextMeshProUGUI _timecode;
        [SerializeField] private float _blinkInterval = 0.6f;

        private int _shownSeconds = -1;

        private void Update()
        {
            if (_dot != null) _dot.enabled = Mathf.Repeat(Time.unscaledTime, _blinkInterval * 2f) < _blinkInterval;

            // 文字列の生成は秒が変わったときだけにする
            int seconds = (int)Time.timeSinceLevelLoad;
            if (_timecode == null || seconds == _shownSeconds) return;

            _shownSeconds = seconds;
            _timecode.text = $"{seconds / 3600:00}:{seconds / 60 % 60:00}:{seconds % 60:00}";
        }
    }
}
