using UnityEngine;
using UnityEngine.UI;
namespace Still.Player.View
{
    public class StaminaView : MonoBehaviour
    {
        private Image _staminaGauge;

        private void Start()
        {
            _staminaGauge = GetComponent<Image>();
        }

        public void UpdateStamina(float value)
        {
            _staminaGauge.fillAmount = value / 100;
            Debug.Log($"StaminaView: スタミナ更新 {value}");
        }
    }
}
