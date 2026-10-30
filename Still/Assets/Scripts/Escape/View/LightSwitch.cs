using System;
using Still.Common;
using TMPro;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///部屋の照明スイッチ
    ///</summary>
    public class LightSwitch : MonoBehaviour, IInteractable, ILightSwitch
    {
        public event Action OnInteracted;

        ///<summary>点灯しているか</summary>
        public bool IsOn => _isOn;
        ///<summary>スイッチの位置</summary>
        public Transform Transform => transform;

        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Light _light;

        private bool _isOn;

        ///<summary>
        ///調べる
        ///</summary>
        public void Interact()
        {
            OnInteracted?.Invoke();
        }

        ///<summary>
        ///点灯状態を反映する
        ///</summary>
        public void SetOn(bool isOn)
        {
            _isOn = isOn;
            _light.enabled = isOn;
        }

        ///<summary>
        ///操作ガイドを表示する
        ///</summary>
        public void ShowUI()
        {
            _text.text = _isOn ? "Turn Off Light" : "Turn On Light";
            _text.gameObject.SetActive(true);
        }

        ///<summary>
        ///操作ガイドを隠す
        ///</summary>
        public void HideUI()
        {
            _text.gameObject.SetActive(false);
        }
    }
}
