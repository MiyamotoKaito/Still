using TMPro;
using UnityEngine;
namespace Still.Player.View
{
    public class SANView : MonoBehaviour
    {
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void UpdateSAN(float value)
        {
            _text.text = $"SAN {(int)value}%";
        }
    }
}