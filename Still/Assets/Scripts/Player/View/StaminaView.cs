using UnityEngine;
using UnityEngine.UI;
namespace Still.Player.View
{
    public class StaminaView : MonoBehaviour
    {
        private Image _staminaGauge;

        private void Awake()
        {
            _staminaGauge = GetComponent<Image>();
        }

        public void UpdateStamina(float value)
        {
            _staminaGauge.fillAmount = value / 100;
        }
    }
}
