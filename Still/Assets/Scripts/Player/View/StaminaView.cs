using UnityEngine;
using UnityEngine.UI;

namespace Still.Player
{
    ///<summary>
    ///スタミナゲージ
    ///</summary>
    public class StaminaView : MonoBehaviour
    {
        // PresenterはスコープのAwake（このViewのAwakeより前）で購読するため、取得を遅らせる
        private Image Gauge => _staminaGauge != null ? _staminaGauge : _staminaGauge = GetComponent<Image>();

        private Image _staminaGauge;

        ///<summary>
        ///ゲージの残量を割合（0〜1）で表示する
        ///</summary>
        public void UpdateStamina(float ratio)
        {
            Gauge.fillAmount = ratio;
        }
    }
}
