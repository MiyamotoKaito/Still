using TMPro;
using UnityEngine;

namespace Still.Player
{
    ///<summary>
    ///SAN値の表示
    ///</summary>
    public class SANView : MonoBehaviour
    {
        // PresenterはスコープのAwake（このViewのAwakeより前）で購読するため、取得を遅らせる
        private TextMeshProUGUI Text => _text != null ? _text : _text = GetComponent<TextMeshProUGUI>();

        private TextMeshProUGUI _text;

        ///<summary>
        ///SAN値を表示する
        ///</summary>
        public void UpdateSAN(float value)
        {
            Text.text = $"SAN {(int)value}%";
        }
    }
}
