using System;
using Still.Common;
using TMPro;
using UnityEngine;

namespace Still.Escape
{
    ///<summary>
    ///調べるとノイズが流れるテレビ
    ///</summary>
    public class Tv : MonoBehaviour, IInteractable
    {
        public event Action OnInteracted;

        ///<summary>ノイズを鳴らすAudioSource</summary>
        public AudioSource AudioSource => _audioSource;

        [SerializeField] private TextMeshProUGUI _text;

        private Light _tvLight;
        private AudioSource _audioSource;
        private bool _isOn;

        ///<summary>
        ///調べる（点いた後は反応しない）
        ///</summary>
        public void Interact()
        {
            if (_isOn) return;
            OnInteracted?.Invoke();
        }

        ///<summary>
        ///画面を点ける
        ///</summary>
        public void TurnOn()
        {
            _isOn = true;
            _tvLight.enabled = true;
            HideUI();
        }

        ///<summary>
        ///操作ガイドを表示する
        ///</summary>
        public void ShowUI()
        {
            if (_isOn) return;
            _text.text = "Watch TV";
            _text.gameObject.SetActive(true);
        }

        ///<summary>
        ///操作ガイドを隠す
        ///</summary>
        public void HideUI()
        {
            _text.gameObject.SetActive(false);
        }

        private void Awake()
        {
            _tvLight = GetComponentInChildren<Light>();
            _audioSource = GetComponent<AudioSource>();
            _tvLight.enabled = false;
        }
    }
}
